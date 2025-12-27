# Sound Assets

This directory contains audio feedback files for AgValoniaGPS3.

## Sound Files

Place the following WAV files in this directory:

| File Name | Purpose | Config Setting |
|-----------|---------|----------------|
| `SteerOn.wav` | AutoSteer engaged | Sound.AutoSteerSoundEnabled |
| `SteerOff.wav` | AutoSteer disengaged | Sound.AutoSteerSoundEnabled |
| `HydUp.wav` | Hydraulic lift up | Sound.HydraulicSoundEnabled |
| `HydDown.wav` | Hydraulic lift down | Sound.HydraulicSoundEnabled |
| `SectionOn.wav` | Section turned on | Sound.SectionSoundEnabled |
| `SectionOff.wav` | Section turned off | Sound.SectionSoundEnabled |
| `Alarm10.wav` | Boundary alarm | Always plays |
| `TF012.wav` | U-turn too close warning | Sound.UTurnSoundEnabled |
| `Headland.wav` | Headland approach | Always plays |
| `rtk_lost.wav` | RTK signal lost | Always plays |
| `rtk_back.wav` | RTK signal recovered | Always plays |

## Source

Sound files can be obtained from:
1. AgOpenGPS repository: `/SourceCode/GPS/Resources/`
2. Custom recordings or sound effects

## Format

- **Format**: WAV (PCM)
- **Sample Rate**: 44.1 kHz recommended
- **Bit Depth**: 16-bit
- **Channels**: Mono or Stereo

## Testing

If sound files are missing, the AudioService will log warnings to the debug console but will not crash the application. This allows development to continue without all sound files present.

## Configuration

Sound playback can be enabled/disabled in the Configuration panel under the Sound section, or by editing the profile XML:

```xml
<Sound>
  <AutoSteerSoundEnabled>true</AutoSteerSoundEnabled>
  <HydraulicSoundEnabled>true</HydraulicSoundEnabled>
  <SectionSoundEnabled>true</SectionSoundEnabled>
  <UTurnSoundEnabled>true</UTurnSoundEnabled>
</Sound>
```
