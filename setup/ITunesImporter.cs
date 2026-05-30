using core;
using Microsoft.EntityFrameworkCore;
using Plist.Kit;
using Plist.Kit.Core;
using Plist.Kit.Core.Types;

namespace setup;

public class ITunesImporter : IImporter {
	public string? FilePath { get; private set; }
	private DbContext db;
	
	public ITunesImporter(string path) {
		this.db = new LbContext(new DbContextOptions<LbContextBase>());
		this.ValidateFile(path);
	}

	private void ValidateFile(string path) {
		// Strip quotation marks and whitespace from the start and end of the string
		var trimmed = path.Trim().Replace("\"", "");
		
		if (!File.Exists(trimmed)) {
			throw new FileNotFoundException(trimmed);
		}

		try {
			var doc = PlistDocument.Load(trimmed).ToDictionary();
			var keys = doc.Keys;
			if (!keys.Contains("Library Persistent ID") || !keys.Contains("Tracks") || !keys.Contains("Music Folder")) {
				throw new InvalidDataException("The provided file does not appear to be a valid iTunes library XML file.");
			}
			
			this.FilePath = trimmed;
		}
		catch (Exception ex) {
			Logger.Error(ex.Message);
			Environment.Exit(1);
		}
	}
	
	public void Import() {
		if (String.IsNullOrEmpty(this.FilePath)) {
			throw new FileNotFoundException();
		}
		
		try {
			var doc = PlistDocument.Load(this.FilePath);
			var tracks = doc.Get("Tracks");
			if (tracks is not PlistDictionary) {
				throw new InvalidDataException("Could not find valid track data in the provided XML file.");
			}
			
			this.FilterTracks((PlistDictionary)tracks);
			
		}
		catch (Exception ex) {
			Logger.Error(ex.Message);
			Logger.Error(ex.StackTrace);
			Environment.Exit(1);
		}
	}
	
	private void FilterTracks(PlistDictionary tracks) {
		Logger.Info($"{tracks.Count} items found");
		
		foreach (var key in tracks.Keys) {
			var data = tracks[key].ToNative() as Dictionary<string, object>;
			if (data == null || !this.IncludeItem(data)) {
				tracks.Remove(key);
			}
		}
		
		Logger.Info($"{tracks.Count} tracks found after filtering");
	}

	private bool IncludeItem(Dictionary<string, object> item) {
		// Do not include TV shows or movies, as indicated by the presence and value of the "TV Show" and "Movie" keys
		// (this allows music videos, concert footage, etc. to be kept)
		item.TryGetValue("TV Show", out var isTvShow);
		if(isTvShow is bool tvShow && tvShow == true) {
			return false;
		}

		item.TryGetValue("Movie", out var isMovie);
		if (isMovie is bool movie && movie == true) {
			return false;
		}

		return true;
	}
}