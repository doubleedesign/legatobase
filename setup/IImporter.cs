using Microsoft.EntityFrameworkCore;

namespace setup;

public interface IImporter {

	void ValidateFile() { }

	void Import(DbContext db) { }
}