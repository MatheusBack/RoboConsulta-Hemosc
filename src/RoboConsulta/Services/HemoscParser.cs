using HtmlAgilityPack;
using RoboConsulta.Models;

namespace RoboConsulta.Services;

public static class HemoscParser
{
    private const string BloodNodesXPath =
        "//p[contains(@class,'text_verm') and contains(@class,'font_dinn_medi2')]";

    private static readonly char[] TextTrimChars = [' ', '"', '\r', '\n', '\t'];

    public static IReadOnlyList<MissingBlood> Parse(string html)
    {
        var document = new HtmlDocument();
        document.LoadHtml(html);

        var nodes = document.DocumentNode.SelectNodes(BloodNodesXPath);
        if (nodes is null)
        {
            return [];
        }

        return nodes
            .Select(ToMissingBlood)
            .OfType<MissingBlood>()
            .ToList();
    }

    private static MissingBlood? ToMissingBlood(HtmlNode node)
    {
        var bloodType = node.ChildNodes
            .FirstOrDefault(child => child.NodeType == HtmlNodeType.Text)
            ?.InnerText.Trim(TextTrimChars);

        var status = node.SelectSingleNode(".//span")?.InnerText.Trim();

        if (string.IsNullOrWhiteSpace(bloodType) || string.IsNullOrWhiteSpace(status))
        {
            return null;
        }

        return new MissingBlood(bloodType, status);
    }
}
