namespace Contracts.Exceptions;

public class WeatherApiClientException : Exception
{
    public WeatherApiClientException(string message) : base(message)
    {
    }

    public WeatherApiClientException(Exception innerException) : base(string.Empty, innerException)
    {
    }

    public WeatherApiClientException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
