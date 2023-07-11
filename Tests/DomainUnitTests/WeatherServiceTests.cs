using MetaApp.Domain.ApiClient;
using MetaApp.Domain.Displayer;
using MetaApp.Domain.Model;
using MetaApp.Domain.Service;
using MetaApp.Domain.Storage;
using Microsoft.Extensions.Logging;
using Moq;

namespace DomainUnitTests;

public class WeatherServiceTests
{
    [Fact]
    public void StartFetchingWeatherData_ShouldStartTimer_AndTriggerTimerElapsed()
    {
        // Arrange
        var weatherApiClientMock = new Mock<IWeatherApiClient>();

        weatherApiClientMock
            .Setup(apiClient => apiClient.GetWeatherDataAsync(It.IsAny<string>()))
            .ReturnsAsync(new WeatherData { City = "TestCity", Temperature = "25", Description = "Sunny" });

        var displayerMock = new Mock<IDisplayer>();
        var storageMock = new Mock<IStorage>();
        var loggerMock = new Mock<ILogger<WeatherService>>();

        var weatherService = new WeatherService(loggerMock.Object, weatherApiClientMock.Object, storageMock.Object, displayerMock.Object);
        var cities = new List<string> { "Vilnius", "Riga" };

        // Act
        weatherService.StartFetchingWeatherData(cities, 10000);

        // Assert
        // Wait for some time to allow the TimerElapsed method to execute
        Task.Delay(2000).Wait();

        weatherApiClientMock.Verify(apiClient => apiClient.GetWeatherDataAsync(It.IsAny<string>()), Times.Exactly(cities.Count));
        storageMock.Verify(storage => storage.SaveWeatherData(It.IsAny<WeatherData>()), Times.Exactly(cities.Count));
    }

    [Fact]
    public void TimerElapsed_ShouldDisplayWeatherData_AndSaveWeatherData()
    {

    }

    [Fact]
    public void TimerElapsed_WhenInvalidWeatherData_ShouldNotSaveWeatherData()
    {
    }
}
