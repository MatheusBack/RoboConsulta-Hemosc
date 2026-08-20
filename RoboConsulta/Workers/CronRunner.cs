using Microsoft.Extensions.Logging;
using RoboConsulta.Services;

namespace RoboConsulta.Workers;

public class CronRunner
{
    private readonly ILogger<CronRunner> _logger;
    private readonly HemoscService _hemoscService;

    public CronRunner(ILogger<CronRunner> logger, HemoscService hemoscService)
    {
        _logger = logger;
        _hemoscService = hemoscService;
    }

    public async Task ExecuteAsync()
    {
        _logger.LogInformation("Starting blood search at {DateTime}", DateTime.UtcNow);

        try
        {
            var missingBloods = await _hemoscService.GetMissingBloodsAsync();

            if (missingBloods.Count == 0)
            {
                _logger.LogInformation("No missing bloods found");
                return;
            }

            _logger.LogInformation("Found {Count} missing bloods", missingBloods.Count);

            foreach (var blood in missingBloods)
            {
                _logger.LogInformation("Blood Type: {BloodType} | Criticality: {Status}", blood.BloodType, blood.Status);
            }

            _logger.LogInformation("Blood search completed successfully");
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("Failed to fetch"))
        {
            _logger.LogWarning(ex, "Failed to fetch data from Hemosc website. The website may be blocking automated requests or temporarily unavailable.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during blood search");
        }
    }
}
