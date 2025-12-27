using System;
using System.IO;
using System.Diagnostics;
using NAudio.Wave;
using AgValoniaGPS.Services.Interfaces;
using AgValoniaGPS.Models.Configuration;

namespace AgValoniaGPS.Desktop.Services;

/// <summary>
/// Desktop audio service implementation using NAudio.
/// Plays WAV files for various application events based on configuration settings.
/// </summary>
public class AudioService : IAudioService, IDisposable
{
    private readonly IWavePlayer _waveOut;
    private readonly string _soundsPath;
    private WaveFileReader? _currentReader;
    private readonly object _lock = new();

    // Sound file names matching AgOpenGPS convention
    private const string SteerOnFile = "SteerOn.wav";
    private const string SteerOffFile = "SteerOff.wav";
    private const string HydUpFile = "HydUp.wav";
    private const string HydDownFile = "HydDown.wav";
    private const string SectionOnFile = "SectionOn.wav";
    private const string SectionOffFile = "SectionOff.wav";
    private const string Alarm10File = "Alarm10.wav";
    private const string UTurnWarningFile = "TF012.wav";
    private const string HeadlandFile = "Headland.wav";
    private const string RtkLostFile = "rtk_lost.wav";
    private const string RtkBackFile = "rtk_back.wav";

    public AudioService()
    {
        // Initialize wave output device
        _waveOut = new WaveOutEvent();

        // Determine sounds directory path
        var baseDir = AppDomain.CurrentDomain.BaseDirectory;
        _soundsPath = Path.Combine(baseDir, "Assets", "Sounds");

        // Create directory if it doesn't exist
        if (!Directory.Exists(_soundsPath))
        {
            Directory.CreateDirectory(_soundsPath);
            Debug.WriteLine($"[AudioService] Created sounds directory: {_soundsPath}");
        }

        Debug.WriteLine($"[AudioService] Initialized. Sounds path: {_soundsPath}");
    }

    public bool IsAutoSteerSoundEnabled =>
        ConfigurationStore.Instance?.Sound?.AutoSteerSoundEnabled ?? false;

    public bool IsHydraulicSoundEnabled =>
        ConfigurationStore.Instance?.Sound?.HydraulicSoundEnabled ?? false;

    public bool IsSectionSoundEnabled =>
        ConfigurationStore.Instance?.Sound?.SectionSoundEnabled ?? false;

    public bool IsUTurnSoundEnabled =>
        ConfigurationStore.Instance?.Sound?.UTurnSoundEnabled ?? false;

    public void PlayAutoSteerOn()
    {
        if (IsAutoSteerSoundEnabled)
        {
            PlaySound(SteerOnFile);
        }
    }

    public void PlayAutoSteerOff()
    {
        if (IsAutoSteerSoundEnabled)
        {
            PlaySound(SteerOffFile);
        }
    }

    public void PlayHydraulicLiftUp()
    {
        if (IsHydraulicSoundEnabled)
        {
            PlaySound(HydUpFile);
        }
    }

    public void PlayHydraulicLiftDown()
    {
        if (IsHydraulicSoundEnabled)
        {
            PlaySound(HydDownFile);
        }
    }

    public void PlaySectionOn()
    {
        if (IsSectionSoundEnabled)
        {
            PlaySound(SectionOnFile);
        }
    }

    public void PlaySectionOff()
    {
        if (IsSectionSoundEnabled)
        {
            PlaySound(SectionOffFile);
        }
    }

    public void PlayBoundaryAlarm()
    {
        // Boundary alarm always plays (critical safety feature)
        PlaySound(Alarm10File);
    }

    public void PlayUTurnWarning()
    {
        if (IsUTurnSoundEnabled)
        {
            PlaySound(UTurnWarningFile);
        }
    }

    public void PlayHeadlandApproach()
    {
        // Headland approach always plays (important navigation feedback)
        PlaySound(HeadlandFile);
    }

    public void PlayRtkLost()
    {
        // RTK alarms always play (critical GPS quality feedback)
        PlaySound(RtkLostFile);
    }

    public void PlayRtkRecovered()
    {
        // RTK recovery always plays
        PlaySound(RtkBackFile);
    }

    private void PlaySound(string filename)
    {
        try
        {
            lock (_lock)
            {
                // Stop any currently playing sound
                if (_waveOut.PlaybackState == PlaybackState.Playing)
                {
                    _waveOut.Stop();
                }

                // Dispose previous reader
                _currentReader?.Dispose();
                _currentReader = null;

                // Load and play new sound
                var soundPath = Path.Combine(_soundsPath, filename);

                if (!File.Exists(soundPath))
                {
                    Debug.WriteLine($"[AudioService] Sound file not found: {soundPath}");
                    return;
                }

                _currentReader = new WaveFileReader(soundPath);
                _waveOut.Init(_currentReader);
                _waveOut.Play();

                Debug.WriteLine($"[AudioService] Playing: {filename}");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[AudioService] Error playing sound '{filename}': {ex.Message}");
        }
    }

    public void Dispose()
    {
        lock (_lock)
        {
            _waveOut?.Stop();
            _waveOut?.Dispose();
            _currentReader?.Dispose();
        }
    }
}
