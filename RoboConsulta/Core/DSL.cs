using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using NUnit.Framework;

namespace RoboConsulta.Core
{
    public class DSL : Global
    {
        private const int DefaultTimeout = 5;

        public void EscreverTexto(string xpath, string value)
        {
            try
            {
                var elemento = WaitElement(xpath);
                elemento.Clear();
                elemento.SendKeys(value);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Erro ao escrever texto no elemento '{xpath}'", ex);
            }
        }

        public void ClicarBotao(string elemento)
        {
            try
            {
                var botao = WaitElement(elemento);
                botao.Click();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Erro ao clicar no botão '{elemento}'", ex);
            }
        }

        public void ValidaDados(string xpath, string value)
        {
            try
            {
                var elemento = WaitElement(xpath);
                NUnit.Framework.Assert.That(elemento.Text, Does.Contain(value));
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Erro ao validar dados no elemento '{xpath}'", ex);
            }
        }

        private IWebElement WaitElement(string xpath)
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(DefaultTimeout));
            return wait.Until(d => d.FindElement(By.XPath(xpath)));
        }
    }
}
