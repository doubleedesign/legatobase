namespace Legatobase.Common;

public static class SimpleDataValueHandlers {
	
	public static string? AsString(this SimpleDataValue value) {
		return value.ToString();
	}
}