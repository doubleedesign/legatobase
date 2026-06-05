namespace Legatobase.Common;

public static class DictionaryExtensions {

	public static Dictionary<string, TValue> MergeWith<TKey, TValue>(this Dictionary<string, TValue> dict1, Dictionary<string, TValue> dict2) {
		return dict1.Union(dict2).ToDictionary(k => k.Key, v => v.Value);
	}
}