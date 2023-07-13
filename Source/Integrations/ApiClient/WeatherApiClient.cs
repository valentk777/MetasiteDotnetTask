using Contracts.Configs;
using Contracts.Exceptions;
using MetaApp.Domain.ApiClient;
using MetaApp.Domain.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Authentication;
using System.Text;

namespace MetaApp.Integrations.ApiClient;

public class WeatherApiClient : IWeatherApiClient, IDisposable
{
    private readonly ILogger<WeatherApiClient> _logger;
    private readonly HttpClient _httpClient;
    private readonly WeatherConfig _config;

    private readonly SemaphoreSlim _authLock = new(1, 1);
    private BearerToken _token = new BearerToken(null);

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
        var url = _httpClient.BaseAddress + Constants.WeatherApiGetWeatherEndpoint + city;

        await UpdateAuthorizationTokenAsync();

        var response = await _httpClient.GetAsync(url);

        if (!response.IsSuccessStatusCode)
        {
            throw new WeatherApiClientException(string.Format(Resources.RequestIsNotSuccessful, url, response.StatusCode));
        }

        var responseContent = await response.Content.ReadAsStringAsync();
        var weatherData = JsonConvert.DeserializeObject<WeatherData>(responseContent);

        if (weatherData == null)
        {
            throw new WeatherApiClientException(Resources.WeatherDataIsNull);
        }

        return weatherData;
    }

    public void Dispose()
    {
        _authLock?.Dispose();
        _httpClient?.Dispose();
    }

    private async Task<BearerToken> UpdateAuthorizationTokenAsync()
    {
        _logger.LogInformation(Resources.StartAuthentication);

        try
        {
            await _authLock.WaitAsync();

            if (string.IsNullOrEmpty(_token?.Bearer))
            {
                _token = await RetrieveAccessTokenAsync();
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(Constants.Bearer, _token.Bearer);

            return _token;
        }
        finally
        {
            _authLock.Release();
        }
    }

    private async Task<BearerToken> RetrieveAccessTokenAsync()
    {
        var secret = new AuthorizationRequest(_config.WeatherApiUsername, _config.WeatherApiPassword);
        var content = new StringContent(JsonConvert.SerializeObject(secret), Encoding.UTF8, Constants.ApplicationJson);
        var response = await _httpClient.PostAsync(Constants.WeatherApiAuthorizationEndpoint, content);

        return await GetValidTokenAsync(response);
    }

    private async Task<BearerToken> GetValidTokenAsync(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError(Resources.BearerTokenIsNotReturned);
            throw new AuthenticationException(Resources.BearerTokenIsNotReturned);
        }

        var token = await response.Content.ReadFromJsonAsync<BearerToken>();

        if (token == null || token.Bearer == null)
        {
            _logger.LogError(Resources.BearerTokenIsNull);
            throw new AuthenticationException(Resources.BearerTokenIsNull);
        }

        return token;
    }
}
