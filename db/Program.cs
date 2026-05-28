using System.Diagnostics;
using System.Runtime.CompilerServices;
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
				.AddChoices("Import iTunes library", "Regenerate EF Core classes", "Exit"));

		switch (action) {
			case "Import iTunes library":
				//Import();
				break;
			case "Regenerate EF Core classes":
				RegenerateClasses(db.GetDbPath());
				break;
			default:
				Environment.Exit(0);
				break;
		}
	}

	static void Import(DbContext db) {
		var filePath = "C:/Users/leesa/Desktop/Library-2026-05-23.xml"; // FIXME: This is temporary, remove it
		
		while (string.IsNullOrEmpty(filePath)) {
			filePath = Logger.Input("Enter the path to your iTunes library XML file:");
		}
		
		var importer = new ITunesImporter(filePath, db);
	}

	static void RegenerateClasses(string dbFilePath) {
		Console.WriteLine("");
		Logger.Info("Regenerating Entity Framework classes");
		Logger.Warning("Existing classes will be replaced to match the current database schema");
		
		var process = new Process {
			StartInfo = new ProcessStartInfo {
				FileName = "dotnet",
				Arguments = $"ef dbcontext scaffold " +
				            $"\"Data Source={dbFilePath}\" Microsoft.EntityFrameworkCore.Sqlite " +
				            $"--context LbContext " +
							$"--force ",
				WorkingDirectory = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"..\..\..\")),
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				UseShellExecute = false
			}
		};
		
		process.Start();
		string output = process.StandardOutput.ReadToEnd();
		string error = process.StandardError.ReadToEnd();
		process.WaitForExit();
		
		Console.WriteLine("");
		if (!string.IsNullOrEmpty(output)) {
			if (output.Contains("failed") || output.Contains("error")) {
				Logger.Error(output);
			}
			else if (output.Contains("Build started")) {
				Logger.Info(output);
			}
			else {
				Logger.Success(output);
			}
		}
		if (!string.IsNullOrEmpty(error)) {
			Logger.Error(error);
		}
	}
}