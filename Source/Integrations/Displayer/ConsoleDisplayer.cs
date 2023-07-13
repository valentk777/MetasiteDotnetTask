using MetaApp.Domain.Displayer;
using MetaApp.Domain.Model;
using Microsoft.Extensions.Logging;

namespace MetaApp.Integrations.Displayer;

public class ConsoleDisplayer : IDisplayer
{
    private readonly ILogger<ConsoleDisplayer> _logger;

    public ConsoleDisplayer(ILogger<ConsoleDisplayer> logger)
    {
        _logger = logger;
    }

    public void DisplayWeatherData(WeatherData weatherData)
    {
        if (weatherData == null)
        {
            _logger.LogError(Resources.WeatherDataIsNull);
            return;
        }

        _logger.LogInformation(Resources.DislplayWeatherData);
        var informationForDisplay = string.Format(
            Resources.WeatherDataForDisplayFormat, DateTime.UtcNow, weatherData.City, weatherData.Temperature, weatherData.Precipitation, weatherData.Weather);

        informationForDisplay = informationForDisplay.Replace("\\n", Environment.NewLine);

        Console.WriteLine(informationForDisplay);
    }
}
