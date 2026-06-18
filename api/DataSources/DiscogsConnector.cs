using System.Text.Json.Nodes;
using Legatobase.Common;
namespace Legatobase.API.Datasources;

public class DiscogsConnector : ExternalApiConnector, IExternalApi {
	
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

	public async Task<JsonObject> Search(string entityType, string searchTerm) {
		var args = new Dictionary<string, string> {
			{ "q", searchTerm.Trim() }, // do not escape the string here, it causes double-escaping and thus unexpected results
			{ "type", entityType.ToLower().Trim() }
		};

		var results = (await this.Get("database/search", args)).Pick(["results"]);
		if (results == null) {
			throw new KeyNotFoundException($"No results found in Discogs search for \"{searchTerm}\"");
		}

		// FIXME: This assumes the first result will be the correct one - it should probably keep going if not
		var firstResult = results.First().Value?.AsArray().First() ?? null;
		if (firstResult == null) {
			throw new KeyNotFoundException($"No results found in Discogs search for \"{searchTerm}\"");
		}

		return firstResult.AsObject();
	}


	public async Task<SimpleDataObject> GetArtistByName(string name) {
		JsonObject searchResult = await this.Search("artist", name);

		var returnedName = searchResult["title"]?.GetValue<string>();
		var returnedId = searchResult["id"]?.GetValue<int>();
		if (returnedName is null || !string.Equals(name, returnedName, StringComparison.OrdinalIgnoreCase)) {
			throw new KeyNotFoundException($"Artist \"{name}\" not found in Discogs search, or was not the first result");
		}
		if (returnedId is null) {
			throw new KeyNotFoundException($"ID for artist \"{name}\" not found in Discogs search");
		}

		SimpleDataObject result = new SimpleDataObject {
			{ "Name", returnedName },
			{ "DID", (int)returnedId },
		};

		try {
			JsonObject profileResult = await this.Get($"artists/{returnedId}");
			var profileText = profileResult["profile"]?.GetValue<string>() ?? "";
			result.Add("Profile", profileText);
			
			// TODO: This search query also gets groups (e.g., searching for Stevie Nicks will get you Fleetwood Mac.
			//		 Could be a good way to handle associating individuals to groups.
		}
		catch (HttpRequestException e) {
			Logger.Warning($"Error {e.StatusCode.ToString()}", $"Failed to get profile for artist \"{name}\" with ID {returnedId} from Discogs API. Error message: {e.Message}");
		}

		return result;
	}
	
	public Task<List<SimpleDataObject>> GetReleasesByBarcode(string barcode) {
		throw new NotImplementedException();
	}
}