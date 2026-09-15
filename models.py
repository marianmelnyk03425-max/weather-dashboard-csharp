from dataclasses import dataclass, asdict
from datetime import datetime
from typing import Optional, List, Dict, Any

@dataclass
class WeatherResponse:
    """Weather response model"""
    city: Optional[str] = None
    country: Optional[str] = None
    temperature: float = 0.0
    feels_like: float = 0.0
    humidity: int = 0
    pressure: int = 0
    description: Optional[str] = None
    icon: Optional[str] = None
    wind_speed: float = 0.0
    cloudiness: float = 0.0
    sunrise: Optional[datetime] = None
    sunset: Optional[datetime] = None
    last_updated: Optional[datetime] = None

    def to_dict(self) -> Dict[str, Any]:
        """Convert to dictionary for JSON serialization"""
        data = asdict(self)
        if self.sunrise:
            data['sunrise'] = self.sunrise.isoformat()
        if self.sunset:
            data['sunset'] = self.sunset.isoformat()
        if self.last_updated:
            data['last_updated'] = self.last_updated.isoformat()
        return data
