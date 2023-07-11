using MetaApp.Domain.Model;

namespace MetaApp.Domain.ApiClient;

public interface IWeatherApiClient
{
    Task<WeatherData> GetWeatherDataAsync(string city);
}