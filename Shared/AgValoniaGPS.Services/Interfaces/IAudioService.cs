namespace AgValoniaGPS.Services.Interfaces;

/// <summary>
/// Service for playing audio feedback sounds based on application events.
/// Sounds provide user feedback for state changes without requiring visual attention.
/// </summary>
public interface IAudioService
{
    /// <summary>
    /// Plays the AutoSteer engaged sound.
    /// </summary>
    void PlayAutoSteerOn();

    /// <summary>
    /// Plays the AutoSteer disengaged sound.
    /// </summary>
    void PlayAutoSteerOff();

    /// <summary>
    /// Plays the hydraulic lift up sound.
    /// </summary>
    void PlayHydraulicLiftUp();

    /// <summary>
    /// Plays the hydraulic lift down sound.
    /// </summary>
    void PlayHydraulicLiftDown();

    /// <summary>
    /// Plays the section turned on sound.
    /// </summary>
    void PlaySectionOn();

    /// <summary>
    /// Plays the section turned off sound.
    /// </summary>
    void PlaySectionOff();

    /// <summary>
    /// Plays the boundary alarm sound (approaching field boundary).
    /// </summary>
    void PlayBoundaryAlarm();

    /// <summary>
    /// Plays the U-turn too close warning sound.
    /// </summary>
    void PlayUTurnWarning();

    /// <summary>
    /// Plays the headland approach sound.
    /// </summary>
    void PlayHeadlandApproach();

    /// <summary>
    /// Plays the RTK signal lost alarm sound.
    /// </summary>
    void PlayRtkLost();

    /// <summary>
    /// Plays the RTK signal recovered sound.
    /// </summary>
    void PlayRtkRecovered();

    /// <summary>
    /// Checks if AutoSteer sounds are enabled in configuration.
    /// </summary>
    bool IsAutoSteerSoundEnabled { get; }

    /// <summary>
    /// Checks if hydraulic lift sounds are enabled in configuration.
    /// </summary>
    bool IsHydraulicSoundEnabled { get; }

    /// <summary>
    /// Checks if section control sounds are enabled in configuration.
    /// </summary>
    bool IsSectionSoundEnabled { get; }

    /// <summary>
    /// Checks if U-turn sounds are enabled in configuration.
    /// </summary>
    bool IsUTurnSoundEnabled { get; }
}
