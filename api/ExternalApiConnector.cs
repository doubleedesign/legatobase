using System.Text.Json.Nodes;
using FluentHttpClient;
using Legatobase.Common;
namespace Legatobase.API;

public abstract class ExternalApiConnector {
	private readonly string _baseUrl;
	private readonly HttpClient _client;
	private Dictionary<string, string> _requestHeaders;
	protected int? RequestFrequency { get; set; }
	private long? LastRequestTimeStamp { get; set; }

	private string UserAgent {
		get => $"Legatobase/{Config.GetVersion()}({Config.GetUrl()})";
	}

	protected ExternalApiConnector(string baseUrl, Dictionary<string, string>? commonRequestHeaders = null) {
		this._baseUrl = baseUrl;
		this._client = new HttpClient();
		this.RequestFrequency = null;
		this.LastRequestTimeStamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
		
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
		bool throttled = this.RequestFrequency > 0 && this.LastRequestTimeStamp.HasValue && (DateTimeOffset.UtcNow.ToUnixTimeSeconds() - this.LastRequestTimeStamp.Value) < this.RequestFrequency;

		if (throttled) {
			var timeSinceLastRequest = DateTimeOffset.UtcNow.ToUnixTimeSeconds() - this.LastRequestTimeStamp;
			var timeToWait = this.RequestFrequency - timeSinceLastRequest;
			if (timeToWait > 0) {
				Logger.Warning($"Throttling request to {this._baseUrl}/{path} for {timeToWait}ms to avoid exceeding rate of 1 request per {this.RequestFrequency}ms", logToFile: true);
				await Task.Delay((int)timeToWait);
			}
		}
		
		HttpResponseMessage response = await this._client
			.UsingRoute($"{this._baseUrl}/{path}")
			.WithQueryParameters((IEnumerable<KeyValuePair<string, string?>>)(queryParams ?? empty))
			.WithHeaders(mergedheaders)
			.GetAsync();
		
		Logger.Info($"Request made to {this._baseUrl}/{path}", logToFile: true);
		this.LastRequestTimeStamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
		
		if (!response.IsSuccessStatusCode) {
			// If it is a 500-level error, retry the request
			if ((int)response.StatusCode >= 500 && (int)response.StatusCode < 600) {
				return await this.RetryGet(path, queryParams, headers);
			}

			Logger.Error($"Request to {this._baseUrl}/{path} failed with response: \n \t \t \t \t \t \t \t {(int)response.StatusCode} {response.ReasonPhrase}", logToFile: true);
			throw new HttpRequestException(response.ReasonPhrase);
		}

		Logger.Success($"Request to {path} succeeded with response: {(int)response.StatusCode}", logToFile: true);
		
		return await response.GetResultObject();
	}
	
	private async Task<JsonObject> RetryGet(string path, Dictionary<string, string>? queryParams = null, Dictionary<string, string>? headers = null) {
		Logger.Info($"Waiting {this.RequestFrequency ?? 1000}ms before retrying request to {this._baseUrl}/{path}", logToFile: true);
		await Task.Delay(this.RequestFrequency ?? 1000);
		
		return await this.Get(path, queryParams, headers);
	}
}