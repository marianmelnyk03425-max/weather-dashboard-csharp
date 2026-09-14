using Microsoft.CognitiveServices.Speech;
using WeatherDashboard.Models;
using Microsoft.Extensions.Logging;

namespace WeatherDashboard.Services;

public class AzureSpeechService : IVoiceService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<AzureSpeechService> _logger;
    private readonly string? _subscriptionKey;
    private readonly string? _region;

    public AzureSpeechService(IConfiguration configuration, ILogger<AzureSpeechService> logger)
    {
        _configuration = configuration;
        _logger = logger;
        _subscriptionKey = _configuration["AzureSpeech:SubscriptionKey"] ?? 
                          Environment.GetEnvironmentVariable("AZURE_SPEECH_KEY");
        _region = _configuration["AzureSpeech:Region"] ?? 
                 Environment.GetEnvironmentVariable("AZURE_SPEECH_REGION") ?? "eastus";
    }

    public async Task<byte[]?> TextToSpeechAsync(string text, VoiceConfig? config = null)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(_subscriptionKey))
            {
                _logger.LogWarning("Azure Speech subscription key not configured. Using fallback TTS.");
                return await FallbackTextToSpeechAsync(text, config);
            }

            var speechConfig = SpeechConfig.FromSubscription(_subscriptionKey, _region);
            config ??= new VoiceConfig();
            
            speechConfig.SpeechSynthesisVoiceName = config.VoiceName;
            speechConfig.SpeechSynthesisOutputFormat = SpeechSynthesisOutputFormat.Audio16Khz32KBitRateMonoMp3;

            using var synthesizer = new SpeechSynthesizer(speechConfig, null);
            
            // SSML for JARVIS-like professional MALE voice
            var ssml = GenerateSSML(text, config);
            _logger.LogInformation("Generating JARVIS speech: {Text}", text.Substring(0, Math.Min(50, text.Length)));
            
            var result = await synthesizer.SpeakSsmlAsync(ssml);

            if (result.Reason == ResultReason.SynthesizingAudioCompleted)
            {
                _logger.LogInformation("JARVIS speech synthesis completed successfully");
                return result.AudioData;
            }
            else if (result.Reason == ResultReason.Canceled)
            {
                var cancellation = SpeechSynthesisCancellationDetails.FromResult(result);
                _logger.LogError("Speech synthesis canceled: {Error}", cancellation.ErrorDetails);
                return null;
            }

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in TextToSpeechAsync");
            return await FallbackTextToSpeechAsync(text, config);
        }
    }

    public async Task<byte[]?> AnnounceWeatherAsync(WeatherResponse weather)
    {
        var announcement = GenerateWeatherAnnouncement(weather);
        var config = new VoiceConfig
        {
            VoiceName = "uk-UA-OstapNeural", // MALE voice
            SpeechRate = 0.95f,
            Pitch = 0.95f // Deeper for male voice
        };
        return await TextToSpeechAsync(announcement, config);
    }

    public async Task<byte[]?> GreetAsync()
    {
        var greeting = "Добро пожалувати до панелі керування погодою. Я — Джарвіс. Як я можу вам допомогти?";
        var config = new VoiceConfig
        {
            VoiceName = "uk-UA-OstapNeural", // MALE voice
            SpeechRate = 1.0f,
            Pitch = 0.95f
        };
        return await TextToSpeechAsync(greeting, config);
    }

    public async Task<List<string>> GetAvailableVoicesAsync()
    {
        var voices = new List<string>
        {
            "uk-UA-OstapNeural",      // MALE - JARVIS style (RECOMMENDED)
            "uk-UA-PavloNeural",      // MALE alternative
        };
        return await Task.FromResult(voices);
    }

    private string GenerateSSML(string text, VoiceConfig config)
    {
        var rate = (config.SpeechRate - 1.0f) * 100; // Convert to percentage
        var pitch = (config.Pitch - 1.0f) * 100;
        
        return $@"<speak version='1.0' xml:lang='uk-UA'>
            <voice name='{config.VoiceName}'>
                <prosody rate='{rate:+0;-0;0}%' pitch='{pitch:+0;-0;0}%'>
                    {text}
                </prosody>
            </voice>
        </speak>";
    }

    private string GenerateWeatherAnnouncement(WeatherResponse weather)
    {
        return $@"У місті {weather.City}, {weather.Country}. 
                  Температура: {weather.Temperature} градусів Цельсія. 
                  Відчувається як: {weather.FeelsLike} градусів. 
                  Вологість: {weather.Humidity} відсотків. 
                  Швидкість вітру: {weather.WindSpeed} метрів за секунду. 
                  Умови: {weather.Description}. 
                  Сонце сходить о {weather.Sunrise:HH:mm} та заходить о {weather.Sunset:HH:mm}.";
    }

    // Fallback TTS using Windows SAPI (for development without Azure)
    private async Task<byte[]?> FallbackTextToSpeechAsync(string text, VoiceConfig? config)
    {
        try
        {
            _logger.LogInformation("Using fallback TTS (Windows SAPI)");
            
            // This is a simplified fallback - in production, use a proper library
            // For now, we'll create a dummy audio response
            // In real implementation, use libraries like NReco.ImageGenerator or similar
            
            return await Task.FromResult<byte[]?>(null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fallback TTS error");
            return null;
        }
    }
}
