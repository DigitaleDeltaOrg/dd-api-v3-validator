using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace DigitaleDeltaValidator.Controllers;

  public class ValidationController 
  {
    private readonly WebApplication _webApplication;
    private readonly string         _folder;

    public ValidationController(WebApplication webApplication, string folder)
    {
      _webApplication = webApplication;
      _folder         = folder;
    }

    public async Task<IResult> ValidateHandler(HttpContext httpContext, [FromQuery] string url, [FromQuery] string? version)
    {
      if (!url.EndsWith("/$metadata", StringComparison.CurrentCultureIgnoreCase))
      {
        url += "/$metadata";
      }

      if (!ValidationHelper.IsUrlSafe(url)) 
      {
        return Results.BadRequest("Invalid URL");
      }

      if (await ValidationHelper.IsContentSafeAsync(url).ConfigureAwait(false))
      {
        Results.BadRequest("Content is not XML, or contains possible unsafe content which could result in an XSS attack.");
      }

      var versions = GetVersions(_webApplication, _folder);
      if (string.IsNullOrEmpty(version))
      {
        version = _webApplication.Configuration["CurrentVersion"];
      }

      if (!versions.Contains(version!))
      {
        Results.BadRequest($"Unknown version: {version}. Check {GetBaseUrl(httpContext)}/versions for supported versions.");
      }

      var versionFileContent = await File.ReadAllTextAsync(Path.Combine(_webApplication.Environment.WebRootPath, _folder, $"{version}.txt"));

      var validationResult = await DigitaleDeltaMetaDataValidator.Validator.ValidateAsync(url, versionFileContent).ConfigureAwait(false);

      return validationResult == null 
        ? Results.BadRequest("Validation failed") 
        : Results.Ok(validationResult.Count == 0 
          ? "Validated without messages" 
          : JsonSerializer.Serialize(validationResult));
    }
    
    List<string> GetVersions(WebApplication webApplication, string folder)
    {
      var env        = webApplication.Environment;
      var folderPath = Path.Combine(env.WebRootPath, folder);
      var files      = Directory.GetFiles(folderPath);
  
      return files.Select(f => Path.GetFileName(f).Replace(".txt", "")).ToList();
    }

    string GetBaseUrl(HttpContext httpContext)
    {
      var request = httpContext.Request;
      var baseUrl = $"{request.Scheme}://{request.Host}{request.PathBase}";
      return baseUrl;
    }
  }