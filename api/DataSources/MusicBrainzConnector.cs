using System.Text.Json.Nodes;
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

		return await this.Get(entityType.ToLower(), args);
	}
	
	private new async Task<JsonObject> Get(string path, Dictionary<string, string>? queryParams = null, Dictionary<string, string>? headers = null) {
		var updatedQueryParams = queryParams ?? new Dictionary<string, string>();
		updatedQueryParams.Add("fmt", "json");

		return await base.Get(path, updatedQueryParams, headers);
	}
	
	public async Task<JsonObject> GetByMbid(string entityType, string mbid) {
		return await this.Get($"{entityType.Trim()}/{mbid.Trim()}");
	}
	
	public async Task<SimpleDataObject> GetArtistByName(string name) {
		//return await this.Search("artist", name.Trim());

		return new SimpleDataObject();
	}
}