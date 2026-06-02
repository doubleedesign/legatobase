using FluentHttpClient;
namespace Legatobase.API;

public abstract class ExternalApiConnector {
	private readonly string _baseUrl;
	private readonly string? _apiKey;
	private readonly HttpClient _client;

	private string UserAgent {
		get => $"Legatobase/{Config.GetVersion()}({Config.GetUrl()})";
	}

	protected ExternalApiConnector(string baseUrl) {
		this._baseUrl = baseUrl;
		this._client = new HttpClient();
	}

	protected ExternalApiConnector(string baseUrl, string apiKey) {
		this._baseUrl = baseUrl;
		this._apiKey = apiKey;
		this._client = new HttpClient();
	}

	protected async Task<HttpResponseMessage> Get(string path, Dictionary<string, string>? queryParams = null, Dictionary<string, string>? headers = null) {
		var empty = new Dictionary<string, string>();
		var mergedheaders = new Dictionary<string, string>(headers ?? empty);
		mergedheaders.Add("User-Agent", UserAgent);
		mergedheaders.Add("Content-Type", "application/json");

		return await this._client
			.UsingRoute($"{this._baseUrl}/{path}")
			.WithQueryParameters((IEnumerable<KeyValuePair<string, string?>>)(queryParams ?? empty))
			.WithHeaders(mergedheaders)
			.GetAsync();
	}
}