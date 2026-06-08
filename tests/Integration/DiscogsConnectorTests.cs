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
}