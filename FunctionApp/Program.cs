using FunctionApp;
using Microsoft.Extensions.Hosting;

var serviceName = Environment.GetEnvironmentVariable("AppName");

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices(services =>
    {
        services.AddDatadogModule(serviceName);
    })
    .Build();

host.Run();
