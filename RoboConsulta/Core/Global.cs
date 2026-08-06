using OpenQA.Selenium;

namespace RoboConsulta.Core
{
    public class Global
    {
        public IWebDriver driver = null!;
        public bool driverQuit = false;
        public bool headlessTest = false;
    }
}
