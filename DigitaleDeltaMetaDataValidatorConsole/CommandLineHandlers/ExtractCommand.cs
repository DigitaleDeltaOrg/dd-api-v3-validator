using DigitaleDeltaMetaDataValidatorConsole.Shared;
using Spectre.Console.Cli;

namespace DigitaleDeltaMetaDataValidatorConsole.CommandLineHandlers;
using System.ComponentModel;
internal sealed class ExtractCommand : AsyncCommand<ExtractCommand.ExtractCommandSettings>
{
  
  public sealed class ExtractCommandSettings : CommandSettings
  {
    [CommandArgument(0, "<examine>")] 
    [Description("URL or file to examine: the definition from which the definition needs to be extracted. This can either be url or a file. If a URL, the $metadata endpoint should not be added.")]
    public string? Examine { get; set; }
    [CommandArgument(1, "<result>")] 
    [Description("Name of the file that needs to hold the result of the validation.")]
    public string? Result { get; set; }
  }
  
  public override async Task<int> ExecuteAsync(CommandContext context, ExtractCommandSettings settings)
  {
    if (string.IsNullOrWhiteSpace(settings.Examine))
    {
      Helper.ReportIsRequired(nameof(settings.Examine));
      
      return 1;
    }

    if (string.IsNullOrWhiteSpace(settings.Result))
    {
      Helper.ReportIsRequired(nameof(settings.Result));
      
      return 1;
    }
    
    if (Helper.IsUrl(settings.Examine) && !Helper.IsUrlValid(settings.Examine))
    {
      Helper.ReportInvalidUrl(settings.Examine);
      
      return 1;
    }
    
    if (Helper.IsUrl(settings.Examine))
    {
      using var client = new HttpClient();
      if (!await Helper.CheckUrlAccessibility(client, settings.Examine + "/$metadata"))
      {
        Helper.ReportNotAccessible(settings.Examine);
        
        return 1;
      }
    }

    try
    {
      var result     = await DigitaleDeltaMetaDataValidator.DigitaleDeltaMetaDataValidator.GetCsdlPropertiesAsync(settings.Examine).ConfigureAwait(true);
      var properties = Helper.CompileValidationRules(result);
      
      Helper.WriteValidationRulesToCsv(properties, settings.Result);
      Helper.ReportFinished($"Properties written to file '{settings.Result}'");

      return 0;
    }
    catch (Exception e)
    {
      Helper.ReportError(e.Message);
      
      return 1;
    }
  }
}