using System.Net;
using System.Text.Json.Nodes;
using FluentHttpClient;
using Legatobase.Common;
namespace Legatobase.API;

public abstract class ExternalApiConnector {
	private readonly string _baseUrl;
	protected readonly HttpClient Client;
	private Dictionary<string, string> _requestHeaders;
	protected int? RequestFrequency { get; set; }
	protected int? MaxRetries { get; set; } = 5;
	protected int? RetriesAttempted { get; set; } = 0;
	private long? LastRequestTimeStamp { get; set; }

	private string UserAgent {
		get => $"Legatobase/{Config.GetVersion()}({Config.GetUrl()})";
	}

	protected ExternalApiConnector(string baseUrl, Dictionary<string, string>? commonRequestHeaders = null, HttpClient? client = null) {
		this._baseUrl = baseUrl;
		this._client = client ?? new HttpClient();
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
		await this.MaybeThrottleRequest();
		
		HttpResponseMessage response = await this.DoGet(path, queryParams, headers);
		if (response.IsSuccessStatusCode) {
			Logger.Success($"Request to {this._baseUrl}/{path} succeeded with response: {(int)response.StatusCode}", logToFile: true);
			return await response.GetResultObject();
		}
		
		if (response.IsRetryable()) {
			return await this.RetryGet(path, queryParams, headers);
		}
		
		Logger.Error($"Request to {this._baseUrl}/{path} failed with response: \n \t \t \t \t \t \t \t {(int)response.StatusCode} {response.ReasonPhrase}", logToFile: true);
		throw new HttpRequestException(response.ReasonPhrase);
	}
	
	private async Task MaybeThrottleRequest() {
		bool throttle = this.WaitBetweenRequests > 0 && this.LastRequestTimeStamp.HasValue && (DateTimeOffset.UtcNow.ToUnixTimeSeconds() - this.LastRequestTimeStamp.Value) < this.WaitBetweenRequests;
		if(!throttle) {
			return;
		}
		
		var timeSinceLastRequest = DateTimeOffset.UtcNow.ToUnixTimeSeconds() - this.LastRequestTimeStamp;
		var timeToWait = this.WaitBetweenRequests - timeSinceLastRequest;
		if (timeToWait > 0) {
			Logger.Warning($"Throttling request for {timeToWait}ms to avoid exceeding rate of 1 request per {this.WaitBetweenRequests}ms", logToFile: true);
			await Task.Delay((int)timeToWait);
		}
	}
	
	private async Task<HttpResponseMessage> DoGet(string path, Dictionary<string, string>? queryParams = null, Dictionary<string, string>? headers = null) {
		this.LastRequestTimeStamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
		
		var empty = new Dictionary<string, string>();
		var mergedheaders = headers != null ? this._requestHeaders.MergeWith<string, string>(headers) : this._requestHeaders;
		
		HttpResponseMessage response = await this._client
			.UsingRoute($"{this._baseUrl}/{path}")
			.WithQueryParameters((IEnumerable<KeyValuePair<string, string?>>)(queryParams ?? empty))
			.WithHeaders(mergedheaders)
			.GetAsync();
		
		Logger.Info($"Request made to {this._baseUrl}/{path}", logToFile: true);
		
		return response;
	}

	private async Task<JsonObject> RetryGet(string path, Dictionary<string, string>? queryParams = null, Dictionary<string, string>? headers = null) {
		while (this.RetriesAttempted < this.MaxRetries) {
			Logger.Info($"Waiting {this.RequestFrequency ?? 1000}ms before retrying request to {this._baseUrl}/{path}", logToFile: true);
			await Task.Delay(this.RequestFrequency ?? 1000);
			var result = await this.DoGet(path, queryParams, headers);
			this.RetriesAttempted++;
			
			if (result.IsSuccessStatusCode) {
				return await result.GetResultObject();
			}
		}
		
		throw new HttpRequestException($"Request to {this._baseUrl}/{path} failed after {this.RetriesAttempted} retries");
	}
}