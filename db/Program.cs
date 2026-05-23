using System.Text;
using Spectre.Console;

namespace db;

class Program {
	static void Main(string[] args) {
		Console.OutputEncoding = Encoding.UTF8;
		using var db = new Context();
		
		var action = AnsiConsole.Prompt(
			new SelectionPrompt<string>()
				.Title("What would you like to do?")
				.AddChoices("Import iTunes library", "Exit"));

		switch (action) {
			case "Import iTunes library":
				Import(db);
				break;
			default:
				Environment.Exit(0);
				break;
		}
	}

	static void Import(Context db) {
		var filePath = "";
		
		while (string.IsNullOrEmpty(filePath)) {
			filePath = Logger.Input("Enter the path to your iTunes library XML file:");
		}
		
		var importer = new ITunesImporter(filePath, db);
	}
}