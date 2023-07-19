namespace FunctionApp.Datadog
{
    public class DatadogSettings
    {
        public string Environment { get; set; }
        public string ServiceName { get; set; }
        public string AgentUri { get; set; }
        public string[] EnabledIntegrations { get; set; }
        public string AppVersion { get; set; }
    }
}
