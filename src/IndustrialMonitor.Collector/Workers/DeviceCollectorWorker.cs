using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace IndustrialMonitor.Collector.Workers;

public class DeviceCollectorWorker : BackgroundService
{
    private readonly ILogger<DeviceCollectorWorker> _logger;

    public DeviceCollectorWorker(ILogger<DeviceCollectorWorker> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Collector started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Collector running at: {time}", DateTimeOffset.Now);
            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }
    }
}
