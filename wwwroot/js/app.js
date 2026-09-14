// JARVIS Weather Dashboard - Main Application

class WeatherDashboard {
    constructor() {
        this.currentUnit = 'metric';
        this.currentCity = 'Kyiv';
        this.isDarkTheme = true;
        this.soundEnabled = true;
        this.weatherData = null;
        this.init();
    }

    init() {
        this.setupEventListeners();
        this.loadPreferences();
        this.fetchWeather(this.currentCity);
        this.startClock();
    }

    setupEventListeners() {
        // Search
        document.getElementById('searchBtn').addEventListener('click', () => {
            const city = document.getElementById('cityInput').value;
            if (city) this.fetchWeather(city);
        });

        document.getElementById('cityInput').addEventListener('keypress', (e) => {
            if (e.key === 'Enter') {
                const city = document.getElementById('cityInput').value;
                if (city) this.fetchWeather(city);
            }
        });

        // City items
        document.querySelectorAll('.city-item').forEach(item => {
            item.addEventListener('click', () => {
                const city = item.dataset.city;
                this.fetchWeather(city);
                document.querySelectorAll('.city-item').forEach(i => i.classList.remove('active'));
                item.classList.add('active');
            });
        });

        // Temperature units
        document.querySelectorAll('[data-unit]').forEach(btn => {
            btn.addEventListener('click', () => {
                document.querySelectorAll('[data-unit]').forEach(b => b.classList.remove('active'));
                btn.classList.add('active');
                this.currentUnit = btn.dataset.unit;
                this.fetchWeather(this.currentCity);
                localStorage.setItem('unit', this.currentUnit);
            });
        });

        // Theme toggle
        document.getElementById('themeBtn').addEventListener('click', () => {
            this.isDarkTheme = !this.isDarkTheme;
            document.body.classList.toggle('light-theme');
            document.getElementById('themeBtn').textContent = this.isDarkTheme ? '🌙 Темна режим' : '☀️ Світла режим';
            localStorage.setItem('theme', this.isDarkTheme ? 'dark' : 'light');
        });

        // Sound toggle
        document.getElementById('soundBtn').addEventListener('click', () => {
            this.soundEnabled = !this.soundEnabled;
            document.getElementById('soundBtn').textContent = this.soundEnabled ? '🔊 Увімкнено' : '🔇 Вимкнено';
            localStorage.setItem('sound', this.soundEnabled);
        });
    }

    async fetchWeather(city) {
        try {
            const weatherDisplay = document.getElementById('weatherDisplay');
            weatherDisplay.innerHTML = '<div class="loading-indicator"><div class="pulse"></div><p>Синхронізація з системою...</p></div>';

            const url = `/api/weather/city/${city}?units=${this.currentUnit}`;
            const response = await fetch(url);

            if (!response.ok) {
                throw new Error('City not found');
            }

            this.weatherData = await response.json();
            this.currentCity = city;
            this.displayWeather();
            this.updateStatus('🟢 ОНЛАЙН', 'var(--accent-color)');

            // Optional: Get voice announcement
            if (this.soundEnabled) {
                this.getWeatherAnnouncement(city);
            }
        } catch (error) {
            console.error('Error fetching weather:', error);
            document.getElementById('weatherDisplay').innerHTML = `
                <div style="text-align: center; color: var(--danger-color);">
                    <p style="font-size: 1.3em; text-shadow: 0 0 10px currentColor;">❌ Помилка!</p>
                    <p>${error.message}</p>
                    <p style="font-size: 0.9em; margin-top: 10px;">Спробуйте іншу назву міста</p>
                </div>
            `;
            this.updateStatus('🔴 ПОМИЛКА', 'var(--danger-color)');
        }
    }

    displayWeather() {
        if (!this.weatherData) return;

        const w = this.weatherData;
        const iconMap = {
            'Clear': '☀️',
            'Clouds': '☁️',
            'Rain': '🌧️',
            'Drizzle': '🌦️',
            'Thunderstorm': '⚡',
            'Snow': '❄️',
            'Mist': '🌫️',
            'Smoke': '💨',
            'Haze': '🌫️',
            'Dust': '🌪️',
            'Fog': '🌫️',
            'Sand': '🏜️',
            'Ash': '🌋',
            'Squall': '🌪️',
            'Tornado': '🌪️'
        };

        const weatherType = w.description.split(' ')[0];
        const icon = iconMap[weatherType] || '🌍';

        const unitSymbol = this.currentUnit === 'metric' ? '°C' : this.currentUnit === 'imperial' ? '°F' : 'K';

        const html = `
            <div class="weather-card animate-in">
                <div class="weather-city">${w.city}, ${w.country}</div>
                <div class="weather-icon">${icon}</div>
                <div class="weather-temp">${Math.round(w.temperature)}${unitSymbol}</div>
                <div class="weather-description">${w.description}</div>
                
                <div class="weather-details">
                    <div class="weather-detail-item">
                        <div class="weather-detail-label">Відчувається як</div>
                        <div class="weather-detail-value">${Math.round(w.feelsLike)}${unitSymbol}</div>
                    </div>
                    <div class="weather-detail-item">
                        <div class="weather-detail-label">Вологість</div>
                        <div class="weather-detail-value">${w.humidity}%</div>
                    </div>
                    <div class="weather-detail-item">
                        <div class="weather-detail-label">Вітер</div>
                        <div class="weather-detail-value">${w.windSpeed} м/с</div>
                    </div>
                    <div class="weather-detail-item">
                        <div class="weather-detail-label">Тиск</div>
                        <div class="weather-detail-value">${w.pressure} мБар</div>
                    </div>
                    <div class="weather-detail-item">
                        <div class="weather-detail-label">Хмари</div>
                        <div class="weather-detail-value">${w.cloudiness}%</div>
                    </div>
                    <div class="weather-detail-item">
                        <div class="weather-detail-label">Схід сонця</div>
                        <div class="weather-detail-value">${new Date(w.sunrise).toLocaleTimeString('uk-UA', {hour: '2-digit', minute: '2-digit'})}</div>
                    </div>
                    <div class="weather-detail-item">
                        <div class="weather-detail-label">Захід сонця</div>
                        <div class="weather-detail-value">${new Date(w.sunset).toLocaleTimeString('uk-UA', {hour: '2-digit', minute: '2-digit'})}</div>
                    </div>
                    <div class="weather-detail-item">
                        <div class="weather-detail-label">Оновлено</div>
                        <div class="weather-detail-value">${new Date(w.lastUpdated).toLocaleTimeString('uk-UA', {hour: '2-digit', minute: '2-digit'})}</div>
                    </div>
                </div>
            </div>
        `;

        document.getElementById('weatherDisplay').innerHTML = html;
    }

    async getWeatherAnnouncement(city) {
        try {
            const response = await fetch(`/api/voice/weather/${city}`);
            if (response.ok) {
                const audioBlob = await response.blob();
                const audioUrl = URL.createObjectURL(audioBlob);
                const audio = document.getElementById('jarvisAudio');
                audio.src = audioUrl;
                audio.play();
            }
        } catch (error) {
            console.log('Could not get voice announcement:', error);
        }
    }

    searchWeather(city) {
        this.fetchWeather(city);
    }

    updateStatus(text, color) {
        const statusText = document.getElementById('statusText');
        statusText.textContent = text;
        statusText.style.color = color;
    }

    startClock() {
        const updateTime = () => {
            const now = new Date();
            document.getElementById('lastUpdate').textContent = now.toLocaleTimeString('uk-UA', {
                hour: '2-digit',
                minute: '2-digit',
                second: '2-digit'
            });
        };
        updateTime();
        setInterval(updateTime, 1000);
    }

    loadPreferences() {
        const theme = localStorage.getItem('theme');
        const unit = localStorage.getItem('unit');
        const sound = localStorage.getItem('sound');

        if (theme === 'light') {
            this.isDarkTheme = false;
            document.body.classList.add('light-theme');
            document.getElementById('themeBtn').textContent = '☀️ Світла режим';
        }

        if (unit) {
            this.currentUnit = unit;
            document.querySelector(`[data-unit="${unit}"]`).classList.add('active');
        }

        if (sound === 'false') {
            this.soundEnabled = false;
            document.getElementById('soundBtn').textContent = '🔇 Вимкнено';
        }
    }
}

// Initialize App
if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', () => {
        window.app = new WeatherDashboard();
    });
} else {
    window.app = new WeatherDashboard();
}
