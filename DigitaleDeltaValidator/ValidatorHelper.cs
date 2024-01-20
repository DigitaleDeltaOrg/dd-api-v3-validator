using System.Text.Json;

namespace DigitaleDeltaValidator;

public class ValidatorHelper
{
  internal static bool IsUrlSafe(string url)
  {
    if (!Uri.TryCreate(url, UriKind.Absolute, out var uriResult))
    {
      return false;
    }

    var scheme = uriResult.Scheme.ToLowerInvariant();

    return scheme is "http" or "https";
  }

  internal static async Task<bool> IsContentSafeAsync(string url)
  {
    var httpClient = new HttpClient();

    var httpResponse = await httpClient.GetAsync(url);
    if (httpResponse.Content.Headers.ContentType?.MediaType != "application/json")
    {
      return false; // Content-Type was not application/json
    }

    var jsonContent = await httpResponse.Content.ReadAsStringAsync();
    try
    {
      JsonDocument.Parse(jsonContent);
    
      return true;
    }
    catch (JsonException)
    {
      return false;
    }
  }
}