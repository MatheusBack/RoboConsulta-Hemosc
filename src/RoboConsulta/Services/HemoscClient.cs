using Microsoft.Extensions.Options;
using RoboConsulta.Configuration;

namespace RoboConsulta.Services;

public class HemoscClient(HttpClient http, IOptions<BloodSearchOptions> options)
{
    private const int MaxAttempts = 3;

    private readonly string _url = options.Value.HemoscUrl;

    public async Task<string> GetHomePageAsync(CancellationToken cancellationToken = default)
    {
        for (var attempt = 1; ; attempt++)
        {
            try
            {
                return await http.GetStringAsync(_url, cancellationToken);
            }
            catch (Exception ex) when (IsTransient(ex, cancellationToken))
            {
                if (attempt == MaxAttempts)
                {
                    throw new HemoscUnavailableException(
                        $"Failed to fetch {_url} after {MaxAttempts} attempts", ex);
                }

                var delay = TimeSpan.FromSeconds(Math.Pow(2, attempt - 1));
                await Task.Delay(delay, cancellationToken);
            }
        }
    }

    private static bool IsTransient(Exception ex, CancellationToken cancellationToken) =>
        ex is HttpRequestException ||
        (ex is TaskCanceledException && !cancellationToken.IsCancellationRequested);
}
