using MetaApp.Domain.Model;

namespace MetaApp.Domain.Displayer;

public interface IDisplayer
{
    void DisplayWeatherData(WeatherData weatherData);
}