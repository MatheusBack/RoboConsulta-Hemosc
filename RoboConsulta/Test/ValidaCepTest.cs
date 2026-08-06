using RoboConsulta.Page;

namespace RoboConsulta.Test
{
    public class ValidaCepTest : ValidaCepPage
    {
        [Fact]
        public void ValidaCep()
        {
            PreencheCep();
            ClicaBotaoBuscar();
            ValidaResultado();
            driverQuit = true;
        }
    }
}
