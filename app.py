from flask import Flask, request, jsonify
from services import OpenWeatherMapService
import logging
from logging.handlers import StreamHandler
import os
from dotenv import load_dotenv

# Load environment variables
load_dotenv()

# Configure logging
logging.basicConfig(
    level=logging.INFO,
    format='%(asctime)s - %(name)s - %(levelname)s - %(message)s'
)
logger = logging.getLogger(__name__)

app = Flask(__name__)

# Configure CORS
from flask_cors import CORS
CORS(app)

# Initialize weather service
weather_service = OpenWeatherMapService()


@app.route('/api/weather/city/<city>', methods=['GET'])
def get_weather_by_city(city):
    """
    Get weather by city name
    Query parameters:
        - units: metric (default), imperial, standard
    """
    if not city or not city.strip():
        return jsonify({"error": "City name cannot be empty"}), 400
    
    units = request.args.get('units', 'metric')
    weather = weather_service.get_weather_by_city(city, units)
    
    if not weather:
        return jsonify({"error": f"Weather data not found for city: {city}"}), 404
    
    return jsonify(weather.to_dict()), 200


@app.route('/api/weather/coordinates', methods=['GET'])
def get_weather_by_coordinates():
    """
    Get weather by geographical coordinates
    Query parameters:
        - latitude: required, -90 to 90
        - longitude: required, -180 to 180
        - units: metric (default), imperial, standard
    """
    try:
        latitude = float(request.args.get('latitude'))
        longitude = float(request.args.get('longitude'))
    except (TypeError, ValueError):
        return jsonify({"error": "Invalid latitude or longitude"}), 400
    
    if latitude < -90 or latitude > 90 or longitude < -180 or longitude > 180:
        return jsonify({
            "error": "Invalid coordinates. Latitude must be between -90 and 90, Longitude between -180 and 180"
        }), 400
    
    units = request.args.get('units', 'metric')
    weather = weather_service.get_weather_by_coordinates(latitude, longitude, units)
    
    if not weather:
        return jsonify({"error": "Weather data not found for the specified coordinates"}), 404
    
    return jsonify(weather.to_dict()), 200


@app.route('/api/weather/cities', methods=['GET'])
def get_weather_for_cities():
    """
    Get weather for multiple cities
    Query parameters:
        - cities: comma-separated list of city names (required)
        - units: metric (default), imperial, standard
    """
    cities_param = request.args.get('cities', '')
    
    if not cities_param or not cities_param.strip():
        return jsonify({"error": "Cities parameter cannot be empty"}), 400
    
    city_list = [c.strip() for c in cities_param.split(',') if c.strip()]
    
    if not city_list:
        return jsonify({"error": "At least one city name is required"}), 400
    
    units = request.args.get('units', 'metric')
    weather_list = weather_service.get_weather_for_cities(city_list, units)
    
    if not weather_list:
        return jsonify({"error": "Weather data not found for any of the specified cities"}), 404
    
    return jsonify([w.to_dict() for w in weather_list]), 200


@app.route('/api/weather/health', methods=['GET'])
def health_check():
    """Health check endpoint"""
    from datetime import datetime
    return jsonify({
        "status": "healthy",
        "timestamp": datetime.utcnow().isoformat()
    }), 200


@app.errorhandler(404)
def not_found(error):
    return jsonify({"error": "Endpoint not found"}), 404


@app.errorhandler(500)
def internal_error(error):
    logger.error(f"Internal server error: {error}")
    return jsonify({"error": "Internal server error"}), 500


if __name__ == '__main__':
    logger.info("Starting Weather Dashboard API...")
    debug_mode = os.getenv('FLASK_ENV') == 'development'
    app.run(host='0.0.0.0', port=5000, debug=debug_mode)
