using System.Text.Json;
using DigitaleDeltaValidator;
using Microsoft.AspNetCore.Mvc;

const string folderName = "ValidationFiles";
var          builder    = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
var configuration = builder.Configuration;

if (string.IsNullOrWhiteSpace(configuration["CurrentVersion"]))
{
  Console.WriteLine("Error: CurrentVersion is not set in the appsettings.json. Please set a value for it, I.e. \"CurrentVersion\":\"2024.01\"");
  return;
}

var app = builder.Build();

app.UseStaticFiles();
app.UseSwaggerUI(c =>
{
  c.SwaggerEndpoint("/openapi.json", "Digitale Delta Validator");
});

app.UseHttpsRedirection();

app.MapGet("/currentversion", () => CurrentVersionHandler(app))
  .WithName("CurrentVersion")
  .WithSummary("Returns the current version.")
  .WithOpenApi();

app.MapGet("/versions", () => VersionsHandler(app, folderName))
  .WithName("Versions")
  .WithSummary("Lists the available versions of Digitale Delta validation definitions.")
  .WithOpenApi();

app.MapGet("/validate", (HttpContext httpContext, [FromQuery] string url, [FromQuery] string? version) => ValidateHandler(httpContext, app, folderName, url, version))
  .WithName("Validate")
  .WithSummary("Validates the specified Url for compliance with the Digitale Delta Property Definitions. URl is required. If version is omitted, the current default version is used.")
  .WithOpenApi();

app.Run();

return;

async Task<IResult> ValidateHandler(HttpContext httpContext, WebApplication webApplication, string folder, string url, string? version)
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
    Results.BadRequest("Content is not XML, or contains possible unsafe content");
  }

  var versions = GetVersions(webApplication, folder);
  if (string.IsNullOrEmpty(version))
  {
    version = webApplication.Configuration["CurrentVersion"];
  }

  if (!versions.Contains(version!))
  {
    Results.BadRequest($"Unknown version: {version}. Check {GetBaseUrl(httpContext)}/versions for supported versions.");
  }

  var versionFileContent = await File.ReadAllTextAsync(Path.Combine(webApplication.Environment.WebRootPath, folder, $"{version}.txt"));

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

IResult VersionsHandler(WebApplication webApplication, string s)
{
  return Results.Ok(GetVersions(webApplication, s));
}

IResult CurrentVersionHandler(WebApplication webApplication)
{
  return Results.Ok(webApplication.Configuration["CurrentVersion"]);
}

string GetBaseUrl(HttpContext httpContext)
{
  var request = httpContext.Request;
  var baseUrl = $"{request.Scheme}://{request.Host}{request.PathBase}";
  return baseUrl;
}