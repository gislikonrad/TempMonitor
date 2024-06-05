using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;

namespace TempMonitor;

public class TemperatureProvider : ITemperatureProvider
{
    private static readonly Regex _regex = new (@"^temp=(\d{1,2}\.\d)'C$");
    private readonly ITempCommand _command;
    private readonly ILogger<TemperatureProvider> _logger;

    public TemperatureProvider(ITempCommand command, ILogger<TemperatureProvider> logger)
    {
        _command = command;
        _logger = logger;
    }

    public async Task<double> GetTemperatureAsync()
    {
        var result = await _command.GetHostTemperatureAsync();
        _logger.LogDebug("Got host temperature result: {result}", result);
        return Parse(result);
    }

    private double Parse(string result)
    {
        var match = _regex.Match(result);
        var celcius = match.Groups[1].Value;
        return double.Parse(celcius, CultureInfo.InvariantCulture);
    }
}

public interface ITemperatureProvider
{
    Task<double> GetTemperatureAsync();
}