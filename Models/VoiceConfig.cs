namespace WeatherDashboard.Models;

public class VoiceConfig
{
    public string Language { get; set; } = "uk-UA";
    public string VoiceName { get; set; } = "uk-UA-OstapNeural"; // Ukrainian MALE voice (JARVIS-like)
    public float SpeechRate { get; set; } = 1.0f;
    public float Pitch { get; set; } = 0.95f; // Deeper pitch for male voice
    public string VoiceStyle { get; set; } = "professional"; // professional, formal, newscast
}

public class VoiceRequest
{
    public string Text { get; set; } = string.Empty;
    public string? Language { get; set; }
    public string? VoiceName { get; set; }
    public float? SpeechRate { get; set; }
}

public class AudioResponse
{
    public byte[]? AudioData { get; set; }
    public string ContentType { get; set; } = "audio/mpeg";
    public DateTime GeneratedAt { get; set; }
}
