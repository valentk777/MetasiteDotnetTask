using MetaApp.Domain.Models;
using MetaApp.Domain.Storages;
using Newtonsoft.Json;
using System.Reflection;

namespace MetaApp.Integrations.FileStorage
{
    public class FileStorage : IMetaAppStorage
    {
        private string? currentLocation = Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location);

        public async Task SaveWeatherData(WeatherData data)
        {
            var filePath = Path.Combine(currentLocation, $"{Guid.NewGuid()}.txt");
            await File.WriteAllTextAsync(filePath, JsonConvert.SerializeObject(data));
        }
    }
}
