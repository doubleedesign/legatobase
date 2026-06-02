using OneOf;
namespace Legatobase.Common;

public static class OneOfHandlers {
	
	/**
	 * Utility function to assert that a value is a string when using a OneOf union type that includes string,
	 * or if it is an integer, convert it to a string
	 */
	public static string? AsString(this OneOf<string, int> value) {
		if (value.IsT1) {
			return value.ToString();
		}
		
		return value.IsT0 ? value.AsT0 : null;
	}

	/**
	 * Utility function to assert that a value is an int when using a OneOf union type that includes int
	 */
	public static int? AsInt(this OneOf<string, int> value) {
		return value.IsT1 ? value.AsT1 : null;
	}

	public static bool IsNull(this OneOf<string, int> value) {
		return !value.IsT0 && !value.IsT1;
	}
}