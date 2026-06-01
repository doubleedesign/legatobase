using core;
using Microsoft.EntityFrameworkCore;

namespace api;

public class TrackImporter {
	private DbContext db;
	
	public TrackImporter(LbContext db) {
		this.db = db;
	}

	public Artist FindOrCreateArtist(string name) {

	}
}