using System.Text;
using Microsoft.Data.Sqlite;

namespace setup;

public class DbCreator {
	private string? _dbPath;
	private string? DbPath {
		get => this.GetDbPath();
		set => this.SetDbPath(value ?? "");
	}
	
	private string? EnvFilePath { get; set; }

	public DbCreator() {
		Console.OutputEncoding = Encoding.UTF8;
		this.EnsureEnvFileCreated();
		this.EnsureDbFileCreated();
	}

	private void SetDbPath(string value) {
		this._dbPath = value;
		this.WriteToEnvFile("DB_PATH", value);
	}

	public string GetDbPath() {
		var path = !string.IsNullOrEmpty(this._dbPath) ? this._dbPath : this.GetEnvVarFromFile("DB_PATH");

		if (string.IsNullOrEmpty(path) || string.IsNullOrWhiteSpace(path)) {
			throw new Exception("Database path is not set");
		}

		return path;
	}

	public void Create() {
		using var connection = new SqliteConnection("Data Source=" + this.DbPath);
		connection.Open();
		this.CreateTables(connection);
		connection.Close();
	}
	
	private void CreateTables(SqliteConnection connection) {
		var tables = new Dictionary<string, string> {
			["Genres"] = @"CREATE TABLE IF NOT EXISTS Genres (
				Id		INTEGER PRIMARY KEY AUTOINCREMENT,
				Label	VARCHAR	NOT NULL
			)",

			["Tracks"] = @"CREATE TABLE IF NOT EXISTS Tracks (
				Id			INTEGER PRIMARY KEY AUTOINCREMENT,
				Title		VARCHAR	NOT NULL,
				Year		INTEGER,
				GenreId 	INTEGER,
				ISRC 		VARCHAR,
				ISWC 		VARCHAR,
				SHS_ID 		INTEGER,
				MBID 		VARCHAR,
				playcount	INTEGER,
				FOREIGN KEY (GenreId) REFERENCES Genres(Id)
			)",
			
			["Artists"] = @"CREATE TABLE IF NOT EXISTS Artists(
    			Id			INTEGER PRIMARY KEY AUTOINCREMENT,
				Name 		VARCHAR	NOT NULL,
				MBID 		VARCHAR,
				DID 		VARCHAR,
				Profile 	TEXT,
				Home 		VARCHAR,
				Country 	VARCHAR,
				Birthdate 	DATETIME,
				Deathdate 	DATETIME
			)",	
			
			["ArtistGroups"] = @"CREATE TABLE IF NOT EXISTS ArtistGroups(
			    ArtistId 	INTEGER NOT NULL,
			    GroupId 	INTEGER NOT NULL,
			    FOREIGN KEY (ArtistId) REFERENCES Artists(Id),
			    FOREIGN KEY (GroupId) REFERENCES Artists(Id)
			)",
			
			["Albums"] = @"CREATE TABLE IF NOT EXISTS Albums(
    			Id 			INTEGER PRIMARY KEY AUTOINCREMENT,
    			Title  		VARCHAR NOT NULL,
    			ArtistId  	INTEGER NOT NULL,
    			Year 		INTEGER,
    			Barcode 	VARCHAR,
    			MasterId 	VARCHAR,
    			MBID 		VARCHAR,
    			external_playcount INTEGER,
    			FOREIGN KEY (ArtistId) REFERENCES Artists(Id)
			)",
			
			["ArtistTypes"] = @"CREATE TABLE IF NOT EXISTS ArtistTypes(
    			Id		INTEGER PRIMARY KEY AUTOINCREMENT,
    			Label 	VARCHAR	NOT NULL
    		)",
			
			["Artists_Tracks"] = @"CREATE TABLE IF NOT EXISTS Artists_Tracks(
    			TrackId 		INTEGER NOT NULL,
    			ArtistId		INTEGER NOT NULL,
    			AristTypeId 	INTEGER NOT NULL,
    			FOREIGN KEY (TrackId) REFERENCES Tracks(Id),
    			FOREIGN KEY (ArtistId) REFERENCES Artists(Id),
				FOREIGN KEY (AristTypeId) REFERENCES ArtistTypes(Id)
			)",
			
			["Albums_Tracks"] = @"CREATE TABLE IF NOT EXISTS Albums_Tracks(
    			TrackId 		INTEGER NOT NULL,
    			AlbumId			INTEGER NOT NULL,
    			TrackNumber 	INTEGER,
    			DiscNumber 		INTEGER,
    			FOREIGN KEY (TrackId) REFERENCES Tracks(Id),
				FOREIGN KEY (AlbumId) REFERENCES Albums(Id)
			)"
		};

		Logger.Info("Creating database tables");
		foreach (var (tableName, sql) in tables) {
			try {
				if(this.TableExists(tableName, connection)) {
					Logger.Success($"Table {tableName} exists");
					continue;
				}
				
				Logger.Info($"Creating table {tableName}");
				using var cmd = connection.CreateCommand();
				cmd.CommandText = sql;
				cmd.ExecuteNonQuery();
				
				if(this.TableExists(tableName, connection)) {
					Logger.Success($"Created table {tableName}");
				}
				else {
					throw new Exception($"Failed to create table {tableName}");
				}
			}
			catch (Exception e) {
				Logger.Error(e.Message);
				Environment.Exit(1);
			}
		}
		
		connection.Close();
	}

	private bool TableExists(string tableName, SqliteConnection connection) {
		using var cmd = connection.CreateCommand();
		cmd.CommandText = $"SELECT name FROM sqlite_master WHERE type='table' AND name='{tableName}'";
		var result = cmd.ExecuteScalar();
		
		if (result is null) {
			return false;
		}
		
		if (result.ToString() == tableName) {
			return true;
		}
	
		Logger.Error($"Unexpected result when checking for table {tableName}: {result}");
		return false;
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
	
	private void EnsureDbFileCreated() {
		string appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
		string directory = Path.Join(appData, "Legatobase");
		string dbFilePath = Path.Join(directory, "legatobase.db");
		
		if (File.Exists(dbFilePath)) {
			Logger.Success("Database file exists at " + dbFilePath);
			this.DbPath = dbFilePath;
			return;
		} 
		
		this.MaybeCreateDirectory(directory);
		try {
			Logger.Info("Creating database file at " + dbFilePath);
			File.WriteAllBytes(dbFilePath, Array.Empty<byte>());
			if (File.Exists(dbFilePath)) {
				Logger.Success($"Created  database file at {dbFilePath}");
				return;
			}
			
			throw new Exception("Failed to create database file at " + dbFilePath);
		}
		catch (Exception e) {
			Logger.Error(e.Message);
			Environment.Exit(1);
		}
	}
	
	private void MaybeCreateDirectory(string dirPath) {
		if (Directory.Exists(dirPath)) {
			Logger.Success($"Directory exists at {dirPath}");
			return;
		}
		
		Logger.Info("Creating directory at " + dirPath);
		try {
			Directory.CreateDirectory(dirPath);
			if(Directory.Exists(dirPath)) {
				Logger.Success("Created directory at " + dirPath);
			}
			else {
				throw new DirectoryNotFoundException();
			}
		}
		catch (Exception e) {
			Logger.Error(e.Message);
			Environment.Exit(1);
		}
	}
}