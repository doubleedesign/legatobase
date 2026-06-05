using Legatobase.Core;
using Legatobase.Common;
using OneOf;

namespace Legatobase.API;

using TrackData = Dictionary<string, OneOf<string, int>>;

public class TrackImporter : Importer {
	
	public TrackImporter(LbContext db) : base(db) {
	}

	// public Track? Import(TrackData data) {
	// 	data.TryGetValue("Title", out var title);
	// 	if (title.IsNull()) {
	// 		throw new Exception("Cannot import track without a title.");
	// 	}
	// 	
	// 	data.TryGetValue("Genre", out var genreLabel);
	// 	Genre? genre = !genreLabel.IsNull() ? this.FindOrCreateGenre(genreLabel.AsString()!) : null;
	//
	// 	data.TryGetValue("Kind", out var fileTypeName);
	// 	FileType? fileType = !fileTypeName.IsNull() ? this.FindOrCreateFileType(fileTypeName.AsString()!) : null;
	// 	
	// 	string? releaseArtistName = data["Sort Artist"].AsString() ?? data["Artist"].AsString() ?? data["Album Artist"].AsString();
	// 	if (releaseArtistName is not null) {
	// 		List<string> releaseArtists = releaseArtistName.Split("feat.").Select(name => name.Trim()).ToList();
	// 		Artist releaseArtist =  this.FindOrCreateArtist(releaseArtists[0]);
	// 		// TODO create ArtistTrackConnection with role "ReleaseArtist"
	// 		List<string> featuredArtists = releaseArtists.Skip(1).ToList();
	// 		foreach (string artistName in featuredArtists) {
	// 			Artist artist = this.FindOrCreateArtist(artistName);
	// 			// TODO create ArtistTrackConnection with role "Featured"
	// 		}
	// 	}
	// 	
	// 	data.TryGetValue("Composer", out var composersRaw);
	// 	if (!composersRaw.IsNull()) {
	// 		List<string> composers = composersRaw.AsString()!.Split(',', '&').Select(name => name.Trim()).ToList();
	// 		foreach (string composer in composers) {
	// 			Artist artist = this.FindOrCreateArtist(composer);
	// 			// TODO create ArtistTrackConnection with role "Composer"
	// 			// TODO: Further refine with MBID data if available
	// 		}
	// 	}
	//
	// 	Track result = new Track {
	// 		Title = title.ToString().Trim(),
	// 		Year = data.GetValueOrDefault("Year").AsInt(),
	// 		PlayCount = data.GetValueOrDefault("Play Count").AsInt(),
	// 		GenreId = genre?.Id,
	// 		FileLocation = data.GetValueOrDefault("Location").AsString(),
	// 		FileSize = data.GetValueOrDefault("File Size").AsInt(),
	// 		FileTypeId = fileType?.Id,
	// 		Length = data.GetValueOrDefault("Total Time").AsInt(),
	// 		BitRate = data.GetValueOrDefault("Bit Rate").AsInt(),
	// 		SampleRate = data.GetValueOrDefault("Sample Rate").AsInt(),
	// 		// TODO: Add Date added, Last play date? (these aren't in the db yet)
	// 	};
	// 	
	// 	// TODO Add AlbumTrackConnection
	// 	
	// 	this.db.Tracks.Add(result);
	// 	this.db.SaveChanges();
	// 	
	// 	// Return the actual inserted track
	// 	return this.db.Tracks.FirstOrDefault(item => item.Title.Equals(title.ToString().Trim()));
	// }
	//
	// private Genre FindOrCreateGenre(string label) {
	// 	Genre? found = this.db.Genres.FirstOrDefault(item => item.Label.Equals(label));
	// 	if (found is not null) {
	// 		return found;
	// 	}
	//
	// 	var genre = new Genre { Label = label };
	// 	this.db.Genres.Add(genre);
	// 	
	// 	return genre;
	// }
	//
	// private FileType FindOrCreateFileType(string label) {
	// 	FileType? found = this.db.FileTypes.FirstOrDefault(item => item.Label.Equals(label));
	// 	if (found is not null) {
	// 		return found;
	// 	}
	// 	
	// 	var fileType = new FileType { Label = label };
	// 	this.db.FileTypes.Add(fileType);
	// 	
	// 	return fileType;
	// }
	
	
}