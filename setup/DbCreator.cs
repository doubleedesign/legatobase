using Legatobase.Common;
using Microsoft.Data.Sqlite;

namespace Legatobase.Setup;

public class DbCreator {
	private readonly string _dbPath;

	public DbCreator() {
		this._dbPath = Config.GetDbPath();
		this.EnsureDbFileCreated();
	}

	public void Create() {
		using var connection = new SqliteConnection("Data Source=" + this._dbPath);
		connection.Open();
		this.CreateTables(connection);
		connection.Close();
	}
	
	private void CreateTables(SqliteConnection connection) {
		var tables = new Dictionary<string, string> {
			["Genres"] = @"CREATE TABLE IF NOT EXISTS Genres (
				Id		INTEGER PRIMARY KEY AUTOINCREMENT UNIQUE NOT NULL,
				Label	VARCHAR	NOT NULL
			)",

			["Tracks"] = @"CREATE TABLE IF NOT EXISTS Tracks (
				Id				INTEGER PRIMARY KEY AUTOINCREMENT UNIQUE NOT NULL,
				Title			VARCHAR	NOT NULL,
				Year			INTEGER,
				GenreId 		INTEGER,
				ISRC 			VARCHAR,
				ISWC 			VARCHAR,
				MBID 			VARCHAR,
				SHS_ID 			INTEGER,
				PlayCount		INTEGER,
				FileLocation 	VARCHAR,
				FileSize 		INTEGER,
				FileTypeId 		INTEGER,
				Length			INTEGER,
				BitRate 		INTEGER,
				SampleRate		INTEGER,
				FOREIGN KEY (GenreId) REFERENCES Genres(Id)
			)",
			
			["FileTypes"] = @"CREATE TABLE IF NOT EXISTS FileTypes (
    			Id INTEGER PRIMARY KEY AUTOINCREMENT UNIQUE NOT NULL,
    			Label VARCHAR NOT NULL
    		)",
			
			["Artists"] = @"CREATE TABLE IF NOT EXISTS Artists(
    			Id				INTEGER PRIMARY KEY AUTOINCREMENT UNIQUE NOT NULL,
				Name 			VARCHAR	NOT NULL,
				MBID 			VARCHAR,
				DID 			VARCHAR,
				Profile 		TEXT,
				Hometown 		VARCHAR,
				Country 		VARCHAR,
				BirthDate 		DATE,
				DeathDate 		DATE
			)",	

			["Groups"] = @"CREATE TABLE IF NOT EXISTS Groups(
    			Id				INTEGER PRIMARY KEY UNIQUE NOT NULL,
    			Origin			VARCHAR,
    			FoundedDate 	DATE,
    			EndedDate		DATE,
    			FOREIGN KEY (Id) REFERENCES Artists(Id)
    		)",
			
			["People"] = @"CREATE TABLE IF NOT EXISTS People(
				Id				INTEGER PRIMARY KEY UNIQUE NOT NULL,
				Hometown 		VARCHAR,
				BirthDate 		VARCHAR,
				DeathDate 		VARCHAR,
				FOREIGN KEY (Id) REFERENCES Artists(Id)
			)",
			
			["Artists_Groups"] = @"CREATE TABLE IF NOT EXISTS ArtistsGroups(
			    ArtistId 		INTEGER NOT NULL,
			    GroupId 		INTEGER NOT NULL,
			    MembershipStart	VARCHAR,
			    MembershipEnd	VARCHAR,
			    FOREIGN KEY (ArtistId) REFERENCES Artists(Id),
			    FOREIGN KEY (GroupId) REFERENCES Artists(Id)
			)",
			
			["Albums"] = @"CREATE TABLE IF NOT EXISTS Albums(
    			Id 					INTEGER PRIMARY KEY AUTOINCREMENT UNIQUE NOT NULL,
    			Title  				VARCHAR NOT NULL,
    			ReleaseArtistId  	INTEGER NOT NULL,
    			Year 				INTEGER,
    			Barcode 			VARCHAR,
    			ReleaseGroupId		VARCHAR, /* Musicbrainz */
    			MasterId 			VARCHAR, /* Discogs */
    			MBID 				VARCHAR,
    			external_playcount 	INTEGER,
    			FOREIGN KEY (ReleaseArtistId) REFERENCES Artists(Id)
			)",
			
			["Roles"] = @"CREATE TABLE IF NOT EXISTS Roles(
    			Id		INTEGER PRIMARY KEY AUTOINCREMENT UNIQUE NOT NULL,
    			Label 	VARCHAR	NOT NULL
    		)",
			
			["Artists_Tracks"] = @"CREATE TABLE IF NOT EXISTS Artists_Tracks(
    			TrackId 		INTEGER NOT NULL,
    			ArtistId		INTEGER NOT NULL,
    			RoleId			INTEGER NOT NULL,
    			FOREIGN KEY (TrackId) REFERENCES Tracks(Id),
    			FOREIGN KEY (ArtistId) REFERENCES Artists(Id),
				FOREIGN KEY (RoleId) REFERENCES Roles(Id)
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
					Logger.Success("Table exists", tableName);
					continue;
				}
				
				Logger.Info("Creating table", tableName);
				using var cmd = connection.CreateCommand();
				cmd.CommandText = sql;
				cmd.ExecuteNonQuery();
				
				if(this.TableExists(tableName, connection)) {
					Logger.Success("Created table", tableName);
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
	
	private void EnsureDbFileCreated() {
		if (File.Exists(this._dbPath)) {
			Logger.Success("Database file exists", this._dbPath);
			return;
		} 
		
		this.MaybeCreateDirectory(Config.GetWindowsAppDataDirectory());
		
		try {
			Logger.Info("Creating database file", this._dbPath);
			File.WriteAllBytes(this._dbPath, Array.Empty<byte>());
			if (File.Exists(this._dbPath)) {
				Logger.Success("Created database file", this._dbPath);
				return;
			}
			
			throw new Exception("Failed to create database file at " + this._dbPath);
		}
		catch (Exception e) {
			Logger.Error(e.Message);
			Environment.Exit(1);
		}
	}
	
	private void MaybeCreateDirectory(string dirPath) {
		if (Directory.Exists(dirPath)) {
			Logger.Success("Directory exists", dirPath);
			return;
		}
		
		Logger.Info("Creating directory", dirPath);
		try {
			Directory.CreateDirectory(dirPath);
			if(Directory.Exists(dirPath)) {
				Logger.Success("Created directory", dirPath);
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