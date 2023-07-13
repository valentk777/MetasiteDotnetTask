using Contracts.Configs;
using Contracts.Exceptions;
using MetaApp.Domain.Displayer;
using MetaApp.Domain.Service;
using MetaApp.Domain.Storage;
using MetaApp.Integrations.Displayer;
using MetaApp.Integrations.Storage;
using MetaApp.MetaAppConsole.Commands;
using MetaApp.MetaAppConsole.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.CommandLine;

namespace MetaApp.MetaAppConsole;

public static class WeatherApp
{
    public static void Run(string[] args)
    {
        IConfigurationRoot configuration = GetConfiguration();
        ServiceProvider serviceProvider = RegisterServices(configuration);
        WeatherCommand weatherCommand = GetWeatherCommand(serviceProvider);

        var rootCommand = new RootCommand
        {
           weatherCommand
        };

        rootCommand.Invoke(args);
    }

    private static WeatherCommand GetWeatherCommand(ServiceProvider serviceProvider)
    {
        var weatherService = serviceProvider.GetService<IWeatherService>();
        var weatherConfig = serviceProvider.GetRequiredService<IOptions<WeatherConfig>>();

        if (weatherService == null)
        {
            throw new ApplicationValidationException(string.Format(Resources.IssuesWithRegistration, nameof(IWeatherService)));
        }

        return new WeatherCommand(weatherService, weatherConfig);
    }

    private static ServiceProvider RegisterServices(IConfigurationRoot configuration) =>
        new ServiceCollection()
            .ConfigureAllOptions(configuration)
            .AddWeatherCommandHandler()
            .AddSingleton<IStorage, FileStorage>()
            .AddSingleton<IDisplayer, ConsoleDisplayer>()
            .AddLogging(builder =>
            {
                builder.AddConfiguration(configuration.GetSection(LoggingConfig.SectionName));
                builder.AddConsole();
            })

            .AddLogging(builder => builder.AddConsole())
            .BuildServiceProvider();

    private static IConfigurationRoot GetConfiguration() =>
        new ConfigurationBuilder()
         .SetBasePath(AppContext.BaseDirectory)
         .AddJsonFile(Constants.AppSettingsFile, optional: true, reloadOnChange: true)
         .Build();
}
