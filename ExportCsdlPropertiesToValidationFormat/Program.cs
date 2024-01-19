using System.Diagnostics;
using System.Globalization;
using CsvHelper;
using ExportCsdlPropertiesToValidationFormat.Models;

args = ["https://localhost:7071/v3/odata/$metadata", "d:\\PropertyCheck2024.01.csv"];
if (args.Length != 2)
{
  Console.WriteLine($"Usage: {Process.GetCurrentProcess().MainModule?.FileName} <metadatadefinition> <filename>");
  return;
}

try
{
  var payload     = await GetContentAsync(args[0]).ConfigureAwait(false);
  var validations = CompileValidationRules(payload);
  
  WriteValidationRulesToCsv(validations, args[1]);
  Console.WriteLine("Done.");
}
catch (HttpRequestException e)
{
  Console.WriteLine($"Network error: {e.Message}");
}
catch (IOException e)
{
  Console.WriteLine($"File error: {e.Message}");
}
catch (Exception e)
{
  Console.WriteLine(e);
}

return;

static async Task<string> GetContentAsync(string url)
{
  if (!IsUri(url))
  {
    return await File.ReadAllTextAsync(url).ConfigureAwait(false);
  }

  using var httpClient = new HttpClient();
  var       payload    = await httpClient.GetStringAsync(url).ConfigureAwait(false);

  if (payload == null)
  {
    throw new Exception($"No data found at {url}");
  }

  return payload;
}

static IEnumerable<ValidationCsv> CompileValidationRules(string payload)
{
  var properties  = CsdlPropertyInspector.CsdlInspector.Inspect(payload, "Observation");
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

static void WriteValidationRulesToCsv(IEnumerable<ValidationCsv> validations, string fileName)
{
  using var writer = new StreamWriter(fileName);
  using var csv    = new CsvWriter(writer, CultureInfo.InvariantCulture);
  
  csv.WriteRecords(validations);
}

static bool IsUri(string input)
{
  return Uri.TryCreate(input, UriKind.Absolute, out var uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
}