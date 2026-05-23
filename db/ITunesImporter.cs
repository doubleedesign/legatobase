using PListNet;
using PListNet.Nodes;

namespace db;

public class ITunesImporter : IImporter {
	public string? FilePath { get; private set; }
	private Context db;
	
	public ITunesImporter(string path, Context db) {
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
			using (var fileStream = File.OpenRead(trimmed)) {
				var node = PList.Load(fileStream);
            
				// Check if root is a Dictionary
				if (node is DictionaryNode) {
					this.FilePath = trimmed;
					Logger.Success("XML file is valid");
				}
			}
		}
		catch (Exception ex) {
			Logger.Error(ex.Message);
			Environment.Exit(1);
		}
	}
	
	public void Import() {
		throw new NotImplementedException();
	}
}