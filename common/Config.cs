using System.Net;
using System.Reflection;
using AdysTech.CredentialManager;

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

	public static void SetCredential(string label, string username = "", string password = "") {
		var cred = new NetworkCredential(username, password);
		CredentialManager.SaveCredentials($"Legatobase:{label}", cred, CredentialType.Generic, persistence: Persistence.LocalMachine);
	}

	public static NetworkCredential? GetCredential(string label) {
		return CredentialManager.GetCredentials($"Legatobase:{label}");
	}
}