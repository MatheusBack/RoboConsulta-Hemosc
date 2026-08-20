using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RoboConsulta.Data;
using RoboConsulta.Models;
using RoboConsulta.Services;

namespace RoboConsulta.Workers;

public class BloodSearchWorker : BackgroundService
{
    private const int IntervalMinutes = 30;
    private readonly ILogger<BloodSearchWorker> _logger;
    private readonly HemoscService _hemoscService;
    private readonly DatabaseContext _databaseContext;

    public BloodSearchWorker(
        ILogger<BloodSearchWorker> logger,
        HemoscService hemoscService,
        DatabaseContext databaseContext)
    {
        _logger = logger;
        _hemoscService = hemoscService;
        _databaseContext = databaseContext;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("BloodSearchWorker started. Running every {IntervalMinutes} minutes", IntervalMinutes);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ExecuteSearchAsync(stoppingToken);
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("Failed to fetch"))
            {
                _logger.LogWarning(ex, "Failed to fetch data from Hemosc website. The website may be blocking automated requests or temporarily unavailable. Will retry in {IntervalMinutes} minutes", IntervalMinutes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during blood search");
            }

            await Task.Delay(TimeSpan.FromMinutes(IntervalMinutes), stoppingToken);
        }

        _logger.LogInformation("BloodSearchWorker stopping");
    }

    private async Task ExecuteSearchAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting blood search at {DateTime}", DateTime.UtcNow);

        var missingBloods = await _hemoscService.GetMissingBloodsAsync();

        if (missingBloods.Count == 0)
        {
            _logger.LogInformation("No missing bloods found");
            return;
        }

        _logger.LogInformation("Found {Count} missing bloods", missingBloods.Count);

        foreach (var blood in missingBloods)
        {
            var log = new BloodSearchLog
            {
                BloodType = blood.BloodType,
                Status = blood.Status,
                SearchedAt = DateTime.UtcNow
            };

            await _databaseContext.InsertBloodSearchLogAsync(log);
            _logger.LogInformation("Logged missing blood: {BloodType} - {Status}", blood.BloodType, blood.Status);
        }

        _logger.LogInformation("Blood search completed successfully");
    }
}
