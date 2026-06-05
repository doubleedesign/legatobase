using Legatobase.Common;
using Legatobase.Core;

namespace Legatobase.API;

public abstract class Importer {
	protected LbContext db;
	
	public Importer(LbContext db) {
		this.db = db;
	}
	
	// protected Artist FindOrCreateArtist(string name) {
	// 	var found = this.db.Artists.FirstOrDefault(item => item.Name.Equals(name.Trim()));
	// 	if (found is not null) {
	// 		return found;
	// 	}
	// }
	//
	// protected Album FindOrCreateAlbum(string title, Artist artist) {
	// 	var found = this.db.Albums.FirstOrDefault(item => item.Title.Equals(title.Trim()) && item.ArtistId == artist.Id);
	// 	if (found is not null) {
	// 		return found;
	// 	}
	// }
}