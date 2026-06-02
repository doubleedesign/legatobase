namespace Legatobase.API.Datasources;

public class MusicBrainzConnector : ExternalApiConnector {

	public MusicBrainzConnector() : base("https://musicbrainz.org/ws/2") {
	}

	public async Task<HttpResponseMessage> Search(string entityType, string searchTerm) {
		var args = new Dictionary<string, string> {
			{ "query", Uri.EscapeDataString(searchTerm) },
			{ "limit", 1.ToString() }
		};

		return await this.Get(entityType.ToLower(), args);
	}

	public async Task<HttpResponseMessage> GetByMbid(string entityType, string mbid) {
		return await this.Get($"{entityType}/{mbid}");
	}
	
	protected new async Task<HttpResponseMessage> Get(string path, Dictionary<string, string>? queryParams = null, Dictionary<string, string>? headers = null) {
		var updatedQueryParams = queryParams ?? new Dictionary<string, string>();
		updatedQueryParams.Add("fmt", "json");

		return await base.Get(path, updatedQueryParams, headers);
	}
}