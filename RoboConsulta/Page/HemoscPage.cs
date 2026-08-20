using OpenQA.Selenium;
using RoboConsulta.Core;

namespace RoboConsulta.Page
{
    public class HemoscPage : Tests
    {
        private readonly string _url = "https://www.hemosc.org.br/"; // Ajuste se necessário

        public void Navegar()
        {
            driver.Navigate().GoToUrl(_url);
        }

        public IEnumerable<(string Tipo, string Status)> ExtrairSanguesDisponiveis()
        {
            // Exemplo: os elementos têm a classe 'text_verm font_dinn_medi2'
            var nodes = driver.FindElements(By.CssSelector("p.text_verm.font_dinn_medi2"));
            foreach (var node in nodes)
            {
                var tipo = node.FindElements(By.XPath("./text()"));
                yield return (Tipo: node.Text.Split('\n')[0].Trim(' ', '"'), Status: node.FindElement(By.TagName("span")).Text.Trim());
            }
        }
    }
}
