using System.Reflection;
namespace Legatobase.Common;

public class Config {

	public static string GetVersion() {
		return Assembly.GetExecutingAssembly().GetName().Version!.ToString();
	}

	public static string GetUrl() {
		return "https://legatobase.app";
	}

	public static string GetWindowsAppDataDirectory() {
		return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Legatobase");
	}
	
	public static string GetDbPath() {
		return Path.Join(GetWindowsAppDataDirectory(), "legatobase.db");
	}
}