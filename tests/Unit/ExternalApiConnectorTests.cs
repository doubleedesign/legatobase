using Legatobase.API;
using System.Net;
using RichardSzalay.MockHttp;
using Shouldly;
using System.Text.Json.Nodes;

namespace LegatoBase.Tests.Unit;

class MockApiConnector : ExternalApiConnector {
	public MockApiConnector(HttpClient client, int? waitBetweenRequests = null)
		: base("https://example.com", client: client, waitBetweenRequests: waitBetweenRequests) { }
	
	public new async Task<JsonObject> Get(string path, Dictionary<string, string>? queryParams = null, Dictionary<string, string>? headers = null) {
		return await base.Get(path, queryParams, headers);
	}
}

public class ExternalApiConnectorTests {
	private readonly ITestOutputHelper _testOutputHelper;
	private readonly MockApiConnector instance;
	private readonly MockHttpMessageHandler mockHttp;
	private readonly JsonObject simpleJsonResponse = new JsonObject { ["some key"] = "some value" };
	
	public ExternalApiConnectorTests(ITestOutputHelper testOutputHelper) {
		this._testOutputHelper = testOutputHelper;
		this.mockHttp = new MockHttpMessageHandler();
		this.instance = new MockApiConnector(client: new HttpClient(mockHttp));
	}

	[Fact]
	public async Task IncludesDefaultUserAgentHeaderInRequest() {
		HttpRequestMessage? captured = null;

		this.mockHttp.When("https://example.com/test")
			.Respond(req => {
				captured = req;
				return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(simpleJsonResponse.ToString()) };
			});

		await this.instance.Get("test");

		captured!.Headers.UserAgent.ToString().ShouldStartWith("Legatobase/");
	}
	
	[Fact]
	public async Task IncludesBasicHeadersOnlyByDefault() {
		HttpRequestMessage? captured = null;

		this.mockHttp.When("https://example.com/test")
			.Respond(req => {
				captured = req;
				return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(simpleJsonResponse.ToString()) };
			});

		await this.instance.Get("test");
		
		this._testOutputHelper.WriteLine($"Captured headers: {string.Join(", ", captured.Headers.Select(h => $"{h.Key}: {string.Join(", ", h.Value)}"))}");

		captured.Headers.Count().ShouldBe(2);
		captured.Headers.Contains("User-Agent").ShouldBeTrue();
		captured.Headers.Contains("Accept").ShouldBeTrue();

	}
	
	[Fact]
	public async Task IncludesAdditionalHeadersIfProvided() {
		HttpRequestMessage? captured = null;

		this.mockHttp.When("https://example.com/test")
			.Respond(req => {
				captured = req;
				return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(simpleJsonResponse.ToString()) };
			});

		var additionalHeaders = new Dictionary<string, string> {
			{ "X-Test-Header", "TestValue" }
		};

		await this.instance.Get("test", headers: additionalHeaders);

		captured.Headers.Count().ShouldBeGreaterThan(1);
		captured.Headers.Contains("X-Test-Header").ShouldBeTrue();
		captured.Headers.GetValues("X-Test-Header").First().ShouldBe("TestValue");
	}
	
	[Fact]
	public async Task ReturnsJsonObjectOnSuccess() {
		this.mockHttp.When("https://example.com/test").Respond("application/json", simpleJsonResponse.ToString());
		
		var result = await this.instance.Get("test");
		
		result.ToString().ShouldBe(this.simpleJsonResponse.ToString());
	}
	
	[Fact]
	public async Task ThrottlesWhenWaitIsSet() {
		var waitTime = 500;
		var instanceWithWait = new MockApiConnector(client: new HttpClient(mockHttp), waitBetweenRequests: waitTime);
		
		var requestCount = 0;
		this.mockHttp.When("https://example.com/test")
			.Respond(req => {
				requestCount++;
				return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(simpleJsonResponse.ToString()) };
			});

		var stopwatch = System.Diagnostics.Stopwatch.StartNew();
		
		await instanceWithWait.Get("test");
		await instanceWithWait.Get("test");
		await instanceWithWait.Get("test");
		await instanceWithWait.Get("test");
		
		stopwatch.Stop();
		
		requestCount.ShouldBe(4);
		stopwatch.ElapsedMilliseconds.ShouldBeGreaterThanOrEqualTo(waitTime * 3);
	}
	
	[Fact]
	public async Task DoesNotThrottleWhenWaitIsNotSet() {
		var instanceWithoutWait = new MockApiConnector(client: new HttpClient(mockHttp), waitBetweenRequests: 0);
		
		var requestCount = 0;
		this.mockHttp.When("https://example.com/test")
			.Respond(req => {
				requestCount++;
				return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(simpleJsonResponse.ToString()) };
			});

		var stopwatch = System.Diagnostics.Stopwatch.StartNew();
		
		await instanceWithoutWait.Get("test");
		await instanceWithoutWait.Get("test");
		await instanceWithoutWait.Get("test");
		await instanceWithoutWait.Get("test");
		
		stopwatch.Stop();
		
		requestCount.ShouldBe(4);
		stopwatch.ElapsedMilliseconds.ShouldBeLessThan(100);
	}
	
	[Fact]
	public async Task ThrowsWhenNotFound() {
		this.mockHttp.When("https://example.com/test").Respond(HttpStatusCode.NotFound);
		
		await this.instance.Get("test")
			.ShouldThrowAsync<HttpRequestException>();
	}
	
	[Fact]
	public async Task DoesNotRetryWhenNotFound() {
		this.mockHttp.When("https://example.com/test").Respond(HttpStatusCode.NotFound);
		
		await this.instance.Get("test")
			.ShouldThrowAsync<HttpRequestException>();
		
		this.mockHttp.GetMatchCount(this.mockHttp.When("https://example.com/test")).ShouldBe(0);
	}

	[Fact]
	public async Task ThrowsWhenUnauthorised() {
		this.mockHttp.When("https://example.com/test").Respond(HttpStatusCode.Unauthorized);
		
		await this.instance.Get("test")
			.ShouldThrowAsync<HttpRequestException>();
	}
	
	[Fact]
	public async Task RetriesWhenTemporarilyUnavailable() {
		var responses = new Queue<HttpResponseMessage>(new[] {
			new HttpResponseMessage(HttpStatusCode.ServiceUnavailable),
			new HttpResponseMessage(HttpStatusCode.ServiceUnavailable),
			new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(simpleJsonResponse.ToString()) }
		});

		var request = this.mockHttp.When("https://example.com/test").Respond(_ => responses.Dequeue());

		await this.instance.Get("test");

		this.mockHttp.GetMatchCount(request).ShouldBe(3);
	}

	[Fact]
	public async Task ThrowsAfterMaxRetries() {
		var responses = new Queue<HttpResponseMessage>(new[] {
			new HttpResponseMessage(HttpStatusCode.ServiceUnavailable),
			new HttpResponseMessage(HttpStatusCode.ServiceUnavailable),
			new HttpResponseMessage(HttpStatusCode.ServiceUnavailable),
			new HttpResponseMessage(HttpStatusCode.ServiceUnavailable),
			new HttpResponseMessage(HttpStatusCode.ServiceUnavailable),
			new HttpResponseMessage(HttpStatusCode.ServiceUnavailable)
		});
		
		this.mockHttp.When("https://example.com/test").Respond(_ => responses.Dequeue());
		
		await this.instance.Get("test")
			.ShouldThrowAsync<HttpRequestException>();
	}
}
