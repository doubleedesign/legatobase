using System.Net;
using System.Text.Json.Nodes;
using FluentHttpClient;
using Legatobase.API.Datasources;

namespace LegatoBase.Tests.Integration;

public class MusicBrainzConnectorTests {
	private readonly MusicBrainzConnector instance;
	
	public MusicBrainzConnectorTests() {
		this.instance = new MusicBrainzConnector();
	}
	
	[Fact]
	public async Task GetArtistByMbid() {
		var result = await this.instance.GetByMbid("artist", "faabb55d-3c9e-4c23-8779-732ac2ee2c0d");
		Assert.Equal(HttpStatusCode.OK, result.StatusCode);

		JsonObject data = await result.ReadJsonObjectAsync() ?? throw new InvalidDataException("Response was not a valid JSON object");
		data.TryGetPropertyValue("name", out var name);
		Assert.Equal("Shania Twain", name!.ToString());
	}

	[Fact]
	public async Task GetArtistByName() {
		var result = await this.instance.Search("artist", "Shania Twain");
		Assert.Equal(HttpStatusCode.OK, result.StatusCode);
		
		JsonObject searchResults = await result.ReadJsonObjectAsync() ?? throw new InvalidDataException("Response was not a valid JSON object");
		JsonObject data = searchResults["artists"]?.AsArray()?.FirstOrDefault()?.AsObject() ?? throw new InvalidDataException("No artists found in search results");
		data.TryGetPropertyValue("name", out var name);
		data.TryGetPropertyValue("type", out var type);
		data.TryGetPropertyValue("country", out var country);
		data.TryGetPropertyValue("id", out var id);

		Assert.Equal("Shania Twain", name!.ToString());
		Assert.Equal("Person", type!.ToString());
		Assert.Equal("CA", country!.ToString());
		Assert.Equal("faabb55d-3c9e-4c23-8779-732ac2ee2c0d", id!.ToString());
	}
}