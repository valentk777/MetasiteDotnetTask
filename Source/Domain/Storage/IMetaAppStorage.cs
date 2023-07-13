using MetaApp.Domain.Model;

namespace MetaApp.Domain.Storage;

public interface IStorage
{
    Task SaveWeatherDataAsync(WeatherData data);
}
