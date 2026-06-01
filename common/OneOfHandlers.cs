using OneOf;
namespace common;

public static class OneOfHandlers {
	
	/**
	 * Utility function to assert that a value is a string when using a OneOf union type that includes string
	 */
	public static string? AsString(this OneOf<string, int> value) {
		return value.IsT0 ? value.AsT0 : null;
	}

	/**
	 * Utility function to assert that a value is an int when using a OneOf union type that includes int
	 */
	public static int? AsInt(this OneOf<string, int> value) {
		return value.IsT1 ? value.AsT1 : null;
	}
}