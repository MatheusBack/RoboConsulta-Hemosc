using OpenQA.Selenium.Chrome;
using RoboConsulta.Page;
using Xunit;

namespace RoboConsulta.Test
{
    public class HemoscSeleniumTest
    {
        [Fact]
        public void ExtractWithSelenium_ShouldHitBreakpoint()
        {
            // Ajuste: executar com o debugger ligado para que o breakpoint em ExtrairSanguesDisponiveis seja atingido
            var options = new ChromeOptions();
            options.AddArgument("start-maximized");
            // Remova o headless para ver o navegador enquanto depura

            using var driver = new ChromeDriver(options);
            var page = new HemoscPage();
            // inject driver into page (inherited from Global)
            page.driver = driver;

            try
            {
                page.Navegar();

                // Coloque o breakpoint em ExtrairSanguesDisponiveis aqui ou dentro do método
                var items = page.ExtrairSanguesDisponiveis().ToList();

                // Asserção mínima para garantir execução
                Assert.NotNull(items);
            }
            finally
            {
                driver.Quit();
                driver.Dispose();
            }
        }
    }
}
