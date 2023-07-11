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
        var informationForDisplay = string.Format(
            Resources.WeatherDataForDisplayFormat, weatherData.City, weatherData.Temperature, weatherData.Description);

        informationForDisplay = informationForDisplay.Replace("\\n", Environment.NewLine);

        Console.WriteLine(informationForDisplay);
    }
}
