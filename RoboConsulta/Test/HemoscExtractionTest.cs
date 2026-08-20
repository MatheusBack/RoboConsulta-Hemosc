using RoboConsulta.Services;
using Xunit;

namespace RoboConsulta.Test
{
    public class HemoscExtractionTest
    {
        [Fact]
        public void ParseSampleHtml_ShouldExtractBloods()
        {
            var html = @"<p class=""text_verm font_dinn_medi2"">
  ""B+""
  <br>
  <span>Estável</span>
</p>
<p class=""text_verm font_dinn_medi2"">
  ""A-""
  <br>
  <span>Crítico</span>
</p>";

            var service = new HemoscService();
            var result = service.GetMissingBloodsWithHtmlAgility(html);

            Assert.Equal(2, result.Count);
            Assert.Contains(result, r => r.Tipo == "B+" && r.Status == "Estável");
            Assert.Contains(result, r => r.Tipo == "A-" && r.Status == "Crítico");
        }
    }
}
