using System.Globalization;
using CsdlPropertyInspector;
using CsvHelper;
using DigitaleDeltaMetaDataValidator.Models;
using Spectre.Console;

namespace DigitaleDeltaMetaDataValidatorConsole.Shared;

public static class Helper
{
  public static bool IsUrlValid(string url)
  {
    return Uri.IsWellFormedUriString(url, UriKind.Absolute);
  }

  public static async Task<bool> CheckUrlAccessibility(HttpClient client, string url)
  {
    try
    {
      var response = await client.GetAsync(url).ConfigureAwait(true);
      return response.IsSuccessStatusCode;
    }
    catch
    {
      return false;
    }
  }

  public static bool IsUrl(string url)
  {
    try
    {
      var uri = new Uri(url);
      return uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps;
    }
    catch
    {
      return false;
    }
  }

  public static bool IsValidFilePath(string filePath)
  {
    FileInfo? fileInfo = null;
    try
    {
      fileInfo = new FileInfo(filePath);
    }
    catch (ArgumentException) { }
    catch (PathTooLongException) { }
    catch (NotSupportedException) { }

    return fileInfo != null;
  }

  internal static void ReportIsRequired(string parameter)
  {
    AnsiConsole.MarkupLine($"[red]{parameter} is required[/]");
  }

  internal static void ReportInvalidUrl(string parameter)
  {
    AnsiConsole.MarkupLine($"[red]{parameter} is an invalid URL[/]");
  }
  
  internal static void ReportNotAccessible(string parameter)
  {
    AnsiConsole.MarkupLine($"[red]URL {parameter} is not accessible[/]");
  }

  internal static void ReportError(string error)
  {
    AnsiConsole.MarkupLine($"[red]{error}[/]");
  }
  
  internal static void ReportFinished(string message)
  {
    AnsiConsole.MarkupLine($"[green]{message}[/]");
  }

  internal static void WriteValidationRulesToCsv(IEnumerable<ValidationCsv> validations, string fileName)
  {
    using var writer = new StreamWriter(fileName);
    using var csv    = new CsvWriter(writer, CultureInfo.InvariantCulture);
  
    csv.WriteRecords(validations);
  }

  internal static void WriteStringArrayToFile(List<string>? array, string fileName)
  {
    if (array == null)
    {
      File.WriteAllText(fileName, "");
      
      return;
    }
    File.WriteAllLines(fileName, array);
  }
  
  internal static IEnumerable<ValidationCsv> CompileValidationRules(Dictionary<string, CsdlType> properties)
  {
    var validations = new List<ValidationCsv>();

    foreach (var value in properties.Values.Where(value => value.Properties != null))
    {

      validations.AddRange(value.Properties!.Select(property => new ValidationCsv
      {
        Entity           = value.Name,
        EntityRequired   = value.IsRequired,
        Property         = property.Name,
        Type             = property.Type,
        PropertyRequired = !property.IsNullable
      }));
    }

    return validations;
  }
}