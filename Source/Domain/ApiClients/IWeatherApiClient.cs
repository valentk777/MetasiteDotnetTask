using MetaApp.Domain.Models;

namespace MetaApp.Domain.ApiClients
{
    public interface IWeatherApiClient
    {
        Task<WeatherData> GetWeatherDataAsync(string city);
    }
}