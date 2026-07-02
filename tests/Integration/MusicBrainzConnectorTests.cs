using Legatobase.API;
using Legatobase.API.Datasources;

namespace LegatoBase.Tests.Integration;

public class MusicBrainzConnectorTests {
	private readonly MusicBrainzConnector instance;
	
	public MusicBrainzConnectorTests() {
		this.instance = new MusicBrainzConnector();
	}
	
	[Fact]
	public async Task GetArtistByName() {
		var result = await this.instance.GetArtistByName("Shania Twain");
	
		Assert.Equal("faabb55d-3c9e-4c23-8779-732ac2ee2c0d", result["MBID"]);
		Assert.Equal("Shania Twain", result["Name"]);
		Assert.Equal("CA", result["Country"]);
		Assert.Equal("1965-08-28", result["BirthDate"]);
	}
	
		
	[Fact]
	public async Task GetArtistByMbid() {
		var result = await this.instance.GetByMbid("artist", "faabb55d-3c9e-4c23-8779-732ac2ee2c0d");
	
		Assert.Equal("Shania Twain", result["Name"]);
		Assert.Equal("CA", result["Country"]);
		Assert.Equal("1965-08-28", result["BirthDate"]);
	}

	[Fact]
	public async Task GetReleasesByBarcode() {
		var result = await this.instance.GetReleasesByBarcode("731452823328");
		
		Assert.Contains(result, r => r.Title.Contains("Come On Over", StringComparison.OrdinalIgnoreCase) && r.ReleaseArtist == "Shania Twain");
	}
}