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
				.AddChoices(
					"Import release by barcode", 
					"Import release from folder", 
					"Import single track from file",
					"Import discography of artist",
					"Regenerate EF Core classes", 
					"Exit"
				)
			);

		switch (action) {
			case "Import release by barcode":
				ImportByBarcode();
				break;
			case "Import release from folder":
				ImportFromFolder();
				break;
			case "Import single track from file":
				ImportSingleTrack();
				break;
			case "Import discography of artist":
				ImportByArtist();
				break;
			case "Regenerate EF Core classes":
				RegenerateClasses(Config.GetDbPath());
				break;
			default:
				Environment.Exit(0);
				break;
		}
	}

	private static void ImportByBarcode() {
		throw new NotImplementedException();
	}

	private static void ImportFromFolder() {
		throw new NotImplementedException();
	}

	private static void ImportSingleTrack() {
		throw new NotImplementedException();
	}

	private static void ImportByArtist() {
		throw new NotImplementedException();
	}

	private static void EnsureDbExists() {
		new DbCreator().Create();
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
							"--force ",
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
			LogOutput(output);
		}
		if (!string.IsNullOrEmpty(error)) {
			// Use the same handler for "errors" as for standard output
			// because for some reason messages like "build started" are sent to stderr instead of stdout
			LogOutput(error);
		}
		
		CleanUpStaleClasses();
	}
	
	static void CleanUpStaleClasses() {
		string coreDir = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"..\..\..\..\core"));
		var files = Directory.GetFiles(coreDir, "*.cs", SearchOption.AllDirectories);
		foreach (var file in files) {
			// If the file was not generated in the last 5 minutes, delete it
			var lastWriteTime = File.GetLastWriteTime(file);
			if (DateTime.Now - lastWriteTime > TimeSpan.FromMinutes(5)) {
				File.Delete(file);
				Logger.Warning("Deleted stale class file: ", file);
			}
		}
	}

	static void LogOutput(string output) {
		if (output.Contains("Build succeeded")) {
			Logger.Success(output);
		}
		else if (output.Contains("Build started")) {
			Logger.Info(output);
		}
		else if (output.Contains("Build failed")) {
			Logger.Error(output);
		}
		else if (output.Contains("failed") || output.Contains("error")) {
			Logger.Error(output);
		}
		else {
			Logger.Success(output);
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