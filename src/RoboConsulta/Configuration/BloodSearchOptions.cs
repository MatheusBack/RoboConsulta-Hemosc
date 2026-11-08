namespace RoboConsulta.Configuration;

public class BloodSearchOptions
{
    public const string SectionName = "BloodSearch";

    public string HemoscUrl { get; set; } = "https://www.hemosc.org.br/";
    public int IntervalMinutes { get; set; } = 30;
    public string DatabasePath { get; set; } = "blood_search.db";
}
