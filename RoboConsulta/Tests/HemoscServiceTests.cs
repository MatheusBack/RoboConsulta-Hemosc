using Xunit;
using RoboConsulta.Services;

namespace RoboConsulta.Tests;

public class HemoscServiceTests
{
    [Fact]
    public void ParseMissingBloods_WithValidHtml_ReturnsCorrectData()
    {
        var html = @"
            <html>
            <body>
                <p class=""text_verm font_dinn_medi2"">""O+""<span>Em falta</span></p>
                <p class=""text_verm font_dinn_medi2"">""AB-""<span>Crítico</span></p>
            </body>
            </html>";

        var service = new HemoscService();
        var result = service.ParseMissingBloods(html);

        Assert.NotEmpty(result);
        Assert.Equal(2, result.Count);
        Assert.Single(result, b => b.BloodType == "O+" && b.Status == "Em falta");
        Assert.Single(result, b => b.BloodType == "AB-" && b.Status == "Crítico");
    }

    [Fact]
    public void ParseMissingBloods_WithEmptyHtml_ReturnsEmptyList()
    {
        var html = "<html><body></body></html>";

        var service = new HemoscService();
        var result = service.ParseMissingBloods(html);

        Assert.Empty(result);
    }
}
