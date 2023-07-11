namespace Contracts.Configs;

public class ApplicationConfig
{
    public const string SectionName = "Application";

    public WeatherConfig Weather { get; init; } = default!;
}