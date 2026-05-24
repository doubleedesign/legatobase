using System.Text;
using Microsoft.EntityFrameworkCore;
using Spectre.Console;

namespace db;


class Program {
	static void Main(string[] args) {
		Console.OutputEncoding = Encoding.UTF8;
		var db = new DbCreator();
		db.Create();
		
		var action = AnsiConsole.Prompt(
			new SelectionPrompt<string>()
				.Title("What would you like to do?")
				.AddChoices("Import iTunes library", "Exit"));

		switch (action) {
			case "Import iTunes library":
				//Import();
				break;
			default:
				Environment.Exit(0);
				break;
		}
	}

	static void Import(DbContext db) {
		var filePath = "";
		
		while (string.IsNullOrEmpty(filePath)) {
			filePath = Logger.Input("Enter the path to your iTunes library XML file:");
		}
		
		var importer = new ITunesImporter(filePath, db);
	}
}