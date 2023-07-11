using Contracts.Exceptions;
using MetaApp.Domain.Model;
using MetaApp.Domain.Storage;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Reflection;

namespace MetaApp.Integrations.Storage;

public class FileStorage : IStorage
{
    private readonly ILogger<FileStorage> _logger;

    public FileStorage(ILogger<FileStorage> logger)
    {
        _logger = logger;
    }

    private readonly string? currentLocation = GetFileStorageLocation();

    public async Task SaveWeatherData(WeatherData data)
    {
        if (currentLocation == null)
        {
            _logger.LogError(Resources.PathIsNull);
            throw new ApplicationValidationException(Resources.PathIsNull);
        }

        var filePath = Path.Combine(currentLocation, $"{DateTime.Now.ToString("yyyyMMdd_HHmmss")}.txt");

        _logger.LogInformation(string.Format(Resources.FileStoragePath, filePath));

        using (var streamWriter = new StreamWriter(filePath))
        {
            await streamWriter.WriteAsync(JsonConvert.SerializeObject(data));
        }
    }

    private static string GetFileStorageLocation()
    {
        var currentModule = Assembly.GetEntryAssembly()?.Location;
        var parentDirectory = Directory.GetParent(currentModule).Parent.Parent.Parent.Parent.Parent.FullName;
        var storageDirectory = Path.Join(parentDirectory, "FileStorage");

        return storageDirectory;
    }
}
