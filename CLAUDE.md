# CLAUDE.md - AgValoniaGPS3

This file provides guidance to Claude Code when working with this repository.

## Project Overview

AgValoniaGPS3 is a clean cross-platform rewrite of AgValoniaGPS with a 95%/5% architecture split:
- **95% Shared Code**: Models, Services, ViewModels (no Avalonia/platform dependencies)
- **5% Platform Code**: Desktop (OpenGL/Silk.NET) and iOS (SkiaSharp) specific implementations

## Architecture

```
AgValoniaGPS3/
├── Shared/                              # 95% - Platform-agnostic code
│   ├── AgValoniaGPS.Models/            # Data models, no dependencies
│   ├── AgValoniaGPS.Services/          # Business logic, interfaces
│   └── AgValoniaGPS.ViewModels/        # MVVM ViewModels (ReactiveUI)
│
├── Platforms/                           # 5% - Platform-specific code
│   ├── AgValoniaGPS.Desktop/           # Avalonia Desktop (OpenGL)
│   └── AgValoniaGPS.iOS/               # Avalonia iOS (stub - needs implementation)
│
└── AgValoniaGPS.sln                    # Solution file
```

## Key Differences from AgValoniaGPS2

1. **No AgOpenGPS.Core References**: All code uses only `AgValoniaGPS.*` namespaces
2. **Clean Separation**: Shared projects have NO Avalonia dependencies
3. **Platform Abstraction**: Rendering and platform services are abstracted via interfaces

## Build Commands

```bash
# Build Desktop
cd /Users/chris/Code/AgValoniaGPS3
dotnet build Platforms/AgValoniaGPS.Desktop/AgValoniaGPS.Desktop.csproj

# Run Desktop
dotnet run --project Platforms/AgValoniaGPS.Desktop/AgValoniaGPS.Desktop.csproj

# Build entire solution
dotnet build AgValoniaGPS.sln
```

## Current Status

### Completed
- [x] Project structure created (Shared/Platforms folders)
- [x] All .csproj files created
- [x] Models copied and working
- [x] Services copied, AgOpenGPS.Core references removed
- [x] ViewModels copied, AgOpenGPS.Core references removed
- [x] Desktop platform code copied and fixed
- [x] Desktop builds successfully
- [x] Git repository initialized

### Pending
- [ ] Create iOS platform code (App.axaml, Program.cs, etc.)
- [ ] Implement SkiaSharp-based map control for iOS
- [ ] Build and test iOS on simulator
- [ ] Create Android platform (future)

## iOS Implementation Notes

The iOS project stub exists at `Platforms/AgValoniaGPS.iOS/` with just the .csproj file.
It needs:
1. `App.axaml` / `App.axaml.cs` - Avalonia application setup
2. `Program.cs` / `AppDelegate.cs` - iOS entry point
3. `MainView.axaml` - Simplified mobile UI
4. `SkiaMapControl.cs` - SkiaSharp-based rendering (no OpenGL on iOS)
5. Platform-specific DI registration

The iOS version should NOT use:
- Silk.NET.OpenGL (not supported on iOS)
- Mapsui (may have issues on mobile)
- Desktop-specific dialogs

## Service Interfaces

All services are defined via interfaces in `Shared/AgValoniaGPS.Services/Interfaces/`:
- `IGpsService` - GPS data processing
- `IUdpCommunicationService` - UDP networking
- `INtripClientService` - NTRIP RTK corrections
- `IFieldService` - Field management
- `IBoundaryRecordingService` - Boundary recording
- etc.

Desktop implementations are registered in:
`Platforms/AgValoniaGPS.Desktop/DependencyInjection/ServiceCollectionExtensions.cs`

## Technology Stack

- **.NET 10.0** - Target framework
- **Avalonia 11.3.9** - Cross-platform UI
- **ReactiveUI 20.1.1** - MVVM framework
- **Silk.NET.OpenGL** - Desktop OpenGL (NOT for iOS)
- **SkiaSharp** - Available for iOS rendering
- **Microsoft.Extensions.DependencyInjection** - DI container

## Key Files

| File | Purpose |
|------|---------|
| `Shared/AgValoniaGPS.Services/Interfaces/IGpsService.cs` | GPS service interface |
| `Shared/AgValoniaGPS.ViewModels/MainViewModel.cs` | Main UI state |
| `Platforms/AgValoniaGPS.Desktop/Controls/OpenGLMapControl.cs` | Desktop map rendering |
| `Platforms/AgValoniaGPS.Desktop/DependencyInjection/ServiceCollectionExtensions.cs` | DI setup |

## Session Continuation

This project was created by migrating from AgValoniaGPS2. The key changes made:
1. Created fresh directory structure in `/Users/chris/Code/AgValoniaGPS3`
2. Copied Models, Services, ViewModels from AgValoniaGPS2
3. Used `sed` to replace all `AgOpenGPS.Core.*` references with `AgValoniaGPS.*`
4. Fixed interface mismatches (`IGpsService.UpdateGpsData`, `IFieldStatisticsService`)
5. Desktop now builds successfully

Next steps when continuing:
1. Open VSCode in `/Users/chris/Code/AgValoniaGPS3`
2. Verify Desktop still builds: `dotnet build Platforms/AgValoniaGPS.Desktop/AgValoniaGPS.Desktop.csproj`
3. Implement iOS platform code when ready
