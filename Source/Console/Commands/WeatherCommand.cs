using Contracts.Configs;
using MetaApp.Domain.Service;
using Microsoft.Extensions.Options;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;

namespace MetaApp.MetaAppConsole.Commands;

public class WeatherCommand : Command
{
    private readonly IWeatherService _weatherService;

    public WeatherCommand(IWeatherService weatherService, IOptions<WeatherConfig> config)
        : base(Constants.Commands.Weather, Resources.WeatherCommandDescription)
    {
        _weatherService = weatherService;

        AddOption(new Option<List<string>?>(
            name: Constants.Commands.CityParameter,
            description: Resources.CitiesParameterDescription,
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
