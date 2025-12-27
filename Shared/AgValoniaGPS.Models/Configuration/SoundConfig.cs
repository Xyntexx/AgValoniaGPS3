using ReactiveUI;

namespace AgValoniaGPS.Models.Configuration;

/// <summary>
/// Sound and audio feedback configuration.
/// Controls which sounds are played for various application events.
/// </summary>
public class SoundConfig : ReactiveObject
{
    private bool _autoSteerSoundEnabled = true;
    private bool _hydraulicSoundEnabled = true;
    private bool _sectionSoundEnabled = true;
    private bool _uTurnSoundEnabled = true;

    /// <summary>
    /// Enable sounds when AutoSteer is engaged/disengaged.
    /// </summary>
    public bool AutoSteerSoundEnabled
    {
        get => _autoSteerSoundEnabled;
        set => this.RaiseAndSetIfChanged(ref _autoSteerSoundEnabled, value);
    }

    /// <summary>
    /// Enable sounds when hydraulic lift moves up/down.
    /// </summary>
    public bool HydraulicSoundEnabled
    {
        get => _hydraulicSoundEnabled;
        set => this.RaiseAndSetIfChanged(ref _hydraulicSoundEnabled, value);
    }

    /// <summary>
    /// Enable sounds when sections turn on/off.
    /// </summary>
    public bool SectionSoundEnabled
    {
        get => _sectionSoundEnabled;
        set => this.RaiseAndSetIfChanged(ref _sectionSoundEnabled, value);
    }

    /// <summary>
    /// Enable sounds for U-turn warnings.
    /// </summary>
    public bool UTurnSoundEnabled
    {
        get => _uTurnSoundEnabled;
        set => this.RaiseAndSetIfChanged(ref _uTurnSoundEnabled, value);
    }

    // Note: Boundary alarms, RTK lost/recovered, and headland approach sounds
    // always play as they are critical safety/navigation features
}
