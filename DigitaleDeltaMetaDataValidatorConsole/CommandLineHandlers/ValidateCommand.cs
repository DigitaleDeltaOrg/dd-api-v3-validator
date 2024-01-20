using DigitaleDeltaMetaDataValidatorConsole.Shared;
using Spectre.Console.Cli;
using System.ComponentModel;
using Spectre.Console;

namespace DigitaleDeltaMetaDataValidatorConsole.CommandLineHandlers;

internal sealed class ValidateCommand : AsyncCommand<ValidateCommand.ValidateCommandSettings>
{
  
  public sealed class ValidateCommandSettings : CommandSettings
  {
    [CommandArgument(0, "<source>")] 
    [Description("Validation source: the definition that holds the truth. This can either be url or a file.")]
    public string? Source { get; set; }
    [CommandArgument(0, "<examine>")] 
    [Description("URL or file to examine: the definition from which the definition needs to be extracted. This can either be url or a file. If a URL, the $metadata endpoint should not be added.")]
    public string? Examine { get; set; }
    [CommandArgument(2, "<result>")] 
    [Description("Name of the file that needs to hold the result of the validation.")]
    public string? Result { get; set; }
  }
  
  public override async Task<int> ExecuteAsync(CommandContext context, ValidateCommandSettings settings)
  {
    if (string.IsNullOrWhiteSpace(settings.Source))
    {
      ValidationHelper.ReportIsRequired(nameof(settings.Source));
      
      return 1;
    }
    
    if (string.IsNullOrWhiteSpace(settings.Examine))
    {
      ValidationHelper.ReportIsRequired(nameof(settings.Examine));
      
      return 1;
    }

    if (string.IsNullOrWhiteSpace(settings.Result))
    {
      ValidationHelper.ReportIsRequired(nameof(settings.Result));
      
      return 1;
    }
    
    if (ValidationHelper.IsUrl(settings.Examine) && !ValidationHelper.IsUrlValid(settings.Examine))
    {
      ValidationHelper.ReportInvalidUrl(settings.Examine);
      
      return 1;
    }
    
    if (ValidationHelper.IsUrl(settings.Examine))
    {
      using var client = new HttpClient();
      if (!await ValidationHelper.CheckUrlAccessibility(client, settings.Examine + "/$metadata"))
      {
        ValidationHelper.ReportNotAccessible(settings.Examine);
        
        return 1;
      }
    }

    try
    {
      var result = await DigitaleDeltaMetaDataValidator.Validator.ValidateAsync(settings.Source, settings.Examine).ConfigureAwait(true);
      ValidationHelper.WriteStringArrayToFile(result, settings.Result);

      ValidationHelper.ReportFinished($"Inspection result written to file '{settings.Result}'");
      return 0;
    }
    catch (Exception e)
    {
      ValidationHelper.ReportError(e.Message);
      
      return 1;
    }
  }
}