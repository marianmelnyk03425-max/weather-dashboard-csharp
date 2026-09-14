using WeatherDashboard.Models;

namespace WeatherDashboard.Services;

public interface IWeatherService
{
    /// <summary>
    /// Get weather by city name
    /// </summary>
    Task<WeatherResponse?> GetWeatherByCityAsync(string city, string? units = "metric");

    /// <summary>
    /// Get weather by coordinates (latitude, longitude)
    /// </summary>
    Task<WeatherResponse?> GetWeatherByCoordinatesAsync(double latitude, double longitude, string? units = "metric");

    /// <summary>
    /// Get weather for multiple cities
    /// </summary>
    Task<List<WeatherResponse>> GetWeatherForCitiesAsync(List<string> cities, string? units = "metric");
}