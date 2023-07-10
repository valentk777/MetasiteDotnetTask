using MetaApp.Domain.Service;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;

namespace MetaApp.Domain.Commands
{
    public class WeatherCommand : Command
    {
        private IWeatherService _weatherService;

        public WeatherCommand(IWeatherService weatherService) : base(Constants.Commands.Weather, "Fetches and saves current weather data for the specified cities.")
        {
            _weatherService = weatherService;

            AddOption(new Option<List<string>?>(
                name: "--city",
                description: "The cities for which to fetch weather data.",
                parseArgument: result =>
                {
                    var cityValues = result.Tokens
                        .Select(t => t.Value)
                        .SelectMany(value => value.Split(","))
                        .Select(city => city.Trim())
                        .ToList();

                    return cityValues.Any() ? cityValues : null;
                }));

            Handler = CommandHandler.Create<List<string>>(HandleWeatherCommand);
        }

        private void HandleWeatherCommand(List<string> city) =>
            _weatherService.StartFetchingWeatherData(city);
    }
}
