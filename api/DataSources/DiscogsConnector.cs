using System.Net;
using Legatobase.Common;

namespace Legatobase.API.Datasources;

public class DiscogsConnector : ExternalApiConnector {
	
	public DiscogsConnector() : base("https://api.discogs.com", GetAuthHeaders()) { }

	private static Dictionary<string, string>? GetAuthHeaders() {
		var creds = Config.GetCredential("Discogs");
		if (string.IsNullOrEmpty(creds?.UserName) || string.IsNullOrEmpty(creds?.Password)) {
			return null;
		}

		// The Discogs consumer key and secret are stored as username and password respectively, just because that's how Windows Credential Manager works
		return new Dictionary<string, string> {
			["Authorization"] = $"key={creds.UserName}, secret={creds.Password}"
		};
	}

	public async Task<HttpResponseMessage> Search(string entityType, string searchTerm) {
		var args = new Dictionary<string, string> {
			{ "q", Uri.EscapeDataString(searchTerm) },
			{ "type", entityType.ToLower() }
		};

		return await this.Get("database/search", args);
	}
}