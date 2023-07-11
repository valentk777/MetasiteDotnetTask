namespace MetaApp.Domain.Model;

public class WeatherData
{
    public string? City { get; set; }

    public string? Temperature { get; set; }

    public string? Description { get; set; }

    public bool IsValid() => City != null && Temperature != null && Description != null;
}
