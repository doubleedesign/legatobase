namespace Legatobase.Setup;
using Microsoft.EntityFrameworkCore.Design;
using Humanizer;

/**
 * Custom handling of table names -> EF Core entity names
 * This needs to be added in ConfigureDesignTimeServices - EF Core scaffolding will pick it up from there
 */
public class TableEntityMapper : IPluralizer {
	public string Pluralize(string name) {
		return name.Pluralize();
	}

	public string Singularize(string name) {
		if (name == "ArtistsTracks") return "ArtistTrackConnection";
		if (name == "AlbumsTracks") return "AlbumTrackConnection";

		return name.Singularize();
	}
}