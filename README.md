# MyCrosshair

A lightweight, always-on-top crosshair overlay for Windows. Renders a fully customizable crosshair at the center of your primary screen with zero input interference — clicks pass straight through to whatever is underneath.

## Features

- **Click-through overlay** — uses `WS_EX_TRANSPARENT` at the Win32 level; never blocks mouse or keyboard input
- **Real-time preview** — every setting change reflects instantly on screen and in the config panel preview
- **Persistent settings** — configuration saved automatically to `%AppData%\MyCrosshair\settings.json`
- **System tray** — lives quietly in the tray; double-click or right-click → **配置 / Config** to open the panel
- **Bilingual UI** — switch between Chinese (中文) and English at any time

### Crosshair options

| Setting    | Range                        | Description                               |
| ---------- | ---------------------------- | ----------------------------------------- |
| Length     | 1 – 60                      | Length of each arm                        |
| Thickness  | 1 – 10                      | Line width                                |
| Gap        | 0 – 30                      | Distance between center and arm start     |
| Outline    | on/off + 1–5                | Black border around lines for contrast    |
| Color      | R 0–255, G 0–255, B 0–255 | RGB color                                 |
| Opacity    | 10 – 100 %                  | Overall transparency                      |
| Center dot | on/off + 1–20               | Filled circle at crosshair center         |
| T-Shape    | on/off                       | Removes the top arm (classic T crosshair) |

## Requirements

- Windows 10 / 11 (x64)
- Administrator privileges (required to render above elevated game windows)
- No .NET runtime needed — the release build is self-contained

## Installation

1. Download `MyCrosshair.exe` from the [Releases](../../releases) page
2. Run it — Windows UAC will prompt for administrator permission
3. The crosshair appears immediately; a tray icon gives access to the config panel

No installer, no registry writes, no background services.

## Building from source

Requires [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0).

```bash
git clone https://github.com/Clizo1209/MyCrosshair.git
cd MyCrosshair
dotnet build
```

**Publish a self-contained single-file executable:**

```bash
dotnet publish -c Release -r win-x64 --self-contained true \
  -p:PublishSingleFile=true \
  -p:IncludeNativeLibrariesForSelfExtract=true \
  -o publish
```

Output: `publish\MyCrosshair.exe`

## Project structure

```
MyCrosshair/
├── App.xaml / App.xaml.cs          # Entry point, tray icon
├── OverlayWindow.xaml / .cs        # Full-screen transparent overlay (click-through)
├── ConfigWindow.xaml / .cs         # Settings panel
├── CrosshairRenderer.cs            # Custom FrameworkElement — draws the crosshair
├── CrosshairSettings.cs            # Settings data model
├── SettingsManager.cs              # Load / save JSON
├── Loc.cs                          # Chinese / English string table
├── GlobalUsings.cs                 # Resolves WPF vs WinForms type conflicts
└── app.manifest                    # Requests administrator execution level
```

## How click-through works

The overlay window is created with `AllowsTransparency="True"` (sets `WS_EX_LAYERED`) and then `WS_EX_TRANSPARENT` is OR-ed in via `SetWindowLong` after the window handle is initialized. This tells Windows to route all hit-testing to the window beneath, so the crosshair is purely visual.
