namespace Practical_Test_Xtramile.Infrastructure.WeatherApi;

public class OpenWeatherMapSettings
{
    public const string SectionName = "OpenWeatherMap";

    public string ApiKey { get; set; } = string.Empty;
    public string BaseUrl { get; set; } = "https://api.openweathermap.org/data/2.5/";
    public bool UseMock { get; set; } = false;
}
