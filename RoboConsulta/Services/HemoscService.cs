using HtmlAgilityPack;
using RoboConsulta.Models;

namespace RoboConsulta.Services;

public class HemoscService
{
    private const string HemoscUrl = "https://www.hemosc.org.br/";
    private readonly HttpClient _httpClient;
    private const int MaxRetries = 3;
    private const int InitialDelayMs = 1000;

    public HemoscService(HttpClient? httpClient = null)
    {
        _httpClient = httpClient ?? new HttpClient();
        ConfigureHttpClient();
    }

    private void ConfigureHttpClient()
    {
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");
        _httpClient.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");
        _httpClient.DefaultRequestHeaders.Add("Accept-Language", "pt-BR,pt;q=0.9,en-US;q=0.8,en;q=0.7");
        _httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");
        _httpClient.DefaultRequestHeaders.Add("Referer", "https://www.google.com/");
        _httpClient.DefaultRequestHeaders.Add("DNT", "1");
        _httpClient.Timeout = TimeSpan.FromSeconds(30);
    }

    public async Task<List<MissingBlood>> GetMissingBloodsAsync()
    {
        for (int attempt = 1; attempt <= MaxRetries; attempt++)
        {
            try
            {
                var html = await FetchWithTimeoutAsync();
                return ParseMissingBloods(html);
            }
            catch (HttpRequestException) when (attempt < MaxRetries)
            {
                int delayMs = InitialDelayMs * (int)Math.Pow(2, attempt - 1);
                await Task.Delay(delayMs);
                continue;
            }
            catch (HttpRequestException ex)
            {
                throw new InvalidOperationException($"Failed to fetch Hemosc website: {HemoscUrl} after {MaxRetries} attempts", ex);
            }
        }

        throw new InvalidOperationException($"Failed to fetch Hemosc website: {HemoscUrl}");
    }

    private async Task<string> FetchWithTimeoutAsync()
    {
        using (var request = new HttpRequestMessage(HttpMethod.Get, HemoscUrl))
        {
            request.Headers.Add("Connection", "keep-alive");
            request.Headers.Add("Upgrade-Insecure-Requests", "1");

            using (var response = await _httpClient.SendAsync(request))
            {
                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException($"Response status code does not indicate success: {(int)response.StatusCode} ({response.StatusCode}).");
                }
                return await response.Content.ReadAsStringAsync();
            }
        }
    }

    public List<MissingBlood> ParseMissingBloods(string html)
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        var list = new List<MissingBlood>();
        var nodes = doc.DocumentNode.SelectNodes("//p[contains(@class,'text_verm') and contains(@class,'font_dinn_medi2')]");

        if (nodes == null) return list;

        foreach (var node in nodes)
        {
            var bloodType = ExtractBloodType(node);
            var status = ExtractStatus(node);

            if (!string.IsNullOrWhiteSpace(bloodType) && !string.IsNullOrWhiteSpace(status))
            {
                list.Add(new MissingBlood(bloodType, status));
            }
        }

        return list;
    }

    private static string ExtractBloodType(HtmlNode node)
    {
        var textNode = node.ChildNodes
            .FirstOrDefault(n => n.NodeType == HtmlNodeType.Text);

        if (textNode != null)
        {
            return textNode.InnerText.Trim(' ', '"', '\r', '\n', '\t');
        }

        return string.Empty;
    }

    private static string ExtractStatus(HtmlNode node)
    {
        var span = node.SelectSingleNode(".//span");
        return span?.InnerText.Trim() ?? string.Empty;
    }
}
