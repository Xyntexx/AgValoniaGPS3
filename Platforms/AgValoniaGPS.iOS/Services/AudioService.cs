using System;
using System.Diagnostics;
using AgValoniaGPS.Services.Interfaces;
using AgValoniaGPS.Models.Configuration;

namespace AgValoniaGPS.iOS.Services;

/// <summary>
/// iOS audio service stub implementation.
/// TODO: Implement using AVFoundation (AVAudioPlayer) when iOS platform is ready for testing.
/// </summary>
public class AudioService : IAudioService
{
    public AudioService()
    {
        Debug.WriteLine("[AudioService] iOS stub initialized");
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
            LogSound("SteerOn");
            // TODO: Implement with AVAudioPlayer
        }
    }

    public void PlayAutoSteerOff()
    {
        if (IsAutoSteerSoundEnabled)
        {
            LogSound("SteerOff");
            // TODO: Implement with AVAudioPlayer
        }
    }

    public void PlayHydraulicLiftUp()
    {
        if (IsHydraulicSoundEnabled)
        {
            LogSound("HydUp");
            // TODO: Implement with AVAudioPlayer
        }
    }

    public void PlayHydraulicLiftDown()
    {
        if (IsHydraulicSoundEnabled)
        {
            LogSound("HydDown");
            // TODO: Implement with AVAudioPlayer
        }
    }

    public void PlaySectionOn()
    {
        if (IsSectionSoundEnabled)
        {
            LogSound("SectionOn");
            // TODO: Implement with AVAudioPlayer
        }
    }

    public void PlaySectionOff()
    {
        if (IsSectionSoundEnabled)
        {
            LogSound("SectionOff");
            // TODO: Implement with AVAudioPlayer
        }
    }

    public void PlayBoundaryAlarm()
    {
        LogSound("BoundaryAlarm");
        // TODO: Implement with AVAudioPlayer
    }

    public void PlayUTurnWarning()
    {
        if (IsUTurnSoundEnabled)
        {
            LogSound("UTurnWarning");
            // TODO: Implement with AVAudioPlayer
        }
    }

    public void PlayHeadlandApproach()
    {
        LogSound("HeadlandApproach");
        // TODO: Implement with AVAudioPlayer
    }

    public void PlayRtkLost()
    {
        LogSound("RtkLost");
        // TODO: Implement with AVAudioPlayer
    }

    public void PlayRtkRecovered()
    {
        LogSound("RtkRecovered");
        // TODO: Implement with AVAudioPlayer
    }

    private void LogSound(string soundName)
    {
        Debug.WriteLine($"[AudioService] iOS would play: {soundName}");
    }
}
