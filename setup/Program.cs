using System.Diagnostics;
using System.Text;
using Legatobase.Common;
using Spectre.Console;

namespace Legatobase.Setup;

class Program {
	static void Main(string[] args) {
		Console.OutputEncoding = Encoding.UTF8;
		Logger.Info("Legatobase version", Config.GetVersion());
		EnsureDbExists();
		EnterCredentials();
		
		// Prompt for next action
		var action = AnsiConsole.Prompt(
			new SelectionPrompt<string>()
				.Title("What would you like to do?")
				.AddChoices("Import iTunes library", "Regenerate EF Core classes", "Exit"));

		switch (action) {
			case "Import iTunes library":
				Import();
				break;
			case "Regenerate EF Core classes":
				RegenerateClasses(Config.GetDbPath());
				break;
			default:
				Environment.Exit(0);
				break;
		}
	}

	private static void EnsureDbExists() {
		new DbCreator().Create();
	}

	static void Import() {
		var filePath = "C:/Users/leesa/Desktop/Library-2026-05-23.xml"; // FIXME: This is temporary, remove it
		
		while (string.IsNullOrEmpty(filePath)) {
			filePath = Logger.Input("Enter the path to your iTunes library XML file:");
		}
		
		var importer = new ITunesImporter(filePath);
		importer.Import();
	}

	static void RegenerateClasses(string dbFilePath) {
		Console.WriteLine("");
		Logger.Info("Regenerating Entity Framework classes");
		Logger.Warning("Existing classes will be replaced to match the current database schema");
		var solutionRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"..\..\..\..\"));
		
		var process = new Process {
			StartInfo = new ProcessStartInfo {
				FileName = "dotnet",
				Arguments = $"ef dbcontext scaffold " +
				            $"\"Data Source={dbFilePath}\" Microsoft.EntityFrameworkCore.Sqlite " +
				            $"--startup-project setup/setup.csproj " +
				            $"--output-dir {solutionRoot}/core " +
				            $"--context LbContextBase " +
				            "--no-onconfiguring " +
				            "--namespace Legatobase.Core " +
				            "--context-namespace Legatobase.Core " +
							$"--force ",
				WorkingDirectory = solutionRoot,
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

	static void EnterCredentials() {
		var discogs = Config.GetCredential("Discogs");
		if (discogs == null) {
			Logger.Info("Get your Discogs credentials from: https://www.discogs.com/settings/developers");
			string discogsConsumerKey = Logger.Input("Enter your Discogs  consumer key:");
			string discogsConsumerSecret = Logger.Input("Enter your Discogs consumer secret:");
			Config.SetCredential("Discogs", discogsConsumerKey, discogsConsumerSecret);
		}
	}
}