using MetaApp.Domain.ApiClients;
using MetaApp.Domain.Models;
using MetaApp.Domain.Storages;
using System.Timers;

namespace MetaApp.Domain.Service;

public class WeatherService : IWeatherService
{
    private const int resetTimerInterval = 30000;  // 30 seconds

    private readonly IWeatherApiClient _weatherApiClient;
    private readonly IMetaAppStorage _storage;

    private System.Timers.Timer _timer = new System.Timers.Timer(resetTimerInterval);

    public WeatherService(
        IWeatherApiClient weatherApiClient,
        IMetaAppStorage storage)
    {
        _weatherApiClient = weatherApiClient;
        _storage = storage;
    }

    public void StartFetchingWeatherData(List<string> cities)
    {
        _timer = new System.Timers.Timer(30000); // 30 seconds
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
            var weatherData = await _weatherApiClient.GetWeatherDataAsync(city);

            DisplayWeatherData(weatherData);

            await SaveWeatherData(weatherData);
        });
    }

    private void DisplayWeatherData(WeatherData weatherData)
    {
        // TODO: To constants.
        Console.WriteLine($"City: {weatherData.City}");
        Console.WriteLine($"Temperature: {weatherData.Temperature}");
        Console.WriteLine($"Description: {weatherData.Description}");
        Console.WriteLine();
    }

    private async Task SaveWeatherData(WeatherData weatherData)
    {
        if (weatherData.IsValid())
        {
            await _storage.SaveWeatherData(weatherData);
        }
    }
}
