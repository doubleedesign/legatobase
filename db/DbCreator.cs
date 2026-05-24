using System.Text;
using Microsoft.Data.Sqlite;

namespace db;

public class DbCreator {
	private string? DbPath { get; set; }
	
	public DbCreator() {
		Console.OutputEncoding = Encoding.UTF8;
		this.MaybeCreateDirectory();
	}

	public void Create() {
		this.MaybeCreateDb();
		this.CreateTables();
	}

	private void MaybeCreateDirectory() {
		var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
		var dirPath = Path.Join(appData, "Legatobase");
		
		if (!Directory.Exists(dirPath)) {
			Logger.Warning("Directory does not exist");
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
		else {
			Logger.Success("Directory exists at " + Path.Join(appData, "Legatobase"));
		}
	}

	private void MaybeCreateDb() {
		var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
		var dbPath = Path.Join(appData, "Legatobase", "legatobase.db");
		
		if (File.Exists(dbPath)) {
			Logger.Success("Database already exists at " + dbPath);
			this.DbPath = dbPath;
		} 
		else {
			Logger.Warning("Database does not exist");
			Logger.Info("Creating database file at " + dbPath);
			try {
				if (File.Exists(Path.Join(dbPath, "legatobase.db"))) {
					Logger.Success("Created empty database file at " + dbPath);
					this.DbPath = dbPath;
				}
				else {
					throw new FileNotFoundException();
				}
			}
			catch (Exception e) {
				Logger.Error(e.Message);
				Environment.Exit(1);
			}
		}
	}

	private void CreateTables() {
		if (String.IsNullOrEmpty(DbPath)) {
			throw new FileNotFoundException();
		}
		
		Logger.Info("Using database at: " + this.DbPath);
		using var connection = new SqliteConnection("Data Source=" + this.DbPath);
		connection.Open();

		var tables = new[] {
			@"CREATE TABLE IF NOT EXISTS Genres (
				Id		INTEGER PRIMARY KEY AUTOINCREMENT,
				Label	VARCHAR	NOT NULL
			)",

			@"CREATE TABLE IF NOT EXISTS Tracks (
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
			
			@"CREATE TABLE IF NOT EXISTS Artists(
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
			
			@"CREATE TABLE IF NOT EXISTS ArtistGroups(
			    ArtistId 	INTEGER NOT NULL,
			    GroupId 	INTEGER NOT NULL,
			    FOREIGN KEY (ArtistId) REFERENCES Artists(Id),
			    FOREIGN KEY (GroupId) REFERENCES Artists(Id)
			)",
			
			@"CREATE TABLE IF NOT EXISTS Albums(
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
			
			@"CREATE TABLE IF NOT EXISTS ArtistTypes(
    			Id		INTEGER PRIMARY KEY AUTOINCREMENT,
    			Label 	VARCHAR	NOT NULL
    		)",
			
			@"CREATE TABLE IF NOT EXISTS ArtistsTracks(
    			TrackId 		INTEGER NOT NULL,
    			AritstId 		INTEGER NOT NULL,
    			AristTypeId 	INTEGER NOT NULL,
    			FOREIGN KEY (TrackId) REFERENCES Tracks(Id),
    			FOREIGN KEY (AritstId) REFERENCES Artists(Id),
				FOREIGN KEY (AristTypeId) REFERENCES ArtistTypes(Id)
			)",
			
			@"CREATE TABLE IF NOT EXISTS AlbumTracks(
    			TrackId 		INTEGER NOT NULL,
    			AlbumId			INTEGER NOT NULL,
    			TrackNumber 	INTEGER,
    			DiscNumber 		INTEGER,
    			FOREIGN KEY (TrackId) REFERENCES Tracks(Id),
				FOREIGN KEY (AlbumId) REFERENCES Albums(Id)
			)"
		};

		Logger.Info("Creating database tables");
		foreach (var sql in tables) {
			try {
				using var cmd = connection.CreateCommand();
				cmd.CommandText = sql;
				cmd.ExecuteNonQuery();
			}
			catch (Exception e) {
				Logger.Error(e.Message);
				Environment.Exit(1);
			}
		}
		
		// Check if the tables have been created
		var tableNames = new[] { "Genres", "Tracks", "Artists", "ArtistGroups", "Albums", "ArtistTypes", "ArtistsTracks", "AlbumTracks" };
		foreach (var table in tableNames) {
			using var cmd = connection.CreateCommand();
			cmd.CommandText = $"SELECT name FROM sqlite_master WHERE type='table' AND name='{table}'";
			var result = cmd.ExecuteScalar();
			if (result is null) {
				Logger.Error($"Table {table} was not created successfully");
			}
			else if (result.ToString() == table) {
				Logger.Success($"Table {table} created or was already there");
			}
			else {
				Logger.Error($"Unexpected result when checking for table {table}: {result}");
			}
		}

		connection.Close();
	}
}