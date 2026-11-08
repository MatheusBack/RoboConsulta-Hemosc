using RoboConsulta.Services;
using Xunit;

namespace RoboConsulta.Tests;

public class HemoscParserTests
{
    [Fact]
    public void Parse_WithValidHtml_ReturnsMissingBloods()
    {
        const string html = """
            <html><body>
                <p class="text_verm font_dinn_medi2">"O+"<span>Em falta</span></p>
                <p class="text_verm font_dinn_medi2">"AB-"<span>Crítico</span></p>
            </body></html>
            """;

        var result = HemoscParser.Parse(html);

        Assert.Equal(2, result.Count);
        Assert.Single(result, b => b.BloodType == "O+" && b.Status == "Em falta");
        Assert.Single(result, b => b.BloodType == "AB-" && b.Status == "Crítico");
    }

    [Fact]
    public void Parse_WithEmptyHtml_ReturnsEmptyList()
    {
        var result = HemoscParser.Parse("<html><body></body></html>");

        Assert.Empty(result);
    }

    [Fact]
    public void Parse_IgnoresEntriesWithoutStatus()
    {
        const string html = """
            <p class="text_verm font_dinn_medi2">"A+"</p>
            <p class="text_verm font_dinn_medi2">"B+"<span>Em falta</span></p>
            """;

        var result = HemoscParser.Parse(html);

        var blood = Assert.Single(result);
        Assert.Equal("B+", blood.BloodType);
    }
}
