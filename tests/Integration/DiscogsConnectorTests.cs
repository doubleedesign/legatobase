using System.Net;
using System.Text.Json.Nodes;
using FluentHttpClient;
using Legatobase.API.Datasources;
namespace LegatoBase.Tests.Integration;

public class DiscogsConnectorTests {
	private readonly DiscogsConnector instance;
	
	public DiscogsConnectorTests() {
		this.instance = new DiscogsConnector();
	}
	
	[Fact]
	public async Task GetArtistByName() {
		var result = await this.instance.Search("artist", "Shania Twain");
		Assert.Equal(HttpStatusCode.OK, result.StatusCode);
		
		JsonObject searchResults = await result.ReadJsonObjectAsync() ?? throw new InvalidDataException("Response was not a valid JSON object");
		JsonObject data = searchResults["results"]?.AsArray()?.FirstOrDefault()?.AsObject() ?? throw new InvalidDataException("No artists found in search results");
		data.TryGetPropertyValue("name", out var name);
		data.TryGetPropertyValue("id", out var id);

		Assert.Equal("Shania Twain", name!.ToString());
		Assert.Equal("id", id!.ToString());
	}
}