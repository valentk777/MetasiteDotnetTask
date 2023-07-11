namespace Contracts.Configs;

public class WeatherConfig
{
    public const string SectionName = "Application:Weather";

    public string WeatherApiUrl { get; init; } = default!;

    public double FetchIntervalsInMiliseconds { get; init; } = default!;

    public string WeatherApiUsername { get; init; } = default!;

    public string WeatherApiPassword { get; init; } = default!;
}