# Legatobase

A personal music library manager and player with a focus on metadata, relationships, and insights.[^1]

[^1]: Well, that's what it _will_ be.

> - **Legato:** "A smooth and connected manner of performance (as of music)" ([Merriam-Webster Dictionary](https://www.merriam-webster.com/dictionary/legato))
> - Data**base**: "A structured set of data held in computer storage and typically accessed or manipulated by means of specialized software" ([Oxford English Dictionary](https://www.oed.com/dictionary/database_n?tab=meaning_and_use))

## Requirements
- Windows 11
- [Discogs API consumer key and secret](https://www.discogs.com/settings/developers)
- Optional: Android device

## Installation
Instructions to come when the app is actually ready for general use.

---
## Development

- [Prerequisites](#prerequisites)
- [Rider setup](#rider-setup)
  - [General stuff](#general-stuff)
  - [.NET stuff](#net-stuff)
  - [Android stuff](#android-stuff)
- [Project structure](#project-structure)
- [Running the app](#running-the-app)
  - [Windows](#windows)
  - [Android](#android)
- [Troubleshooting](#troubleshooting)
  - [Initial setup issues](#initial-setup-issues)
  - [System environment variables](#system-environment-variables)
  - [Errors when running the app](#errors-when-running-the-app)

The instructions below are written per my preferred environment and tooling:
- [Chocolatey](https://chocolatey.org/install) for system-level package management
- [Jetbrains Rider](https://www.jetbrains.com/rider/) with the [Android Support plugin](https://plugins.jetbrains.com/plugin/12056-rider-android-support) for .NET development and managing the Android SDK and emulator for MAUI projects.

### Prerequisites
- .NET 10+ ([download](https://dotnet.microsoft.com/en-us/download/dotnet) or `choco install dotnet`)
- .NET SDK ([download](https://dotnet.microsoft.com/en-us/download/dotnet) or `choco install dotnet-sdk`)
- A .NET IDE, e.g., [Jetbrains Rider](https://www.jetbrains.com/rider/) or Visual Studio
- Windows 11 SDK ([download](https://learn.microsoft.com/en-gb/windows/apps/windows-sdk/downloads))
- Microsoft OpenJDK 21[^2] ([download](https://learn.microsoft.com/en-us/java/openjdk/download) or `choco install microsoft-openjdk-21`) and point Rider to it in Settings > Build, Execution, Deployment > Android
- Android SDK - `choco install android-sdk` is recommended initially, as this will set the required system environment variables for you[^3]
- [.NET MAUI CLI](https://learn.microsoft.com/en-us/dotnet/maui/developer-tools/cli/?view=net-maui-10.0) `dotnet tool install -g Microsoft.Maui.Cli --prerelease`

[^2]: OpenJDK 21 is the newest version compatible with Android API level 36, which this project is set to target at the time of writing.
[^3]: Alternatively, you can use Android Studio to install and manage the Android SDK and emulators, but I think it's overkill and kinda annoying to have an entire extra IDE installed just for that.

### Rider setup

#### General stuff
- In Settings > Tools > Terminal, ensure "Shell integration" is enabled, so all the settings for SDKs and whatnot in the IDE are used by the built-in terminal.

#### .NET stuff
- In Settings > Build, Execution, Deployment > Toolset and Build, ensure the path to your .NET executable is correct and that the latest MSBuild version is selected (matching the latest SDK version you have installed)
- In the terminal, run `dotnet workload restore` to ensure all .required NET workloads are installed
- In the Solution Explorer, right-click on the `app` project and select "Reload project"

**Note:** You will also need to repeat these steps after upgrading the .NET SDK on your machine.

#### Android stuff
Go to Settings > Build, Execution, Deployment > Android and:
- Set the path to your Android SDK installation; Chocolatey's default is `C:\Android\android-sdk`
- Set the Android version to 14 (SDK 34) and follow the prompts to install it if needed (this is the newest version supported by the Sony Android Walkman NW-ZX707)
- Set the path to your JDK installation; Chocolatey's default for Microsoft OpenJDK is `C:\Program Files\Microsoft\jdk-<version>` (it should pick it up and you can select it from the dropdown)
- Go to Settings > Build, Execution, Deployment > Android > SDK Updater, and in the SDK Tools tab ensure the following are installed (and install them from there if not):
  - Android SDK Command-line Tools (latest)
  - Android SDK Platform-Tools
  - Android SDK Build-Tools (latest)
  - Android Emulator
- Go to Settings > Tools > Terminal and ensure Rider is seeing the correct system environment variables for `ANDROID_HOME`, `JAVA_HOME`, and the `PATH` entries listed in the Troubleshooting section below. If not, restart Rider. You may also need to restart Jetbrains Toolbox if it still doesn't update.
- In your terminal, run `maui doctor` to ensure the MAUI CLI is picking everything up. If not, check the Troubleshooting section below.

### Project structure

The solution is made up of multiple projects:

| Project | Purpose                                                                                                                                                                                                                         |
|---------|---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| Setup   | Console app for database creation, regeneration of Entity Framework classes (if the database schema changes), and manually running some API functions via console.                                                              |
| Core    | The auto-generated Entity Framework classes. Extra methods for objects should live in classes that extend these ones (in the appropriate other project) so they can easily be regenerated if the database schema changes.       |
| Common  | Centralised configuration, utility classes, extension methods, etc.                                                                                                                                                             |
| API     | The back-end code for handling importing and updating data, fetching data from third-party APIs, getting and transforming data from the database for display, etc.[^1]                                                          |
| App     | Cross-platform [MAUI](https://dotnet.microsoft.com/en-us/apps/maui) app for the GUI.[^1]                      |
| Tests   | Unit and integration tests for the API.                                                                                                                                                                                         |

### Running the app

Run configurations are provided for Rider. To use what hot reload support is available, use the built-in Run configurations in debug mode.

Alternatively, you can run from the terminal and select the target platform when prompted.

```powershell
dotnet watch run
```
**Note:** Before running in Android, you will need to set up a simulator (virtual device) in Rider's Device Manager. Select a minimum of API version 34.

#### Hot reload

When running from the terminal or using a Rider Run config in debug mode, hot reload is supported for the C# code in the app, but not XAML. (In Visual Studio, it's [the other way around](https://learn.microsoft.com/en-us/dotnet/maui/xaml/hot-reload)).

You may need to click "Apply changes" in the floating toolbar in Rider (or set a keyboard shortcut and use that).

### Troubleshooting

#### Initial setup issues

If you see lots of IDE errors after first setting up the project, try:
  - `dotnet workload restore`
  - In the Solution Explorer, right-click on the solution and select "Reload all projects"
  - Check your system environment variables and ensure Rider is seeing the correct values in Settings > Tools > Terminal (if not, restart Rider and Jetbrains Toolbox; you can also override them for the duration of your session from that Settings dialog)
  - From the `./app` directory, run `maui doctor`

> Workload update failed: Value cannot be null. (Parameter 's')

If you get this error when installing the MAUI workload, open an elevated PowerShell terminal, `cd` to the project directory, and run:
```powershell
dotnet workload install maui --no-cache
```

Then back in Rider, close and re-open the terminal before running `maui doctor` to check the dependencies again.


#### System environment variables

System environment variables you may need to check and update if something is not working:
- `JAVA_HOME`: the path of your Microsoft OpenJDK installation (e.g., `C:\Program Files\Microsoft\jdk-21.0.11.10-hotspot`)
- `ANDROID_HOME`: the path of your Android SDK installation (e.g., `C:\Android\android-sdk`)
- Present in your user `PATH`:
  - `%USERPROFILE%\.dotnet\tools`
- Present in your system `PATH`[^3]:
  - `C:\Program Files\dotnet\`
  - `C:\Android\android-sdk\tools`
  - `C:\Android\android-sdk\tools\bin`
  - `C:\Android\android-sdk\platform-tools`
  - `C:\Program Files\Microsoft\jdk-21.0.11.10-hotspot\bin`

If `maui doctor` or `Get-Command <thing>` are showing different paths in Rider's terminal, check for and remove unwanted overrides in Settings > Tools > Terminal.

[^3]: Or the equivalent paths/versions if yours are different.

#### Errors when running the app

> The file watcher observing encountered an error: Too many changes at once in directory

Fix: Shut down the build server before restarting the app:
```powershell
dotnet build-server shutdown
```
```powershell
dotnet watch run
```

---
## Database structure

### Entities
- **Track:** A single specific instance of a song, associated to a single audio file.
- **Artist:** A person or group of people who compose and/or perform music. A supertype with subtypes of "Individual" and "Group", where the Group is made up of artists who are also in the database as individuals. For the purpose of the database, producers are also artists even if they do not compose or play music themselves; given the handling of individual and group artists it would overcomplicate things to have a "Person" entity who is sometimes an artist and sometimes not, given a group would be an Artist but not a Person...
- **Album:** A collection of tracks released together. There can be multiple editions of an album, so additional database fields identify the specific edition(s) in the collection.

---
## Glossary

- **ISRC:** International Standard Recording Code, a unique identifier for a single recording (a _track_). A recording is an instance of a _work_.
- **ISWC:** International Standard Work Code, a unique identifier for an individual musical composition. There can be multiple versions of a work, which take the form of different recordings or tracks.
- **MBID:** MusicBrainz Identifier, a unique identifier for an entity in the [MusicBrainz database](https://musicbrainz.org/doc/MusicBrainz_Identifier). MBIDs are also used by some other services, such as [Setlist.fm](https://api.setlist.fm/docs/1.0/json_Artists.html).
- **DID:** Discogs Identifier, a unique identifier for an entity in the [Discogs database](https://www.discogs.com/developers/).