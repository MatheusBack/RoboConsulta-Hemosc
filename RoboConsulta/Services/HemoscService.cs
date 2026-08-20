using System.Net.Http;
using HtmlAgilityPack;
using OpenQA.Selenium;
using RoboConsulta.Page;
using RoboConsulta.Data;

namespace RoboConsulta.Services
{
    public record MissingBloodModel(string Tipo, string Status);

    public class HemoscService
    {
        private readonly HemoscPage? _page;

        public HemoscService(HemoscPage? page = null)
        {
            _page = page;
        }

        public List<MissingBloodModel> GetMissingBloodsWithSelenium()
        {
            if (_page == null) throw new InvalidOperationException("HemoscPage is not available. Construct HemoscService with a HemoscPage to use Selenium.");

            _page.Navegar();
            var list = new List<MissingBloodModel>();

            var nodes = _page.driver.FindElements(By.CssSelector("p.text_verm.font_dinn_medi2"));
            foreach (var node in nodes)
            {
                var fullText = node.Text;
                var lines = fullText.Split('\n', StringSplitOptions.RemoveEmptyEntries);
                var tipo = lines.Length > 0 ? lines[0].Trim(' ', '"') : string.Empty;
                var status = "";
                try
                {
                    status = node.FindElement(By.TagName("span")).Text.Trim();
                }
                catch { }
                list.Add(new MissingBloodModel(tipo, status));
            }

            return list;
        }

        public List<MissingBloodModel> GetMissingBloodsWithHtmlAgility(string html)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var list = new List<MissingBloodModel>();
            var nodes = doc.DocumentNode.SelectNodes("//p[contains(@class,'text_verm') and contains(@class,'font_dinn_medi2')]");
            if (nodes == null) return list;

            foreach (var node in nodes)
            {
                var tipo = node.ChildNodes.FirstOrDefault(n => n.NodeType == HtmlNodeType.Text)?.InnerText.Trim(' ', '"') ?? string.Empty;
                var span = node.SelectSingleNode(".//span");
                var status = span?.InnerText.Trim() ?? string.Empty;
                list.Add(new MissingBloodModel(tipo, status));
            }

            return list;
        }
    }
}
