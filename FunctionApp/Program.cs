using FunctionApp;
using Microsoft.Extensions.Hosting;

var serviceName = "tamas-test";

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices(services =>
    {
        services.AddDatadogModule(serviceName);
    })
    .Build();

host.Run();
