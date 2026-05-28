using Microsoft.EntityFrameworkCore;
using Plist.Kit;
using Plist.Kit.Core.Types;

namespace setup;

public class ITunesImporter : IImporter {
	public string? FilePath { get; private set; }
	private DbContext db;
	
	public ITunesImporter(string path, DbContext db) {
		this.db = db;
		this.ValidateFile(path);
	}

	private void ValidateFile(string path) {
		// Strip quotation marks and whitespace from the start and end of the string
		var trimmed = path.Trim().Replace("\"", "");
		
		if (!File.Exists(trimmed)) {
			throw new FileNotFoundException(trimmed);
		}

		try {
			var doc = PlistDocument.Load(trimmed);
			// TODO Validation
		}
		catch (Exception ex) {
			Logger.Error(ex.Message);
			Environment.Exit(1);
		}
	}
	
	public void Import() {
		if (String.IsNullOrEmpty(this.FilePath)) {
			throw new FileNotFoundException();
		}
		
		try {
			var doc = PlistDocument.Load(this.FilePath);
			// TODO: Import logic
		}
		catch (Exception ex) {
			Logger.Error(ex.Message);
			Environment.Exit(1);
		}
		
	}
}