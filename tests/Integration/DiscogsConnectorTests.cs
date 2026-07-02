using Legatobase.API.Datasources;
using Legatobase.Common;
using Xunit.Abstractions;

namespace LegatoBase.Tests.Integration;

public class DiscogsConnectorTests {
	private readonly ITestOutputHelper _testOutputHelper;
	private readonly DiscogsConnector instance;
	
	public DiscogsConnectorTests(ITestOutputHelper testOutputHelper) {
		this._testOutputHelper = testOutputHelper;
		this.instance = new DiscogsConnector();
	}
	
	[Fact]
	public async Task GetArtistByName() {
		var result = await this.instance.GetArtistByName("Shania Twain");

		Assert.Equal("Shania Twain", result["Name"]);
		Assert.Equal(130060, result["DID"]);
		Assert.False(string.IsNullOrWhiteSpace(result["Profile"].AsString()));
	}
	
	[Fact]
	public async Task GetReleasesByBarcode() {
		var result = await this.instance.GetReleasesByBarcode("731452823328");
		
		Assert.Contains(result, r => r.Title.Contains("Come On Over", StringComparison.OrdinalIgnoreCase) && r.ReleaseArtist == "Shania Twain");
	}
}