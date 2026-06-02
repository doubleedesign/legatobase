using Microsoft.EntityFrameworkCore;

namespace Legatobase.Setup;

public interface IImporter {

	void ValidateFile() { }

	void Import(DbContext db) { }
}