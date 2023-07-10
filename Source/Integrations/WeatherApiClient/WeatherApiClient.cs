using MetaApp.Domain.ApiClients;
using MetaApp.Domain.Models;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;

namespace MetaApp.Integrations.WeatherApiClient;

public class WeatherApiClient : IWeatherApiClient
{
    private HttpClient _httpClient;
    private bool _isAuthenticated = false;

    public WeatherApiClient()
    {
        _httpClient = new HttpClient()
        {
            // TODO: to constants.
            BaseAddress = new Uri("https://weather-api.m3tasite.net/")
        };
    }

    public async Task<WeatherData> GetWeatherDataAsync(string city)
    {
        if (!_isAuthenticated)
        {
            _isAuthenticated = await UpdateAuthorizationToken();
        }

        var url = _httpClient.BaseAddress + city;

        var response = await _httpClient.GetAsync(url);

        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized
            // TODO: remove. this is because API is not working.
            || response.StatusCode == System.Net.HttpStatusCode.PermanentRedirect)
        {
            _isAuthenticated = await UpdateAuthorizationToken();

            if (!_isAuthenticated)
            {
                // TODO: change to custome expection.
                throw new Exception("Auth");
            }
        }

        response = await _httpClient.GetAsync(url);

        if (!response.IsSuccessStatusCode)
        {
            // TODO: change to exception. Now API is not working.
            return new WeatherData()
            {
                City = city,
                Temperature = "5.1",
                Description = "Test"
            };
        }

        var responseContent = await response.Content.ReadAsStringAsync();
        var weatherData = JsonConvert.DeserializeObject<WeatherData>(responseContent);

        if (weatherData == null)
        {
            // TODO: change to custome expection.
            throw new Exception("Auth");
        }

        return weatherData;
    }

    private async Task<bool> UpdateAuthorizationToken()
    {
        // TODO: to constants.
        var secret = new AuthorizationRequest("meta", "site");
        // TODO: to constants.
        var content = new StringContent(JsonConvert.SerializeObject(secret), Encoding.UTF8, "application/json");
        // TODO: to constants.
        var response = await _httpClient.PostAsync("/api/authorize", content);

        if (!response.IsSuccessStatusCode)
        {
            return false;
        }

        var token = await response.Content.ReadFromJsonAsync<AuthorizationResponse>();

        if (token == null || token.Bearer == null)
        {
            return false;
        }

        // TODO: to constants.
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Bearer);

        return response.IsSuccessStatusCode;
    }
}
