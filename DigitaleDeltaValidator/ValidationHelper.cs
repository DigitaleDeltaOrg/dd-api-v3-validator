using System.Xml;

namespace DigitaleDeltaValidator;

internal static class ValidationHelper
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
    using var client  = new HttpClient();
    var       content = await client.GetStringAsync(url);
    try
    {
      var xmlDoc = new XmlDocument
      {
        XmlResolver = null
      }; 

      xmlDoc.LoadXml(content);

      return true;
    }
    catch (XmlException)
    {
      return false;
    }
  }
}