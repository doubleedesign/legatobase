namespace Legatobase.Common;

public class Logger {
	private static int LABEL_WIDTH = 30;
	private static string _logFile = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"..\..\..\..\log.txt"));
	
	private static void LogToFile(string message) {
		if (!File.Exists(_logFile)) {
			File.Create(_logFile).Dispose();
		}
		
		using (var stream = new FileStream(_logFile, FileMode.Append, FileAccess.Write, FileShare.ReadWrite)) {
			using (var writer = new StreamWriter(stream)) {
				writer.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} \t {message}");
			}
		}
	}
	
	public static void Success(string message, string extra = "", bool logToFile = false) {
		Console.ForegroundColor = ConsoleColor.Green;
		Console.WriteLine("✅  " + message.PadRight(LABEL_WIDTH) + extra);
		Console.ResetColor();
		if (logToFile) {
			LogToFile("✅ " + message + " " + extra);
		}
	}
	
	public static void Error(string message, string extra = "", bool logToFile = false) {
		Console.ForegroundColor = ConsoleColor.Red;
		Console.WriteLine("❌  " + message.PadRight(LABEL_WIDTH) + extra);
		Console.ResetColor();
		if (logToFile) {
			LogToFile("❌ " + message + " " + extra);
		}
	}
	
	public static void Warning(string message, string extra = "", bool logToFile = false) {
		Console.ForegroundColor = ConsoleColor.Yellow;
		Console.WriteLine("⚠️  " + message.PadRight(LABEL_WIDTH) + extra);
		Console.ResetColor();
		if (logToFile) {
			LogToFile("⚠️ " + message + " " + extra);
		}
	}

	public static void Info(string message, string extra = "", bool logToFile = false) {
		Console.ForegroundColor = ConsoleColor.Cyan;
		Console.WriteLine("📄  " + message.PadRight(LABEL_WIDTH) + extra);
		Console.ResetColor();
		if (logToFile) {
			LogToFile("📄 " + message + " " + extra);
		}
	}

	public static string Input(string message) {
		Console.ForegroundColor = ConsoleColor.Magenta;
		Console.WriteLine("❔  " + message);
		Console.ResetColor();

		return Console.ReadLine() ?? "";
	}
}