using System.Text.Json.Nodes;
using FluentHttpClient;
using Legatobase.Common;
namespace Legatobase.API;

public abstract class ExternalApiConnector {
	private readonly string _baseUrl;
	private readonly HttpClient _client;
	private Dictionary<string, string> _requestHeaders;

	private string UserAgent {
		get => $"Legatobase/{Config.GetVersion()}({Config.GetUrl()})";
	}

	protected ExternalApiConnector(string baseUrl, Dictionary<string, string>? commonRequestHeaders = null) {
		this._baseUrl = baseUrl;
		this._client = new HttpClient();
		
		// Set some request headers that are the same for all requests for all APIs
		this._requestHeaders = new Dictionary<string, string> {
			{ "User-Agent", UserAgent },
			{ "Content-Type", "application/json" }
		};

		// Allow child classes to add their own "always use" headers at instantiation
		if (commonRequestHeaders != null) {
			this._requestHeaders = this._requestHeaders.MergeWith<string, string>(commonRequestHeaders);
		}
	}

	/// <summary>
	/// Base method for GET requests.
	/// </summary>
	/// <param name="path">The path to append to the base URL for the request</param>
	/// <param name="queryParams">Any URL parameters to add to the request</param>
	/// <param name="headers">Request headers to add to the class-wide ones already instantiated</param>
	/// <returns></returns>
	protected async Task<JsonObject> Get(string path, Dictionary<string, string>? queryParams = null, Dictionary<string, string>? headers = null) {
		var empty = new Dictionary<string, string>();
		var mergedheaders = headers != null ? this._requestHeaders.MergeWith<string, string>(headers) : this._requestHeaders;

		HttpResponseMessage result = await this._client
			.UsingRoute($"{this._baseUrl}/{path}")
			.WithQueryParameters((IEnumerable<KeyValuePair<string, string?>>)(queryParams ?? empty))
			.WithHeaders(mergedheaders)
			.GetAsync();

		return await result.GetResultObject();
	} 
}