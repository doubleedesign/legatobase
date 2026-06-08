using System.Text.Json.Nodes;

namespace Legatobase.Common;

public static class JsonObjectHandlers {
	/// <summary>
	/// Get a subset of data from a JsonObject using keys.
	/// </summary>
	/// <param name="obj"></param>
	/// <param name="keys"></param>
	/// <returns></returns>
	public static JsonObject Pick(this JsonObject obj, string[] keys) {
		var result = new JsonObject();
		foreach (var key in keys) {
			if (obj.TryGetPropertyValue(key, out var value)) {
				result[key] = value?.DeepClone();
			}
		}

		return result;
	}
}