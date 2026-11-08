using Microsoft.Extensions.Logging;
using RoboConsulta.Data;
using RoboConsulta.Models;

namespace RoboConsulta.Services;

public class BloodSearchJob(
    HemoscClient hemosc,
    BloodSearchRepository repository,
    ILogger<BloodSearchJob> logger)
{
    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Starting blood search");

        try
        {
            var html = await hemosc.GetHomePageAsync(cancellationToken);
            var missingBloods = HemoscParser.Parse(html);

            if (missingBloods.Count == 0)
            {
                logger.LogInformation("No missing bloods found");
                return;
            }

            var searchedAt = DateTime.UtcNow;
            var logs = missingBloods
                .Select(blood => new BloodSearchLog
                {
                    BloodType = blood.BloodType,
                    Status = blood.Status,
                    SearchedAt = searchedAt
                })
                .ToList();

            await repository.InsertAsync(logs);

            foreach (var blood in missingBloods)
            {
                logger.LogInformation("Missing blood: {BloodType} - {Status}", blood.BloodType, blood.Status);
            }
        }
        catch (HemoscUnavailableException ex)
        {
            logger.LogWarning(ex, "Hemosc website unavailable or blocking requests");
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogError(ex, "Unexpected error during blood search");
        }
    }
}
