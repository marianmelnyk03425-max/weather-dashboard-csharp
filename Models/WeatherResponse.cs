namespace WeatherDashboard.Models;

public class WeatherResponse
{
    public string? City { get; set; }
    public string? Country { get; set; }
    public double Temperature { get; set; }
    public double FeelsLike { get; set; }
    public int Humidity { get; set; }
    public int Pressure { get; set; }
    public string? Description { get; set; }
    public string? Icon { get; set; }
    public double WindSpeed { get; set; }
    public double Cloudiness { get; set; }
    public DateTime Sunrise { get; set; }
    public DateTime Sunset { get; set; }
    public DateTime LastUpdated { get; set; }
}