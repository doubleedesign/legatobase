using System.Text;
using Microsoft.EntityFrameworkCore;

namespace db;

public class Context : DbContext {
	public string DbPath { get; }
	
	public Context() {
		Console.OutputEncoding = Encoding.UTF8;
		var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
		this.MaybeCreateDirectory();
		
		DbPath = Path.Join(appData, "Legatobase", "legatobase.db");
		this.MaybeCreateDb();
	}

	protected override void OnConfiguring(DbContextOptionsBuilder options) {
		options.UseSqlite($"Data Source={DbPath}");
	}

	protected void MaybeCreateDirectory() {
		var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
		if (!Directory.Exists(Path.Join(appData, "Legatobase"))) {
			Logger.Warning("Directory does not exist");
			Logger.Info("Creating directory at " + Path.Join(appData, "Legatobase"));
			try {
				Directory.CreateDirectory(Path.Join(appData, "Legatobase"));
				Logger.Success("Created directory at " + Path.Join(appData, "Legatobase"));
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

	protected void MaybeCreateDb() {
		if (File.Exists(this.DbPath)) {
			Logger.Success("Database already exists at " + this.DbPath);
		} 
		else {
			Logger.Warning("Database does not exist");
			Logger.Info("Creating database at " + this.DbPath);
			try {
				this.Database.EnsureCreated();
				Logger.Success("Created empty database at " + this.DbPath);
			}
			catch (Exception e) {
				Logger.Error(e.Message);
				Environment.Exit(1);
			}
		}
	}
}