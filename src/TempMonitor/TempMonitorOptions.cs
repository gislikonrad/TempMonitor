namespace TempMonitor;

public class TempMonitorOptions
{
    public string? HostName { get; set; }
    public TimeSpan Interval { get; set; } = TimeSpan.FromSeconds(30);
}