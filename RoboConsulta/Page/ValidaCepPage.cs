using RoboConsulta.Core;

namespace RoboConsulta.Page
{
    public class ValidaCepPage : Tests
    {
        public void PreencheCep()
        {
            EscreverTexto("//input[@id='cep-input']", "89219522");
        }

        public void ClicaBotaoBuscar()
        {
            ClicarBotao("//*[@id=\'submit-btn\']");
        }

        public void ValidaResultado()
        {
            ValidaDados("/html/body/main/div/div/div[3]/div[2]/div[2]/div[1]/div/p", "Rui Barbosa");
        }
    }
}
