import requests
import logging
from datetime import datetime
from typing import Optional, List
from models import WeatherResponse
import os

logger = logging.getLogger(__name__)

class WeatherService:
    """Base weather service interface"""
    
    async def get_weather_by_city(self, city: str, units: str = "metric") -> Optional[WeatherResponse]:
        raise NotImplementedError
    
    async def get_weather_by_coordinates(self, latitude: float, longitude: float, units: str = "metric") -> Optional[WeatherResponse]:
        raise NotImplementedError
    
    async def get_weather_for_cities(self, cities: List[str], units: str = "metric") -> List[WeatherResponse]:
        raise NotImplementedError


class OpenWeatherMapService(WeatherService):
    """OpenWeatherMap API implementation"""
    
    BASE_URL = "https://api.openweathermap.org/data/2.5/weather"
    
    def __init__(self):
        self.api_key = os.getenv("OPENWEATHERMAP_API_KEY") or os.getenv("OPENWEATHERMAP_APIKEY")
        if not self.api_key:
            logger.error("OpenWeatherMap API key is not configured")
    
    def get_weather_by_city(self, city: str, units: str = "metric") -> Optional[WeatherResponse]:
        """Get weather by city name"""
        try:
            if not self.api_key:
                logger.error("OpenWeatherMap API key is not configured")
                return None
            
            params = {
                "q": city,
                "appid": self.api_key,
                "units": units or "metric"
            }
            
            logger.info(f"Fetching weather for city: {city}")
            response = requests.get(self.BASE_URL, params=params, timeout=10)
            response.raise_for_status()
            
            data = response.json()
            return self._map_to_weather_response(data)
        
        except requests.exceptions.RequestException as ex:
            logger.error(f"Error fetching weather for city {city}: {ex}")
            return None
        except Exception as ex:
            logger.error(f"Unexpected error in get_weather_by_city: {ex}")
            return None
    
    def get_weather_by_coordinates(self, latitude: float, longitude: float, units: str = "metric") -> Optional[WeatherResponse]:
        """Get weather by coordinates"""
        try:
            if not self.api_key:
                logger.error("OpenWeatherMap API key is not configured")
                return None
            
            params = {
                "lat": latitude,
                "lon": longitude,
                "appid": self.api_key,
                "units": units or "metric"
            }
            
            logger.info(f"Fetching weather for coordinates: {latitude}, {longitude}")
            response = requests.get(self.BASE_URL, params=params, timeout=10)
            response.raise_for_status()
            
            data = response.json()
            return self._map_to_weather_response(data)
        
        except requests.exceptions.RequestException as ex:
            logger.error(f"Error fetching weather for coordinates {latitude}, {longitude}: {ex}")
            return None
        except Exception as ex:
            logger.error(f"Unexpected error in get_weather_by_coordinates: {ex}")
            return None
    
    def get_weather_for_cities(self, cities: List[str], units: str = "metric") -> List[WeatherResponse]:
        """Get weather for multiple cities"""
        results = []
        for city in cities:
            weather = self.get_weather_by_city(city, units)
            if weather:
                results.append(weather)
        return results
    
    @staticmethod
    def _map_to_weather_response(data: dict) -> Optional[WeatherResponse]:
        """Map OpenWeatherMap API response to WeatherResponse"""
        if not data:
            return None
        
        try:
            main = data.get("main", {})
            weather_list = data.get("weather", [])
            wind = data.get("wind", {})
            clouds = data.get("clouds", {})
            sys = data.get("sys", {})
            
            weather_desc = weather_list[0] if weather_list else {}
            
            return WeatherResponse(
                city=data.get("name"),
                country=sys.get("country"),
                temperature=main.get("temp", 0),
                feels_like=main.get("feels_like", 0),
                humidity=main.get("humidity", 0),
                pressure=main.get("pressure", 0),
                description=weather_desc.get("description", "N/A"),
                icon=weather_desc.get("icon"),
                wind_speed=wind.get("speed", 0),
                cloudiness=clouds.get("all", 0),
                sunrise=OpenWeatherMapService._unix_to_datetime(sys.get("sunrise", 0)),
                sunset=OpenWeatherMapService._unix_to_datetime(sys.get("sunset", 0)),
                last_updated=datetime.utcnow()
            )
        except (KeyError, ValueError, TypeError) as ex:
            logger.error(f"Error mapping weather response: {ex}")
            return None
    
    @staticmethod
    def _unix_to_datetime(unix_timestamp: int) -> Optional[datetime]:
        """Convert Unix timestamp to datetime"""
        if unix_timestamp:
            try:
                return datetime.fromtimestamp(unix_timestamp)
            except (ValueError, OSError):
                return None
        return None
