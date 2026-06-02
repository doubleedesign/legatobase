using System.Reflection;

namespace Legatobase.API;

public class Config {

	public static string GetVersion() {
		return Assembly.GetEntryAssembly()?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? "0.0.0";
	}

	public static string GetUrl() {
		return "https://github.com/doubleedesign/legatobase";
	}
}