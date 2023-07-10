using MetaApp.Domain.ApiClients;
using MetaApp.Domain.Commands;
using MetaApp.Domain.Service;
using MetaApp.Domain.Storages;
using MetaApp.Integrations.FileStorage;
using MetaApp.Integrations.WeatherApiClient;
using Microsoft.Extensions.DependencyInjection;
using System.CommandLine;

namespace MetaApp.MetaApp;

public static class WeatherApp
{
    // TODO: add logging.
    public static void Run(string[] args)
    {
        var serviceProvider = new ServiceCollection()
            .AddSingleton<IWeatherService, WeatherService>()
            .AddSingleton<IWeatherApiClient, WeatherApiClient>()
            .AddSingleton<IMetaAppStorage, FileStorage>()
            .BuildServiceProvider();

        var weatherService = serviceProvider.GetService<IWeatherService>();

        var rootCommand = new RootCommand
        {
            new WeatherCommand(weatherService)
        };

        rootCommand.Invoke(args);
    }
}
