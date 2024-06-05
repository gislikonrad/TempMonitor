using System.Diagnostics;
using System.Globalization;
using System.Security.Cryptography;
using Microsoft.Extensions.Logging;

namespace TempMonitor;

public class FakeTempCommand : ITempCommand
{
    private readonly ILogger<FakeTempCommand> _logger;

    public FakeTempCommand(ILogger<FakeTempCommand> logger)
    {
        _logger = logger;
    }
    public async Task<string> GetHostTemperatureAsync()
    {
        _logger.LogDebug("Creating fake temperature between 40.0 and 55.0");
        var random = RandomNumberGenerator.GetInt32(400, 550);
        var celcius = ((double)random) / 10;
        var str = celcius.ToString("N1", CultureInfo.InvariantCulture);
        _logger.LogDebug("Created {value}", str);
        return $"temp={str}'C";
    }
}

public class PiTempCommand : ITempCommand
{
    private readonly ILogger<PiTempCommand> _logger;

    public PiTempCommand(ILogger<PiTempCommand> logger)
    {
        _logger = logger;
    }
    public async Task<string> GetHostTemperatureAsync()
    {
        if (!File.Exists("/bin/vcgencmd"))
            throw new ApplicationException(
                "/bin/vcgencmd not found. This container is designed to be run on a Raspberry Pi.");
        
        _logger.LogDebug("Running '{command}'", "/bin/vcgencmd measure_temp");
        var info = new ProcessStartInfo("/bin/vcgencmd", "measure_temp");
        var process = new Process
        {
            StartInfo = info
        };
        process.Start();
        while (!process.HasExited)
            await Task.Delay(100);

        _logger.LogDebug("Exit code from command: {code}",process.ExitCode);
        if (process.ExitCode == 0)
            return await process.StandardOutput.ReadToEndAsync();
        throw new ApplicationException("Unable to read host temperature" + Environment.NewLine +
                                       await process.StandardError.ReadToEndAsync());
    }
}

public interface ITempCommand
{
    Task<string> GetHostTemperatureAsync();
}