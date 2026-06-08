using System.Text.Json.Nodes;
using FluentHttpClient;
namespace Legatobase.Common;

public static class HttpResponseHandlers {
	
	public static async Task<JsonObject> GetResultObject(this HttpResponseMessage response) {
		response.EnsureSuccessStatusCode();
		
		// This is the bit that extracts the raw data from the response
		return await response.ReadJsonObjectAsync() ?? throw new InvalidDataException("Response was not a valid JSON object");
	}
}