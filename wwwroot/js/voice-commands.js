// JARVIS Voice Commands System
// Handles speech recognition and JARVIS responses

class VoiceCommandsSystem {
    constructor() {
        this.isListening = false;
        this.recognition = null;
        this.setupSpeechRecognition();
        this.voiceBtn = document.getElementById('voiceBtn');
        this.voiceStatus = document.getElementById('voiceStatus');
    }

    setupSpeechRecognition() {
        const SpeechRecognition = window.SpeechRecognition || window.webkitSpeechRecognition;
        if (!SpeechRecognition) {
            console.warn('Speech Recognition not supported');
            this.voiceStatus.textContent = '❌ Не підтримується браузером';
            return;
        }

        this.recognition = new SpeechRecognition();
        this.recognition.lang = 'uk-UA';
        this.recognition.continuous = false;
        this.recognition.interimResults = false;

        this.recognition.onstart = () => {
            this.isListening = true;
            this.voiceBtn.style.boxShadow = '0 0 20px rgba(0, 255, 136, 0.5)';
            this.voiceStatus.textContent = '🎤 Слухаю...';
            this.voiceStatus.style.color = 'var(--accent-color)';
        };

        this.recognition.onresult = (event) => {
            let transcript = '';
            for (let i = event.resultIndex; i < event.results.length; i++) {
                transcript += event.results[i][0].transcript;
            }
            console.log('Recognized:', transcript);
            this.processCommand(transcript.toLowerCase());
        };

        this.recognition.onerror = (event) => {
            console.error('Speech recognition error:', event.error);
            this.voiceStatus.textContent = `❌ Помилка: ${event.error}`;
            this.voiceStatus.style.color = 'var(--danger-color)';
        };

        this.recognition.onend = () => {
            this.isListening = false;
            this.voiceBtn.style.boxShadow = '';
            this.voiceStatus.textContent = '✓ Готово до команд';
            this.voiceStatus.style.color = 'var(--accent-color)';
        };
    }

    startListening() {
        if (this.recognition && !this.isListening) {
            this.recognition.start();
        }
    }

    processCommand(command) {
        // Weather command patterns
        if (command.includes('погода') || command.includes('weather')) {
            // Extract city name if provided
            const cityMatch = command.match(/(погода|weather)\s+(\w+)/i);
            const city = cityMatch ? cityMatch[2] : 'Kyiv';
            this.speakResponse(`Завантажую погоду для ${city}`);
            window.app.searchWeather(city);
        }
        // Temperature unit commands
        else if (command.includes('цельсій') || command.includes('celsius')) {
            document.querySelector('[data-unit="metric"]').click();
            this.speakResponse('Одиниці змінені на Цельсій');
        }
        else if (command.includes('фаренгейт') || command.includes('fahrenheit')) {
            document.querySelector('[data-unit="imperial"]').click();
            this.speakResponse('Одиниці змінені на Фаренгейт');
        }
        // Greeting
        else if (command.includes('привіт') || command.includes('hello')) {
            this.speakResponse('Добро пожалувати до панелі керування погодою');
        }
        // Help
        else if (command.includes('допомога') || command.includes('help')) {
            this.speakResponse('Скажіть: погода для міста, змінити одиниці температури');
        }
        // Theme
        else if (command.includes('тема') || command.includes('theme')) {
            document.getElementById('themeBtn').click();
            this.speakResponse('Тема змінена');
        }
        else {
            this.speakResponse('Команду не розпізнано. Спробуйте запитати про погоду');
        }
    }

    async speakResponse(text) {
        try {
            const response = await fetch('/api/voice/speak', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({
                    text: text,
                    language: 'uk-UA',
                    voiceName: 'uk-UA-OstapNeural'
                })
            });

            if (response.ok) {
                const audioBlob = await response.blob();
                const audioUrl = URL.createObjectURL(audioBlob);
                const audio = document.getElementById('jarvisAudio');
                audio.src = audioUrl;
                audio.play();
            }
        } catch (error) {
            console.error('Error getting voice response:', error);
        }
    }
}

// Initialize when DOM is ready
if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', () => {
        const voiceSystem = new VoiceCommandsSystem();
        document.getElementById('voiceBtn').addEventListener('click', () => {
            voiceSystem.startListening();
        });
        window.voiceSystem = voiceSystem;
    });
} else {
    const voiceSystem = new VoiceCommandsSystem();
    document.getElementById('voiceBtn').addEventListener('click', () => {
        voiceSystem.startListening();
    });
    window.voiceSystem = voiceSystem;
}
