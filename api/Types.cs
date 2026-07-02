
global using SimpleDataObject = System.Collections.Generic.Dictionary<string, OneOf.OneOf<string,int>>;

namespace Legatobase.API;

/// <summary>
/// A minimal search result object for a release, where a search is likely to return more than one result, such as searching by barcode.
/// This object type is intended to present just enough information for the user to select the intended release
/// and be used to fetch the remaining data for the selected release.
/// </summary>
public record ReleaseSearchResult {
	public string Title { get; init; } = "";
	public string ReleaseArtist { get; init; } = "";
	/// <summary>
	/// Platform-specific ID (e.g., MBID), used to get the rest of the data once this search result is selected to be actioned
	/// </summary>
	public string IdOnSourcePlatform { get; init; } = "";
}