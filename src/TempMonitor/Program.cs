using System.Net;
using System.Security.Cryptography;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Prometheus;
using TempMonitor;

var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
    .AddJsonFile($"appsettings.{environment?.ToLower()}.json", optional: true, reloadOnChange: false)
    .AddEnvironmentVariables()
    .Build();

var collection = new ServiceCollection()
    .AddSingleton(configuration)
    .AddSingleton<IConfiguration>(p => p.GetRequiredService<IConfigurationRoot>())
    .Configure<TempMonitorOptions>(configuration.GetSection("TempMonitor"))
    .AddLogging(logging => logging.AddConfiguration(configuration.GetSection("Logging")).AddConsole())
    .AddSingleton<ITemperatureProvider, TemperatureProvider>()
    .AddSingleton<IMetricServer>(new MetricServer(port: 1234))
    .AddSingleton<Worker>();

#if DEBUG
collection.AddSingleton<ITempCommand, FakeTempCommand>();
#else
collection.AddSingleton<ITempCommand, PiTempCommand>();
#endif
var services = collection.BuildServiceProvider();

var source = new CancellationTokenSource();
Console.CancelKeyPress += (_, _) => source.Cancel(); 
var worker = services.GetRequiredService<Worker>();
await worker.WorkAsync(source.Token);