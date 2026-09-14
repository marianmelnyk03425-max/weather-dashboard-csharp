using WeatherDashboard.Models;
using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace WeatherDashboard.Services;

/// <summary>
/// Local speech synthesis service using available system TTS
/// Supports Windows (SAPI), Linux (espeak), macOS (say command)
/// JARVIS MALE voice configuration
/// </summary>
public class LocalSpeechService : IVoiceService
{
    private readonly ILogger<LocalSpeechService> _logger;

    public LocalSpeechService(ILogger<LocalSpeechService> logger)
    {
        _logger = logger;
    }

    public async Task<byte[]?> TextToSpeechAsync(string text, VoiceConfig? config = null)
    {
        try
        {
            config ??= new VoiceConfig();
            var audioFile = Path.Combine(Path.GetTempPath(), $"speech_{Guid.NewGuid()}.mp3");

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return await WindowsTTSAsync(text, audioFile, config);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return await LinuxTTSAsync(text, audioFile, config);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return await MacOSTTSAsync(text, audioFile, config);
            }

            _logger.LogWarning("Unsupported platform for local TTS");
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in TextToSpeechAsync");
            return null;
        }
    }

    public async Task<byte[]?> AnnounceWeatherAsync(WeatherResponse weather)
    {
        var announcement = $"У місті {weather.City}. Температура {weather.Temperature} градусів. {weather.Description}.";
        return await TextToSpeechAsync(announcement);
    }

    public async Task<byte[]?> GreetAsync()
    {
        return await TextToSpeechAsync("Добро пожалувати. Я Джарвіс. Як я можу вам допомогти?");
    }

    public async Task<List<string>> GetAvailableVoicesAsync()
    {
        return await Task.FromResult(new List<string> 
        { 
            "uk-UA-Male",     // MALE (JARVIS style)
            "uk-UA-Default"   // Default
        });
    }

    private async Task<byte[]?> WindowsTTSAsync(string text, string outputFile, VoiceConfig config)
    {
        try
        {
            // Using PowerShell with SAPI for Ukrainian MALE voice support
            var psScript = $@"
Add-Type –AssemblyName System.Speech
$speak = New-Object System.Speech.Synthesis.SpeechSynthesizer
$speak.Rate = {(int)(config.SpeechRate * 10) - 10}
# Use male voice (David for Ukrainian or default male)
$speak.SelectVoice('Microsoft David Desktop')
$speak.Speak('{text.Replace("'", "''")}')  
";

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "powershell.exe",
                    Arguments = $"-Command \"{psScript}\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };

            process.Start();
            await process.WaitForExitAsync();

            if (File.Exists(outputFile))
            {
                return await File.ReadAllBytesAsync(outputFile);
            }

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Windows TTS error");
            return null;
        }
    }

    private async Task<byte[]?> LinuxTTSAsync(string text, string outputFile, VoiceConfig config)
    {
        try
        {
            // Using espeak for Linux with male voice (m7 = male voice variant)
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "espeak",
                    Arguments = $"-v uk -m -s 150 -o {outputFile} \"{text}\"", // -m for male
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };

            process.Start();
            await process.WaitForExitAsync();

            if (File.Exists(outputFile))
            {
                return await File.ReadAllBytesAsync(outputFile);
            }

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Linux TTS error");
            return null;
        }
    }

    private async Task<byte[]?> MacOSTTSAsync(string text, string outputFile, VoiceConfig config)
    {
        try
        {
            // Using macOS 'say' command with male voice (Alex or similar)
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "say",
                    Arguments = $"-v Alex -r 150 \"{text}\"", // Alex is male voice on macOS
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };

            process.Start();
            await process.WaitForExitAsync();

            return null; // macOS say doesn't easily save to file
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "macOS TTS error");
            return null;
        }
    }
}
