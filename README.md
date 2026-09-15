# Weather Dashboard - Python Flask Application

A modern, fully-featured weather dashboard built with Python and Flask that fetches real-time weather data from the OpenWeatherMap API.

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
- ✅ **REST API with JSON responses** - Built-in API documentation
- ✅ **Error Handling & Logging** - Comprehensive error handling with Python logging
- ✅ **CORS Support** - Ready for frontend integration
- ✅ **Health Check Endpoint** - Monitor API status

## 🛠️ Tech Stack

- **Framework**: Flask 3.0.0
- **Language**: Python 3.8+
- **HTTP Client**: requests
- **Weather Data**: OpenWeatherMap API
- **Environment**: python-dotenv

## 📋 Prerequisites

- Python 3.8 or later
- pip (Python package manager)
- OpenWeatherMap API Key (free tier available at https://openweathermap.org/api)

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

**Option A: Using .env file (Recommended)**

```bash
cp .env.example .env
# Edit .env and add your API key
OPENWEATHERMAP_API_KEY=your_api_key_here
FLASK_ENV=development
```

**Option B: Using Environment Variables**

```bash
# Linux/macOS
export OPENWEATHERMAP_API_KEY="your_api_key_here"

# Windows (Command Prompt)
set OPENWEATHERMAP_API_KEY=your_api_key_here

# Windows (PowerShell)
$env:OPENWEATHERMAP_API_KEY="your_api_key_here"
```

### 4. Create Virtual Environment

```bash
python -m venv venv

# On Windows
venv\Scripts\activate

# On Linux/macOS
source venv/bin/activate
```

### 5. Install Dependencies

```bash
pip install -r requirements.txt
```

### 6. Run the Application

```bash
python app.py
```

The API will start at `http://localhost:5000`

## 📚 API Endpoints

### Get Weather by City

```bash
GET /api/weather/city/{city}?units=metric
```

**Example:**
```bash
curl http://localhost:5000/api/weather/city/Kyiv?units=metric
```

**Response:**
```json
{
  "city": "Kyiv",
  "country": "UA",
  "temperature": 15.5,
  "feels_like": 14.2,
  "humidity": 65,
  "pressure": 1013,
  "description": "partly cloudy",
  "icon": "02d",
  "wind_speed": 3.5,
  "cloudiness": 40,
  "sunrise": "2024-01-15T07:30:00",
  "sunset": "2024-01-15T17:45:00",
  "last_updated": "2024-01-15T12:00:00Z"
}
```

### Get Weather by Coordinates

```bash
GET /api/weather/coordinates?latitude=50.4501&longitude=30.5234&units=metric
```

**Example:**
```bash
curl "http://localhost:5000/api/weather/coordinates?latitude=50.4501&longitude=30.5234&units=metric"
```

### Get Weather for Multiple Cities

```bash
GET /api/weather/cities?cities=Kyiv,London,NewYork&units=metric
```

**Example:**
```bash
curl "http://localhost:5000/api/weather/cities?cities=Kyiv,London,NewYork&units=metric"
```

### Health Check

```bash
GET /api/weather/health
```

## 📦 Project Structure

```
weather-dashboard-csharp/
├── app.py                    # Flask application and routes
├── services.py               # Weather service implementation
├── models.py                 # Data models
├── requirements.txt          # Python dependencies
├── .env.example              # Environment variables example
├── .gitignore                # Git ignore file
├── README.md                 # This file
└── LICENSE                   # MIT License
```

## 🔐 Security Notes

- Never commit your API key to version control
- Use environment variables for sensitive data in production
- Keep your API key private
- Consider using a secrets management service for production

## 📊 Temperature Units

- `metric` (default) - Celsius (°C)
- `imperial` - Fahrenheit (°F)
- `standard` - Kelvin (K)

## 🐛 Troubleshooting

### "API key not configured" error
- Ensure your API key is set in .env file or OPENWEATHERMAP_API_KEY environment variable
- Verify the API key is correct and active on OpenWeatherMap

### "City not found" error
- Check the spelling of the city name
- Some city names may require country codes (e.g., "London,GB")

### ModuleNotFoundError
- Make sure you've activated the virtual environment
- Run `pip install -r requirements.txt` to install dependencies

## 🤝 Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## 📝 License

This project is licensed under the MIT License - see the LICENSE file for details.

## 📞 Support

For issues and questions, please open an issue on GitHub.

## 🔗 Useful Links

- [OpenWeatherMap API Documentation](https://openweathermap.org/api)
- [Flask Documentation](https://flask.palletsprojects.com/)
- [Python Documentation](https://docs.python.org/)
- [requests Library Documentation](https://requests.readthedocs.io/)

---

**Made with ❤️ by Marian Melnyk**
