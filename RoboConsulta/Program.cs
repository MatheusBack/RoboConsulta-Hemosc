using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using RoboConsulta.Services;

// Simple runner that fetches the Hemosc page, parses the missing bloods and writes to a text file.
// Run with `dotnet run` or start the project in the debugger.

var url = "https://www.hemosc.org.br/";
var outputFile = Path.Combine(Directory.GetCurrentDirectory(), "hemosc_bloods.txt");

try
{
    using var http = new HttpClient();
    Console.WriteLine($"Fetching {url} ...");
    var html = await http.GetStringAsync(url);

    var service = new HemoscService();
    var items = service.GetMissingBloodsWithHtmlAgility(html);

    if (items.Count == 0)
    {
        Console.WriteLine("No items found.");
        File.AppendAllText(outputFile, $"{DateTime.Now:o} - No items found{Environment.NewLine}");
    }
    else
    {
        Console.WriteLine($"Found {items.Count} items. Writing to {outputFile}");
        foreach (var it in items)
        {
            var line = $"{DateTime.Now:o} | {it.Tipo} | {it.Status}{Environment.NewLine}";
            File.AppendAllText(outputFile, line);
        }
    }

    Console.WriteLine("Done.");
}
catch (Exception ex)
{
    Console.Error.WriteLine("Error while fetching/parsing: " + ex.Message);
    File.AppendAllText(outputFile, $"{DateTime.Now:o} - Error: {ex.Message}{Environment.NewLine}");
}
