using System.Text.Json.Nodes;

namespace Legatobase.API;

/// <summary>
/// Interface to ensure that classes for third-party API integrations have the same core methods for consistency and predictability.
/// For example, GetArtistByName should take the same arguments and have the same return type regardless of which API we're getting data from.
/// </summary>
public interface IExternalApi {
	public Task<JsonObject> Search(string entityType, string searchTerm);
	
	public Task<SimpleDataObject> GetArtistByName(string name);
}