using Microsoft.EntityFrameworkCore;

namespace db;

public interface IImporter {

	void ValidateFile() { }

	void Import(DbContext db) { }
}