using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using NUnit.Framework;


namespace RoboConsulta.Core
{
    public class Tests : DSL
    {
        private void AbaNavegador()
        {
            //Modo de Teste Headless
            var headlessMode = new ChromeOptions();
            headlessMode.AddArguments("window-size=1920,1080");
            headlessMode.AddArguments("disk-cache-size=0");
            headlessMode.AddArguments("headless");

            //Modo Desenvolvedor
            var devMode = new ChromeOptions();
            devMode.AddArguments("disk-cache-size=0");
            devMode.AddArguments("start-maximized");

            driver = headlessTest ? new ChromeDriver(headlessMode) : new ChromeDriver(devMode);
        }

        [SetUp]
        public void InicioTeste()
        {
            AbaNavegador();
            driver.Navigate().GoToUrl("https://buscacep.com.br/");
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
        }

        [TearDown]
        public void EndTest()
        {
            if (driverQuit)
            {
                driver.Quit();
                driver.Dispose();
            }
        }
    }
}