using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RoboConsulta.Configuration;
using RoboConsulta.Services;

namespace RoboConsulta.Workers;

public class BloodSearchWorker(
    BloodSearchJob job,
    IOptions<BloodSearchOptions> options,
    ILogger<BloodSearchWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var interval = TimeSpan.FromMinutes(options.Value.IntervalMinutes);
        logger.LogInformation("Worker started. Running every {Minutes} minutes", interval.TotalMinutes);

        using var timer = new PeriodicTimer(interval);

        do
        {
            await job.RunAsync(stoppingToken);
        }
        while (await timer.WaitForNextTickAsync(stoppingToken));
    }
}
