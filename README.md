# Legatobase

A personal music library manager and player with a focus on metadata, relationships, and insights.[^1]

[^1]: Well, that's what it _will_ be.

> - **Legato:** "A smooth and connected manner of performance (as of music)" ([Merriam-Webster Dictionary](https://www.merriam-webster.com/dictionary/legato))
> - Data**base**: "A structured set of data held in computer storage and typically accessed or manipulated by means of specialized software" ([Oxford English Dictionary](https://www.oed.com/dictionary/database_n?tab=meaning_and_use))

## Database structure

### Entities

- **Track:** A single specific instance of a song, associated to a single audio file.
- **Artist:** A person or group of people who compose and/or perform music. A supertype with subtypes of "Individual" and "Group", where the Group is made up of artists who are also in the database as individuals.
- **Album:** A collection of tracks released together. There can be multiple editions of an album, so additional database fields identify the specific edition(s) in the collection.

---
## Glossary

- **ISRC:** International Standard Recording Code, a unique identifier for a single recording (a _track_). A recording is an instance of a _work_.
- **ISWC:** International Standard Work Code, a unique identifier for an individual musical composition. There can be multiple versions of a work, which take the form of different recordings or tracks.
- **MBID:** MusicBrainz Identifier, a unique identifier for an entity in the [MusicBrainz database](https://musicbrainz.org/doc/MusicBrainz_Identifier). MBIDs are also used by some other services, such as [Setlist.fm](https://api.setlist.fm/docs/1.0/json_Artists.html).