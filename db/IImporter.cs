namespace db;

public interface IImporter {

	void ValidateFile() { }

	void Import(Context db) { }
}