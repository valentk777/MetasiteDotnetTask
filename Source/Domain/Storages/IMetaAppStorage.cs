using MetaApp.Domain.Models;

namespace MetaApp.Domain.Storages
{
    public interface IMetaAppStorage
    {
        Task SaveWeatherData(WeatherData data);
    }
}
