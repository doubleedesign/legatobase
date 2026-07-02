using System.Text.Json.Nodes;
using Humanizer;
using Legatobase.Common;

namespace Legatobase.API.Datasources;

public class MusicBrainzConnector : ExternalApiConnector, IExternalApi {

	public MusicBrainzConnector() : base("https://musicbrainz.org/ws/2") {
	}

	public async Task<JsonObject> Search(string entityType, string searchTerm) {
		var args = new Dictionary<string, string> {
			{ "query", Uri.EscapeDataString(searchTerm).Trim() }
		};

		return await this.Get(entityType.ToLower(), args);
	}

	/// <summary>
	/// Run a search and return the first result. For use when we're confident the first result will be the correct one.
	/// </summary>
	/// <param name="entityType"></param>
	/// <param name="searchTerm"></param>
	/// <returns></returns>
	/// <exception cref="KeyNotFoundException"></exception>
	private async Task<JsonObject> SearchOne(string entityType, string searchTerm) {
		var results = await this.Search(entityType, searchTerm);
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
	
	public async Task<SimpleDataObject> GetByMbid(string entityType, string mbid) {
		var result = await this.Get($"{entityType.Trim()}/{mbid.Trim()}");
		
		// TODO This needs to handle other entity types in a similar way
		return this.MapArtistData(result);
	}
	
	public async Task<SimpleDataObject> GetArtistByName(string name) {
		JsonObject searchResult = await this.SearchOne("artist", name.Trim());
		var returnedName = searchResult["name"]?.GetValue<string>();
		var returnedId = searchResult["id"]?.GetValue<string>();
		if (returnedName is null || !string.Equals(name, returnedName, StringComparison.OrdinalIgnoreCase)) {
			throw new KeyNotFoundException($"Artist \"{name}\" not found in Musicbrainz search, or was not the first result");
		}
		if (returnedId is null) {
			throw new KeyNotFoundException($"ID for artist \"{name}\" not found in Musicbrainz search");
		}
		
		return this.MapArtistData(searchResult);
	}

	public async Task<List<ReleaseSearchResult>> GetReleasesByBarcode(string barcode) {
		JsonObject searchResult = await this.Search("release", barcode);
		JsonArray? rawItems = searchResult["releases"]?.AsArray();
		if (rawItems == null) {
			return [];
		}
		
		List<ReleaseSearchResult> results = [];
		foreach (var rawItem in rawItems) {
			results.Add(new ReleaseSearchResult {
				Title = rawItem!["title"]?.GetValue<string>() ?? "",
				ReleaseArtist = rawItem["artist-credit"]?[0]?["name"]?.GetValue<string>() ?? "",
				IdOnSourcePlatform = rawItem["id"]?.GetValue<string>() ?? ""
			});
		}
		
		return results;
	}

	private SimpleDataObject MapArtistData(JsonObject data) {
		return new SimpleDataObject {
			{ "Name",  data["name"]?.GetValue<string>() },
			{ "MBID", data["id"]?.GetValue<string>() },
			// TODO: Hometown should be Origin for groups
			{ "Hometown", data["begin-area"]?["name"]?.GetValue<string>() ?? null },
			{ "Country", data["country"]?.GetValue<string>() ?? null },
			// TODO: These need to be datetimes, and are also for individuals only
			{ "BirthDate", data["life-span"]?["begin"]?.GetValue<string>() ?? null },
			{ "DeathDate", data["life-span"]?["end"]?.GetValue<string>() ?? null },
			// TODO: FoundedDate and EndedDate for groups only (also need to be added to db)
		};
	}
}