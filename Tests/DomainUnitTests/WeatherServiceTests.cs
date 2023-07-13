using Contracts.Configs;
using MetaApp.Domain.ApiClient;
using MetaApp.Domain.Displayer;
using MetaApp.Domain.Model;
using MetaApp.Domain.Service;
using MetaApp.Domain.Storage;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.Diagnostics.CodeAnalysis;

namespace MetaApp.DomainUnitTests;

[ExcludeFromCodeCoverage]
public class WeatherServiceTests
{
    [Fact]
    public void GivenWeatherService_WhenStartFetchingWeatherData_ThenShouldCallApiForEveryCity()
    {
        // Arrange
        var displayerMock = new Mock<IDisplayer>();
        var storageMock = new Mock<IStorage>();
        var loggerMock = new Mock<ILogger<WeatherService>>();

        var weatherConfigMock = new Mock<IOptions<WeatherConfig>>();
        var weatherConfig = new WeatherConfig
        {
            WeatherApiUrl = "url",
            FetchIntervalsInMiliseconds = 50000,
            WeatherApiUsername = "your-username",
            WeatherApiPassword = "your-password"
        };
        weatherConfigMock.Setup(x => x.Value).Returns(weatherConfig);

        var weatherApiClientMock = new Mock<IWeatherApiClient>();
        weatherApiClientMock
            .Setup(apiClient => apiClient.GetWeatherDataAsync(It.IsAny<string>()))
            .ReturnsAsync(new WeatherData("TestCity", 25, 90, "Sunny"));

        var cities = new List<string> { "Vilnius", "Riga" };

        // Act
        using (var weatherService = new WeatherService(
            loggerMock.Object,
            weatherApiClientMock.Object,
            storageMock.Object,
            displayerMock.Object,
            weatherConfigMock.Object))
        {
            weatherService.StartFetchingWeatherData(cities);
            Thread.Sleep(TimeSpan.FromMilliseconds(100));
        }

        // Assert
        weatherApiClientMock.Verify(apiClient => apiClient.GetWeatherDataAsync(It.IsAny<string>()), Times.Exactly(cities.Count));
        displayerMock.Verify(storage => storage.DisplayWeatherData(It.IsAny<WeatherData>()), Times.Exactly(cities.Count));
        storageMock.Verify(storage => storage.SaveWeatherDataAsync(It.IsAny<WeatherData>()), Times.Exactly(cities.Count));
    }

    [Fact]
    public void GivenWeatherService_WhenStartFetchingWeatherDataForTwoIterations_ThenShouldCallApiForEveryCityTwice()
    {
        // Arrange
        var displayerMock = new Mock<IDisplayer>();
        var storageMock = new Mock<IStorage>();
        var loggerMock = new Mock<ILogger<WeatherService>>();

        var weatherConfigMock = new Mock<IOptions<WeatherConfig>>();
        var weatherConfig = new WeatherConfig
        {
            WeatherApiUrl = "url",
            FetchIntervalsInMiliseconds = 500,
            WeatherApiUsername = "your-username",
            WeatherApiPassword = "your-password"
        };
        weatherConfigMock.Setup(x => x.Value).Returns(weatherConfig);

        var weatherApiClientMock = new Mock<IWeatherApiClient>();
        weatherApiClientMock
            .Setup(apiClient => apiClient.GetWeatherDataAsync(It.IsAny<string>()))
            .ReturnsAsync(new WeatherData("TestCity", 25, 90, "Sunny"));

        var cities = new List<string> { "Vilnius", "Riga" };

        // Act
        using (var weatherService = new WeatherService(
            loggerMock.Object,
            weatherApiClientMock.Object,
            storageMock.Object,
            displayerMock.Object,
            weatherConfigMock.Object))
        {
            weatherService.StartFetchingWeatherData(cities);

            // Assert
            // Wait for some time to allow the TimerElapsed method to execute twice (instantly + 500 + some time for excecution)
            Thread.Sleep(TimeSpan.FromMilliseconds(550));
        }

        weatherApiClientMock.Verify(apiClient => apiClient.GetWeatherDataAsync(It.IsAny<string>()), Times.Exactly(cities.Count * 2));
        displayerMock.Verify(storage => storage.DisplayWeatherData(It.IsAny<WeatherData>()), Times.Exactly(cities.Count * 2));
        storageMock.Verify(storage => storage.SaveWeatherDataAsync(It.IsAny<WeatherData>()), Times.Exactly(cities.Count * 2));
    }

    [Theory]
    [InlineData(null, 10, 80, "sunny")]
    [InlineData("TestCity", null, 80, "sunny")]
    [InlineData("TestCity", 10, null, "sunny")]
    [InlineData("TestCity", 10, 80, null)]
    public void GivenWeatherService_WhenStartFetchingIncorectWeatherData_ThenShouldNotDisplayAndSaveData(
        string city, int? temperatur, int? precipitation, string description)
    {
        // Arrange
        var displayerMock = new Mock<IDisplayer>();
        var storageMock = new Mock<IStorage>();
        var loggerMock = new Mock<ILogger<WeatherService>>();

        var weatherConfigMock = new Mock<IOptions<WeatherConfig>>();
        var weatherConfig = new WeatherConfig
        {
            WeatherApiUrl = "url",
            FetchIntervalsInMiliseconds = 50000,
            WeatherApiUsername = "your-username",
            WeatherApiPassword = "your-password"
        };
        weatherConfigMock.Setup(x => x.Value).Returns(weatherConfig);

        var weatherApiClientMock = new Mock<IWeatherApiClient>();
        weatherApiClientMock
            .Setup(apiClient => apiClient.GetWeatherDataAsync(It.IsAny<string>()))
            .ReturnsAsync(new WeatherData(city, temperatur, precipitation, description));

        var cities = new List<string> { "Vilnius" };

        // Act
        using (var weatherService = new WeatherService(
            loggerMock.Object,
            weatherApiClientMock.Object,
            storageMock.Object,
            displayerMock.Object,
            weatherConfigMock.Object))
        {
            weatherService.StartFetchingWeatherData(cities);
        }

        // Assert
        weatherApiClientMock.Verify(apiClient => apiClient.GetWeatherDataAsync(It.IsAny<string>()), Times.Once);
        displayerMock.Verify(storage => storage.DisplayWeatherData(It.IsAny<WeatherData>()), Times.Never);
        storageMock.Verify(storage => storage.SaveWeatherDataAsync(It.IsAny<WeatherData>()), Times.Never);
    }
}
