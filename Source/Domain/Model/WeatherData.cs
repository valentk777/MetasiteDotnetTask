namespace MetaApp.Domain.Model;

public class WeatherData
{
    public string? City { get; set; }

    public int? Temperature { get; set; }

    public int? Precipitation { get; set; }

    public string? Weather { get; set; }

    public WeatherData(string? city, int? temperature, int? precipitation, string? weather)
    {
        City = city;
        Temperature = temperature;
        Precipitation = precipitation;
        Weather = weather;
    }

    public bool IsValid() => City != null && Temperature != null && Precipitation != null && Weather != null;
}
