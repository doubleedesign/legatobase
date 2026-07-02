# Legatobase

A personal music library manager and player with a focus on metadata, relationships, and insights.[^1]

[^1]: Well, that's what it _will_ be.

> - **Legato:** "A smooth and connected manner of performance (as of music)" ([Merriam-Webster Dictionary](https://www.merriam-webster.com/dictionary/legato))
> - Data**base**: "A structured set of data held in computer storage and typically accessed or manipulated by means of specialized software" ([Oxford English Dictionary](https://www.oed.com/dictionary/database_n?tab=meaning_and_use))

## Requirements
- Windows 11+
- [Discogs API consumer key and secret](https://www.discogs.com/settings/developers)

---
## Development

### Prerequisites
- .NET 10+ ([download](https://dotnet.microsoft.com/en-us/download/dotnet) or `choco install dotnet`)
- .NET SDK ([download](https://dotnet.microsoft.com/en-us/download/dotnet) or `choco install dotnet-sdk`)
- Windows 11 SDK ([download](https://learn.microsoft.com/en-gb/windows/apps/windows-sdk/downloads))
- A .NET IDE, e.g., [Jetbrains Rider](https://www.jetbrains.com/rider/) or [Visual Studio](https://visualstudio.microsoft.com/vs/)

For working on the GUI app:
- Run `dotnet workload restore` from the `./app` directory to ensure all .required NET workloads are installed
- In Rider: In the Solution Explorer, right-click on the `app` project and select "Reload project" after installing workloads.

### Project structure

The solution is made up of multiple projects:

| Project | Purpose                                                                                                                                                                                                                         |
|---------|---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| Setup   | Console app for database creation, regeneration of Entity Framework classes (if the database schema changes), and manually running some API functions via console.                                                              |
| Core    | The auto-generated Entity Framework classes. Extra methods for objects should live in classes that extend these ones (in the appropriate other project) so they can easily be regenerated if the database schema changes.       |
| Common  | Centralised configuration, utility classes, extension methods, etc.                                                                                                                                                             |
| API     | The back-end code for handling importing and updating data, fetching data from third-party APIs, getting and transforming data from the database for display, etc.[^1]                                                              |
| App     | Cross-platform [MAUI](https://dotnet.microsoft.com/en-us/apps/maui) / [Blazor hybrid](https://learn.microsoft.com/en-us/aspnet/core/blazor/hybrid/tutorials/maui?view=aspnetcore-10.0) app for the GUI.[^1] |
| Tests   | Unit and integration tests for the API.                                                                                                                                                                                         |

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