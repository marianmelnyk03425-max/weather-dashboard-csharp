using Microsoft.AspNetCore.Mvc;
using WeatherDashboard.Models;
using WeatherDashboard.Services;

namespace WeatherDashboard.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WeatherController : ControllerBase
{
    private readonly IWeatherService _weatherService;
    private readonly ILogger<WeatherController> _logger;

    public WeatherController(IWeatherService weatherService, ILogger<WeatherController> logger)
    {
        _weatherService = weatherService;
        _logger = logger;
    }

    /// <summary>
    /// Get weather by city name
    /// </summary>
    /// <param name="city">City name (e.g., "Kyiv", "London", "New York")</param>
    /// <param name="units">Temperature units: metric (°C), imperial (°F), standard (K)</param>
    /// <returns>Weather information for the specified city</returns>
    [HttpGet("city/{city}")]
    [Produces(typeof(WeatherResponse), "application/json")]
    public async Task<ActionResult<WeatherResponse>> GetWeatherByCity(string city, [FromQuery] string? units = "metric")
    {
        if (string.IsNullOrWhiteSpace(city))
        {
            return BadRequest("City name cannot be empty");
        }

        var weather = await _weatherService.GetWeatherByCityAsync(city, units);

        if (weather == null)
        {
            return NotFound($"Weather data not found for city: {city}");
        }

        return Ok(weather);
    }

    /// <summary>
    /// Get weather by geographical coordinates
    /// </summary>
    /// <param name="latitude">Latitude</param>
    /// <param name="longitude">Longitude</param>
    /// <param name="units">Temperature units: metric (°C), imperial (°F), standard (K)</param>
    /// <returns>Weather information for the specified location</returns>
    [HttpGet("coordinates")]
    [Produces(typeof(WeatherResponse), "application/json")]
    public async Task<ActionResult<WeatherResponse>> GetWeatherByCoordinates(
        [FromQuery] double latitude,
        [FromQuery] double longitude,
        [FromQuery] string? units = "metric")
    {
        if (latitude < -90 || latitude > 90 || longitude < -180 || longitude > 180)
        {
            return BadRequest("Invalid coordinates. Latitude must be between -90 and 90, Longitude between -180 and 180");
        }

        var weather = await _weatherService.GetWeatherByCoordinatesAsync(latitude, longitude, units);

        if (weather == null)
        {
            return NotFound("Weather data not found for the specified coordinates");
        }

        return Ok(weather);
    }

    /// <summary>
    /// Get weather for multiple cities
    /// </summary>
    /// <param name="cities">Comma-separated list of city names</param>
    /// <param name="units">Temperature units: metric (°C), imperial (°F), standard (K)</param>
    /// <returns>Weather information for all specified cities</returns>
    [HttpGet("cities")]
    [Produces(typeof(List<WeatherResponse>), "application/json")]
    public async Task<ActionResult<List<WeatherResponse>>> GetWeatherForCities(
        [FromQuery] string cities,
        [FromQuery] string? units = "metric")
    {
        if (string.IsNullOrWhiteSpace(cities))
        {
            return BadRequest("Cities parameter cannot be empty");
        }

        var cityList = cities.Split(',', StringSplitOptions.RemoveEmptyEntries)
                              .Select(c => c.Trim())
                              .ToList();

        if (!cityList.Any())
        {
            return BadRequest("At least one city name is required");
        }

        var weatherList = await _weatherService.GetWeatherForCitiesAsync(cityList, units);

        if (!weatherList.Any())
        {
            return NotFound("Weather data not found for any of the specified cities");
        }

        return Ok(weatherList);
    }

    /// <summary>
    /// Health check endpoint
    /// </summary>
    [HttpGet("health")]
    public IActionResult Health()
    {
        return Ok(new { status = "healthy", timestamp = DateTime.UtcNow });
    }
}
