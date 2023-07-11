using Contracts.Configs;
using MetaApp.Domain.ApiClient;
using MetaApp.Domain.Displayer;
using MetaApp.Domain.Storage;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Timers;

namespace MetaApp.Domain.Service;

public class WeatherService : IWeatherService, IDisposable
{
    private readonly ILogger<WeatherService> _logger;
    private readonly IWeatherApiClient _weatherApiClient;
    private readonly IStorage _storage;
    private readonly IDisplayer _displayer;
    private readonly WeatherConfig _config;
    private readonly System.Timers.Timer _timer;

    public WeatherService(
        ILogger<WeatherService> logger,
        IWeatherApiClient weatherApiClient,
        IStorage storage,
        IDisplayer displayer,
        IOptions<WeatherConfig> config)
    {
        _logger = logger;
        _weatherApiClient = weatherApiClient;
        _storage = storage;
        _displayer = displayer;
        _config = config.Value;
        _timer = new System.Timers.Timer(_config.FetchIntervalsInMiliseconds);
    }

    public void StartFetchingWeatherData(List<string> cities)
    {
        _logger.LogInformation(string.Format(Resources.StartFeatchingWeatherData, cities, _config.FetchIntervalsInMiliseconds));

        _timer.Elapsed += (sender, e) => TimerElapsed(sender, e, cities);
        _timer.AutoReset = true;

        // Execute the TimerElapsed method immediately upon starting the timer
        TimerElapsed(null, null, cities);

        _timer.Start();
    }

    private void TimerElapsed(object sender, ElapsedEventArgs e, List<string> cities)
    {
        cities.ForEach(async city =>
        {
            await HandleDataCollection(city);
        });
    }

    private async Task HandleDataCollection(string city)
    {
        _logger.LogInformation(string.Format(Resources.GettingDataForCity, city));

        var weatherData = await _weatherApiClient.GetWeatherDataAsync(city);

        if (weatherData.IsValid())
        {
            _logger.LogInformation(string.Format(Resources.DataForCityCollected, city, weatherData));

            _displayer.DisplayWeatherData(weatherData);

            await _storage.SaveWeatherData(weatherData);
        }
        else
        {
            _logger.LogError(string.Format(Resources.DataForCityNotCollected, city, weatherData));
        }
    }

    public void Dispose()
    {
        _timer.Stop();
        _timer.Dispose();
    }
}
