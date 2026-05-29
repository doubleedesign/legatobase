using System.Text;

namespace setup;

public class ContextUtils {
	protected string? EnvFilePath { get; set; }
	private string? _dbPath;
	protected string? DbPath {
		get => this.GetDbPath();
		set => this.SetDbPath(value ?? "");
	}
	
	public ContextUtils() {
		Console.OutputEncoding = Encoding.UTF8;
		this.EnsureEnvFileCreated();
	}
	
	public string GetDbPath() {
		var path = !string.IsNullOrEmpty(this._dbPath) ? this._dbPath : this.GetEnvVarFromFile("DB_PATH");

		if (string.IsNullOrEmpty(path) || string.IsNullOrWhiteSpace(path)) {
			throw new Exception("Database path is not set");
		}

		return path;
	}
	
	private void SetDbPath(string value) {
		this._dbPath = value;
		this.WriteToEnvFile("DB_PATH", value);
	}
	
	private void EnsureEnvFileCreated() {
		string projectRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../.."));
		string envPath = Path.Combine(projectRoot, ".env");

		if (File.Exists(envPath)) {
			Logger.Success($"Local environment file exists at {envPath}");
			this.EnvFilePath = envPath;
			return;
		}
		
		File.WriteAllText(envPath, string.Empty);
		if (File.Exists(envPath)) {
			Logger.Success($"Created local environment file at {envPath}");
			this.EnvFilePath = envPath;
		}
		
		throw new Exception("Failed to find or create local environment file at " + envPath);
	}
	
	private string GetEnvVarFromFile(string key) {
		if (!File.Exists(this.EnvFilePath)) {
			throw new FileNotFoundException();
		}
		
		var lines = File.ReadAllLines(this.EnvFilePath).ToList();
		var index = lines.FindIndex(l => l.StartsWith($"{key}="));

		if (index < 0) {
			throw new Exception($"Key {key} not found in environment file");
		}

		return lines[index].Split('=', 2)[1];
	}
	
	private void WriteToEnvFile(string key, string? value) {
		if (!File.Exists(this.EnvFilePath)) {
			throw new FileNotFoundException();
		}

		var lines = File.ReadAllLines(this.EnvFilePath).ToList();
		var index = lines.FindIndex(l => l.StartsWith($"{key}="));
		var entry = $"{key}={value}";

		if (index >= 0) {
			lines[index] = entry;
		}
		else {
			lines.Add(entry);
		}

		File.WriteAllLines(this.EnvFilePath, lines);
	}
}