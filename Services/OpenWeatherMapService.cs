using System.Text.Json;
using WeatherDashboard.Models;
using Microsoft.Extensions.Logging;

namespace WeatherDashboard.Services;

public class OpenWeatherMapService : IWeatherService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<OpenWeatherMapService> _logger;
    private readonly IConfiguration _configuration;
    private readonly string? _apiKey;

    public OpenWeatherMapService(HttpClient httpClient, ILogger<OpenWeatherMapService> logger, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _logger = logger;
        _configuration = configuration;
        _apiKey = _configuration["OpenWeatherMap:ApiKey"] ?? Environment.GetEnvironmentVariable("OPENWEATHERMAP_API_KEY");
    }

    public async Task<WeatherResponse?> GetWeatherByCityAsync(string city, string? units = "metric")
    {
        try
        {
            if (string.IsNullOrWhiteSpace(_apiKey))
            {
                _logger.LogError("OpenWeatherMap API key is not configured");
                return null;
            }

            var url = $"weather?q={city}&appid={_apiKey}&units={units ?? "metric"}";
            _logger.LogInformation("Fetching weather for city: {City}", city);

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var weatherData = JsonSerializer.Deserialize<OpenWeatherMapDto>(content, options);

            return MapToWeatherResponse(weatherData);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error fetching weather for city: {City}", city);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in GetWeatherByCityAsync");
            return null;
        }
    }

    public async Task<WeatherResponse?> GetWeatherByCoordinatesAsync(double latitude, double longitude, string? units = "metric")
    {
        try
        {
            if (string.IsNullOrWhiteSpace(_apiKey))
            {
                _logger.LogError("OpenWeatherMap API key is not configured");
                return null;
            }

            var url = $"weather?lat={latitude}&lon={longitude}&appid={_apiKey}&units={units ?? "metric"}";
            _logger.LogInformation("Fetching weather for coordinates: {Latitude}, {Longitude}", latitude, longitude);

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var weatherData = JsonSerializer.Deserialize<OpenWeatherMapDto>(content, options);

            return MapToWeatherResponse(weatherData);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error fetching weather for coordinates: {Latitude}, {Longitude}", latitude, longitude);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in GetWeatherByCoordinatesAsync");
            return null;
        }
    }

    public async Task<List<WeatherResponse>> GetWeatherForCitiesAsync(List<string> cities, string? units = "metric")
    {
        var results = new List<WeatherResponse>();

        foreach (var city in cities)
        {
            var weather = await GetWeatherByCityAsync(city, units);
            if (weather != null)
            {
                results.Add(weather);
            }
        }

        return results;
    }

    private static WeatherResponse? MapToWeatherResponse(OpenWeatherMapDto? dto)
    {
        if (dto == null)
            return null;

        return new WeatherResponse
        {
            City = dto.Name,
            Country = dto.Sys?.Country,
            Temperature = dto.Main?.Temp ?? 0,
            FeelsLike = dto.Main?.Feels_like ?? 0,
            Humidity = dto.Main?.Humidity ?? 0,
            Pressure = dto.Main?.Pressure ?? 0,
            Description = dto.Weather?.FirstOrDefault()?.Description ?? "N/A",
            Icon = dto.Weather?.FirstOrDefault()?.Icon,
            WindSpeed = dto.Wind?.Speed ?? 0,
            Cloudiness = dto.Clouds?.All ?? 0,
            Sunrise = UnixTimeStampToDateTime(dto.Sys?.Sunrise ?? 0),
            Sunset = UnixTimeStampToDateTime(dto.Sys?.Sunset ?? 0),
            LastUpdated = DateTime.UtcNow
        };
    }

    private static DateTime UnixTimeStampToDateTime(long unixTimeStamp)
    {
        var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dateTime = dateTime.AddSeconds(unixTimeStamp).ToLocalTime();
        return dateTime;
    }
}