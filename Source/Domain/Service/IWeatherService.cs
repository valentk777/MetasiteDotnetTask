namespace MetaApp.Domain.Service
{
    public interface IWeatherService
    {
        void StartFetchingWeatherData(List<string> cities);
    }
}