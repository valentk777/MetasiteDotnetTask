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

    private const string FileNameFormat = "yyyyMMdd_HHmmss";

    public FileStorage(ILogger<FileStorage> logger)
    {
        _logger = logger;
    }

    private readonly string? currentLocation = GetFileStorageLocation();

    public async Task SaveWeatherDataAsync(WeatherData data)
    {
        if (currentLocation == null)
        {
            _logger.LogError(Resources.PathIsNull);
            throw new ApplicationValidationException(Resources.PathIsNull);
        }

        var filePath = Path.Combine(currentLocation, $"{DateTime.Now.ToString(FileNameFormat)}.txt");
        _logger.LogInformation(string.Format(Resources.FileStoragePath, filePath));

        await WriteToFileWithRetriesAsync(filePath, data);
    }

    private static string GetFileStorageLocation()
    {
        var currentModule = (Assembly.GetEntryAssembly()?.Location)
            ?? throw new ApplicationValidationException(Resources.PathIsNull);
        var parentDirectory = (Directory.GetParent(currentModule)?.Parent?.Parent?.Parent?.Parent?.Parent?.FullName)
            ?? throw new ApplicationValidationException(Resources.PathIsNull);

        var storageDirectory = Path.Join(parentDirectory, "FileStorage");

        return storageDirectory;
    }

    private async Task WriteToFileWithRetriesAsync(
        string filePath, WeatherData data, int maxRetries = 5, int retryDelayMilliseconds = 500)
    {
        var serializedData = JsonConvert.SerializeObject(data);

        for (int retryNumber = 1; retryNumber <= maxRetries; retryNumber++)
        {
            try
            {
                await File.WriteAllTextAsync(filePath, serializedData);
                break; // Exit the loop if the write operation succeeds
            }
            catch (IOException)
            {
                _logger.LogWarning(string.Format(Resources.RetrySavingToFile, retryNumber));
                await Task.Delay(retryDelayMilliseconds);
            }
        }
    }
}
