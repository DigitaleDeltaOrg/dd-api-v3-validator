using System.Globalization;
using CsdlPropertyInspector;
using CsvHelper;
using CsvHelper.Configuration;
using DigitaleDeltaMetaDataValidator.Models;

namespace DigitaleDeltaMetaDataValidator;

/// <summary>
/// Validates a CSDL according to the specified CSV data located at validationUrl
/// </summary>
public class Validator
{
  /// <summary>
  ///  Validate.
  /// </summary>
  /// <param name="validationData">URL or file name of the validation CSV.</param>
  /// <param name="dataToValidate">URL of the metadata that needs to be validated.</param>
  /// <returns>List of strings, containing error messages.</returns>
  /// <exception cref="Exception"></exception>
  public static async Task<List<string>?> ValidateAsync(string validationData, string dataToValidate)
  {
    var errors = new List<string>();
    var fields = await GetValidationStructureFromUrlAsync(validationData).ConfigureAwait(false);
    if (fields == null)
    {
      throw new Exception($"No data found at {validationData}.");
    }

    // Check only Observations. It will contain References as well.
    var csdlProperties = await GetCsdlPropertiesAsync(dataToValidate).ConfigureAwait(false);
    
    CheckForRequiredButMissingEntitiesAndProperties(csdlProperties, fields, errors);
    CheckForUnknownEntitiesAndProperties(csdlProperties, fields, errors);
    
    return errors.Count != 0 ? errors : null;
  }

  /// <summary>
  /// Check for properties, unknown to the specification.
  /// </summary>
  /// <param name="csdlProperties">Properties read from the $metadata</param>
  /// <param name="fields">Validation specification from the validation file</param>
  /// <param name="errors">Errors, to be amended.</param>
  private static void CheckForUnknownEntitiesAndProperties(Dictionary<string, CsdlType> csdlProperties, List<ValidationCsv> fields, List<string> errors)
  {
    var fieldsDictionary = fields.GroupBy(f => f.Entity).ToDictionary(g => g.Key, g => g.ToList());

    foreach (var (entity, properties) in fieldsDictionary)
    {
      if (!csdlProperties.ContainsKey(entity))
      {
        errors.Add($"Entity {entity} is not found in the CSDL properties.");
        continue;
      }

      var csdlType = csdlProperties[entity];
      foreach (var property in properties.Where(a => a.Type == csdlType.Name))
      {
        if (csdlType.Properties == null || csdlType.Properties.All(p => p.Name != property.Property))
        {
          errors.Add($"Property {property.Property} of entity {entity} is not found in the CSDL properties and therefore unsupported.");
        }
      }
    }
  }

  /// <summary>
  /// Check for required, but missing properties.
  /// </summary>
  /// <param name="csdlEntities">Properties read from the $metadata</param>
  /// <param name="fields">Validation specification from the validation file</param>
  /// <param name="errors">Errors, to be amended.</param>
  private static void CheckForRequiredButMissingEntitiesAndProperties(Dictionary<string, CsdlType> csdlEntities, List<ValidationCsv> fields, List<string> errors)
  {
    var fieldsDictionary = fields.Where(a => a is { PropertyRequired: true, EntityRequired: true }).GroupBy(f => f.Entity).ToDictionary(g => g.Key, g => g.ToList());
   
    foreach (var csdlEntity in csdlEntities.Values.Where(a => a is { Properties.Count: > 0, IsRequired: true }))
    {
      if (!fieldsDictionary.ContainsKey(csdlEntity.Name))
      {
        errors.Add($"Entity {csdlEntity.Name} not found in validation fields.");
        continue;
      }
      
      var entityFields = fieldsDictionary[csdlEntity.Name];
      foreach (var entityField in entityFields)
      {
        if (csdlEntity.Properties == null || csdlEntity.Properties.Count == 0)
        {
          continue;
        }

        if (csdlEntity.Properties.All(p => p.Name != entityField.Property))
        {
          errors.Add($"Required property {entityField.Property} not found in entity {csdlEntity.Name}.");
        }

        var property = csdlEntity.Properties.FirstOrDefault(p => p.Name == entityField.Property);
        if (property != null && property.Type != entityField.Type)
        {
          errors.Add($"Property {entityField.Property} in entity {csdlEntity.Name} has incorrect type.");
        }
      }
    }
  }
  
  /// <summary>
  /// Retrieve CSDL properties from the $metadata URL.
  /// </summary>
  /// <param name="url">URL with the OData $metadata</param>
  /// <param name="type">Name of the type to examine.</param>
  /// <returns>Dictionary of types by name, with their type definition and their properties.</returns>
  /// <exception cref="Exception"></exception>
  public static async Task<Dictionary<string, CsdlType>> GetCsdlPropertiesAsync(string url)
  {
    var payload = await GetContentAsync(url + "/$metadata").ConfigureAwait(false);
    var data    = CsdlInspector.Inspect(payload, "Observation");
    if (data == null)
    {
      throw new Exception("No CSDL properties found.");
    }
    
    return data;
  }
  
  /// <summary>
  /// Read the body of the specified URL or file as a string.
  /// </summary>
  /// <param name="url">URL</param>
  /// <returns>Body as a string.</returns>
  /// <exception cref="Exception"></exception>
  private static async Task<string> GetContentAsync(string url)
  {
    if (!IsUri(url))
    {
      return await File.ReadAllTextAsync(url).ConfigureAwait(false);
    }

    using var httpClient = new HttpClient();
    var       payload    = await httpClient.GetStringAsync(url).ConfigureAwait(false);
    if (payload == null)
    {
      throw new Exception($"No data found at {url}.");
    }

    return payload;
  }

  /// <summary>
  /// Retrieve validation data from the specified URL.
  /// </summary>
  /// <param name="url">URL</param>
  /// <returns>List of Validation data.</returns>
  /// <exception cref="Exception"></exception>
  private static async Task<List<ValidationCsv>?> GetValidationStructureFromUrlAsync(string url)
  {
    var payload          = await GetContentAsync(url).ConfigureAwait(false);
    if (payload == null)
    {
      throw new Exception("");
    }
    
    var csvConfiguration = new CsvConfiguration(CultureInfo.InvariantCulture);
    var csvParser        = new CsvParser(new StringReader(payload), csvConfiguration);
    var csvReader           = new CsvReader(csvParser);
    
    return csvReader.GetRecords<ValidationCsv>().ToList();
  }
  
  /// <summary>
  /// Checks if the specified string is a URI.
  /// </summary>
  /// <param name="input">URI of File name</param>
  /// <returns>True if Uri</returns>
  private static bool IsUri(string input)
  {
    return Uri.TryCreate(input, UriKind.Absolute, out var uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
  }
}