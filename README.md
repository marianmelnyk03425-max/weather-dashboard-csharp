# Weather Dashboard - C# .NET Application

A modern, fully-featured weather dashboard built with C# and ASP.NET Core that fetches real-time weather data from the OpenWeatherMap API.

## 🌟 Features

- ✅ **Real-time Weather Data** - Fetch current weather from OpenWeatherMap API
- ✅ **Multiple Query Options**:
  - Get weather by city name
  - Get weather by GPS coordinates (latitude/longitude)
  - Get weather for multiple cities at once
- ✅ **Temperature Units** - Support for metric (°C), imperial (°F), and standard (K) units
- ✅ **Comprehensive Weather Info**:
  - Temperature, "feels like" temperature
  - Humidity, pressure, wind speed
  - Cloud coverage
  - Sunrise/sunset times
  - Weather description and icons
- ✅ **REST API with Swagger Documentation** - Built-in API documentation with Swagger UI
- ✅ **Error Handling & Logging** - Comprehensive error handling with Serilog logging
- ✅ **CORS Support** - Ready for frontend integration
- ✅ **Health Check Endpoint** - Monitor API status

## 🛠️ Tech Stack

- **Framework**: ASP.NET Core 8.0
- **Language**: C# 12
- **API Documentation**: Swagger/OpenAPI
- **Logging**: Serilog
- **Weather Data**: OpenWeatherMap API
- **HTTP Client**: HttpClientFactory

## 📋 Prerequisites

- .NET 8.0 SDK or later
- OpenWeatherMap API Key (free tier available at https://openweathermap.org/api)
- Visual Studio 2022 or VS Code

## 🚀 Quick Start

### 1. Clone the Repository

```bash
git clone https://github.com/marianmelnyk03425-max/weather-dashboard-csharp.git
cd weather-dashboard-csharp
```

### 2. Get Your API Key

1. Visit [OpenWeatherMap](https://openweathermap.org/api)
2. Sign up for a free account
3. Copy your API key from the API keys section

### 3. Configure API Key

**Option A: Using appsettings.json**

```json
{
  "OpenWeatherMap": {
    "ApiKey": "YOUR_API_KEY_HERE"
  }
}
```

**Option B: Using Environment Variables (Recommended)**

```bash
# Linux/macOS
export OPENWEATHERMAP_API_KEY="your_api_key_here"

# Windows (Command Prompt)
set OPENWEATHERMAP_API_KEY=your_api_key_here

# Windows (PowerShell)
$env:OPENWEATHERMAP_API_KEY="your_api_key_here"
```

### 4. Install Dependencies

```bash
dotnet restore
```

### 5. Run the Application

```bash
dotnet run
```

The API will start at `https://localhost:5001` (or `http://localhost:5000`)

### 6. Access Swagger Documentation

Visit: `https://localhost:5001/swagger`

## 📚 API Endpoints

### Get Weather by City

```bash
GET /api/weather/city/{city}?units=metric
```

**Example:**
```bash
curl https://localhost:5001/api/weather/city/Kyiv?units=metric
```

**Response:**
```json
{
  "city": "Kyiv",
  "country": "UA",
  "temperature": 15.5,
  "feelsLike": 14.2,
  "humidity": 65,
  "pressure": 1013,
  "description": "partly cloudy",
  "icon": "02d",
  "windSpeed": 3.5,
  "cloudiness": 40,
  "sunrise": "2024-01-15T07:30:00",
  "sunset": "2024-01-15T17:45:00",
  "lastUpdated": "2024-01-15T12:00:00Z"
}
```

### Get Weather by Coordinates

```bash
GET /api/weather/coordinates?latitude=50.4501&longitude=30.5234&units=metric
```

**Example:**
```bash
curl "https://localhost:5001/api/weather/coordinates?latitude=50.4501&longitude=30.5234&units=metric"
```

### Get Weather for Multiple Cities

```bash
GET /api/weather/cities?cities=Kyiv,London,NewYork&units=metric
```

**Example:**
```bash
curl "https://localhost:5001/api/weather/cities?cities=Kyiv,London,NewYork&units=metric"
```

### Health Check

```bash
GET /api/weather/health
```

## 📦 Project Structure

```
weather-dashboard-csharp/
├── Controllers/
│   └── WeatherController.cs      # API endpoints
├── Services/
│   ├── IWeatherService.cs        # Service interface
│   └── OpenWeatherMapService.cs  # OpenWeatherMap implementation
├── Models/
│   ├── WeatherResponse.cs        # API response model
│   └── OpenWeatherMapDto.cs      # OpenWeatherMap API models
├── Program.cs                    # Application startup
├── appsettings.json              # Configuration
├── appsettings.Development.json  # Development configuration
├── .env.example                  # Environment variables example
└── WeatherDashboard.csproj       # Project file
```

## 🔐 Security Notes

- Never commit your API key to version control
- Use environment variables for sensitive data in production
- Keep your API key private
- Consider using Azure Key Vault or similar services for production

## 📊 Temperature Units

- `metric` (default) - Celsius (°C)
- `imperial` - Fahrenheit (°F)
- `standard` - Kelvin (K)

## 🐛 Troubleshooting

### "API key not configured" error
- Ensure your API key is set in appsettings.json or OPENWEATHERMAP_API_KEY environment variable
- Verify the API key is correct and active on OpenWeatherMap

### "City not found" error
- Check the spelling of the city name
- Some city names may require country codes (e.g., "London,GB")

### HTTPS certificate error in development
- Run: `dotnet dev-certs https --trust`

## 🤝 Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## 📝 License

This project is licensed under the MIT License - see the LICENSE file for details.

## 📞 Support

For issues and questions, please open an issue on GitHub.

## 🔗 Useful Links

- [OpenWeatherMap API Documentation](https://openweathermap.org/api)
- [ASP.NET Core Documentation](https://docs.microsoft.com/dotnet/core/)
- [Swagger/OpenAPI Documentation](https://swagger.io/)
- [Serilog Documentation](https://serilog.net/)

---

**Made with ❤️ by Marian Melnyk**
