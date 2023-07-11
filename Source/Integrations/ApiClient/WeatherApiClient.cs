using Contracts.Configs;
using Contracts.Exceptions;
using MetaApp.Domain.ApiClient;
using MetaApp.Domain.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;

namespace MetaApp.Integrations.ApiClient;

public class WeatherApiClient : IWeatherApiClient
{
    private readonly ILogger<WeatherApiClient> _logger;
    private readonly HttpClient _httpClient;
    private readonly WeatherConfig _config;

    private bool _isAuthenticated = false;

    public WeatherApiClient(ILogger<WeatherApiClient> logger, IOptions<WeatherConfig> config)
    {
        _logger = logger;
        _config = config.Value;
        _httpClient = new HttpClient()
        {
            BaseAddress = new Uri(_config.WeatherApiUrl)
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
            throw new WeatherApiClientException(Resources.WeatherDataIsNull);
        }

        return weatherData;
    }

    private async Task<bool> UpdateAuthorizationToken()
    {
        _logger.LogInformation(Resources.StartAuthentication);

        var secret = new AuthorizationRequest(_config.WeatherApiUsername, _config.WeatherApiPassword);
        var content = new StringContent(JsonConvert.SerializeObject(secret), Encoding.UTF8, Constants.ApplicationJson);
        var response = await _httpClient.PostAsync(Constants.WeatherApiAuthorizationEndpoint, content);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError(Resources.BearerTokenIsNotReturned);
            // TODO: uncomment when API will work.
            //throw new WeatherApiClientException(Resources.BearerTokenIsNotReturned);

            return false;
        }

        var token = await response.Content.ReadFromJsonAsync<AuthorizationResponse>();

        if (token == null || token.Bearer == null)
        {
            _logger.LogError(Resources.BearerTokenIsNull);

            // TODO: uncomment when API will work.
            //throw new WeatherApiClientException(Resources.BearerTokenIsNull);

            return false;
        }

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(Constants.Bearer, token.Bearer);

        return response.IsSuccessStatusCode;
    }
}
