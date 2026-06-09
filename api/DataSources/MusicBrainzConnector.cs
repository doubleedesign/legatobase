using System.Text.Json.Nodes;
using Humanizer;
using Legatobase.Common;

namespace Legatobase.API.Datasources;

public class MusicBrainzConnector : ExternalApiConnector, IExternalApi {

	public MusicBrainzConnector() : base("https://musicbrainz.org/ws/2") {
	}

	public async Task<JsonObject> Search(string entityType, string searchTerm) {
		var args = new Dictionary<string, string> {
			{ "query", Uri.EscapeDataString(searchTerm).Trim() },
			{ "limit", 1.ToString() }
		};

		JsonObject results = await this.Get(entityType.ToLower(), args);
		var firstResult = results[entityType.ToLower().Pluralize()]?.AsArray().First();
		if (firstResult == null) {
			throw new KeyNotFoundException($"No results found in MusicBrainz search for \"{searchTerm}\"");
		}
		
		return firstResult.AsObject();
	}
	
	private new async Task<JsonObject> Get(string path, Dictionary<string, string>? queryParams = null, Dictionary<string, string>? headers = null) {
		var updatedQueryParams = queryParams ?? new Dictionary<string, string>();
		updatedQueryParams.Add("fmt", "json");

		return await base.Get(path, updatedQueryParams, headers);
	}
	
	public async Task<JsonObject> GetByMbid(string entityType, string mbid) {
		// TODO: Some kind of reusable formatter that ensures getting something by MBID returns the same data as its dedicated GetBy/search
		return await this.Get($"{entityType.Trim()}/{mbid.Trim()}");
	}
	
	public async Task<SimpleDataObject> GetArtistByName(string name) {
		JsonObject searchResult = await this.Search("artist", name.Trim());

		var returnedName = searchResult["title"]?.GetValue<string>();
		var returnedId = searchResult["id"]?.GetValue<string>();
		if (returnedName is null || !string.Equals(name, returnedName, StringComparison.OrdinalIgnoreCase)) {
			throw new KeyNotFoundException($"Artist \"{name}\" not found in Discogs search, or was not the first result");
		}
		if (returnedId is null) {
			throw new KeyNotFoundException($"ID for artist \"{name}\" not found in Discogs search");
		}
		
		SimpleDataObject result = new SimpleDataObject {
			{ "Name", returnedName },
			{ "MBID", returnedId },
			// TODO: Hometown should be Origin for groups
			{ "Hometown", searchResult["begin-area"]?["name"]?.GetValue<string>() ?? null },
			{ "Country", searchResult["area"]?["country"]?.GetValue<string>() ?? null },
			// TODO: These need to be datetimes, and are also for individuals only
			{ "BirthDate", searchResult["life-span"]?["begin"]?.GetValue<string>() ?? null },
			{ "DeathDate", searchResult["life-span"]?["end"]?.GetValue<string>() ?? null },
			// TODO: FoundedDate and EndedDate for groups only (also need to be added to db)
		};

		return result;
	}
}