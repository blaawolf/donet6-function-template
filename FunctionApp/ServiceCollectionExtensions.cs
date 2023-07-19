using Datadog.Trace;
using Datadog.Trace.Configuration;
using FunctionApp.Datadog;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Datadog.Logs;
using StatsdClient;

namespace FunctionApp
{
    public static class ServiceCollectionExtensions
    {
        private static string DataDogApiKey => Environment.GetEnvironmentVariable("DataDogApiKey");
        private static string DataDogAgentUri => Environment.GetEnvironmentVariable("DataDogAgentUri");
        private static string DataDogStatsDHost => Environment.GetEnvironmentVariable("DataDogStatsDHost");
        private static string DataDogLogUri => Environment.GetEnvironmentVariable("DataDogLogUri") ?? "https://http-intake.logs.us3.datadoghq.com";
        private static string ServiceEnvironment => "hackathon";
        private static LogEventLevel GetLogLevel()
        {
            string logLevelStr = Environment.GetEnvironmentVariable("DataDogLogLevel");
            if (String.IsNullOrEmpty(logLevelStr)) return LogEventLevel.Debug;
            LogEventLevel level = (LogEventLevel)Enum.Parse(typeof(LogEventLevel), logLevelStr);
            return level;
        }

        public static IServiceCollection AddDatadogModule(this IServiceCollection serviceCollection, string serviceName)
        {
            serviceName = serviceName.ToLower();
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.DatadogLogs(
                    apiKey: DataDogApiKey,
                    host: Environment.MachineName,
                    source: "csharp",
                    service: serviceName,
                    configuration: new DatadogConfiguration()
                    {
                        Url = DataDogLogUri
                    },
                    logLevel: GetLogLevel(),
                    tags: new string[] { $"service:{serviceName}", $"env:{ServiceEnvironment}" }
                )
                .CreateLogger();

            if (!string.IsNullOrWhiteSpace(DataDogAgentUri))
            {
                AddDatadogTracing(serviceName);
            }

            if (!string.IsNullOrEmpty(DataDogStatsDHost))
            {
                ConfigureDatadogStatsD(serviceName);
            }

            return serviceCollection;
        }

        private static void AddDatadogTracing(string serviceName)
        {
            var settings = new DatadogSettings
            {
                AgentUri = DataDogAgentUri,
                EnabledIntegrations = new[] { "AspNet", "AspNetMvc", "AspNetWebApi2", "HttpMessageHandler", "WebRequest" },
                Environment = ServiceEnvironment,
                ServiceName = serviceName
            };

            TracerSettings datadogSettings = TracerSettings.FromDefaultSources();
            datadogSettings.AnalyticsEnabled = true;
            datadogSettings.TraceEnabled = true;
            datadogSettings.TracerMetricsEnabled = true;
            datadogSettings.LogsInjectionEnabled = true;
            datadogSettings.Environment = settings.Environment;
            datadogSettings.ServiceName = settings.ServiceName;
            datadogSettings.ServiceVersion = settings.AppVersion;

            if (!string.IsNullOrEmpty(settings.AgentUri))
            {
                datadogSettings.Exporter.AgentUri = new Uri(settings.AgentUri);
            }

            foreach (var enabledIntegration in settings.EnabledIntegrations)
            {
                datadogSettings.Integrations[enabledIntegration].Enabled = true;
                datadogSettings.Integrations[enabledIntegration].AnalyticsSampleRate = 1.0;
                datadogSettings.Integrations[enabledIntegration].AnalyticsEnabled = true;
            }

            Tracer.Configure(datadogSettings);
        }

        private static void ConfigureDatadogStatsD(string serviceName)
        {
            var config = new StatsdConfig
            {
                StatsdServerName = DataDogStatsDHost,
                ServiceName = serviceName,
                Prefix = serviceName,
                ConstantTags = new[] { $"host:{Environment.MachineName}" },
                Environment = ServiceEnvironment
            };

            DogStatsd.Configure(config);
        }
    }
}
