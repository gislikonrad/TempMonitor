using System.Net;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Prometheus;

namespace TempMonitor;

public class Worker : IDisposable
{
    private readonly IMetricServer _metricServer;
    private readonly ITemperatureProvider _provider;
    private readonly ILogger<Worker> _logger;
    private readonly IDisposable? _optionsChangeToken;
    private TempMonitorOptions _options;

    public Worker(IMetricServer metricServer, ITemperatureProvider provider, IOptionsMonitor<TempMonitorOptions> monitor, ILogger<Worker> logger)
    {
        _metricServer = metricServer;
        _provider = provider;
        _logger = logger;

        _options = monitor.CurrentValue;
        _optionsChangeToken = monitor.OnChange(o => _options = o);
    }

    public async Task WorkAsync(CancellationToken cancellationToken)
    {
        try
        {
            _metricServer.Start();
        }
        catch (HttpListenerException ex)
        {
            _logger.LogCritical(ex, "Failed to start metric server");
            return;
        }
        
        _logger.LogInformation("Metric server started. Endpoint located at http://:1234/metrics");
        _logger.LogInformation("Press ctrl-c to close");
        
        var gauge = Metrics
            .WithLabels(new Dictionary<string, string> {{ "host", _options.HostName ?? Environment.MachineName }})
            .CreateGauge("host_temp", "Temperature of the host")
        ;

        while (!cancellationToken.IsCancellationRequested)
        {
            var celcius = await _provider.GetTemperatureAsync();
            gauge.Set(celcius);
            await Task.Delay(_options.Interval, cancellationToken);
        }
    }

    public void Dispose()
    {
        _metricServer.Dispose();
        _optionsChangeToken?.Dispose();
    }
}