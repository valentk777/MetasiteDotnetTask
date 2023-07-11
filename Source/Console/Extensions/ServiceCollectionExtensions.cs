using Contracts.Configs;
using MetaApp.Domain.ApiClient;
using MetaApp.Domain.Service;
using MetaApp.Integrations.ApiClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MetaApp.MetaAppConsole.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddWeatherCommandHandler(this IServiceCollection service) =>
        service
            .AddSingleton<IWeatherService, WeatherService>()
            .AddSingleton<IWeatherApiClient, WeatherApiClient>();

    public static IServiceCollection ConfigureAllOptions(this IServiceCollection services, IConfiguration configuration) =>
        services
            .Configure<ApplicationConfig>(configuration.GetSection(ApplicationConfig.SectionName))
            .Configure<WeatherConfig>(configuration.GetSection(WeatherConfig.SectionName))
            .Configure<LoggingConfig>(configuration.GetSection(LoggingConfig.SectionName));
}
