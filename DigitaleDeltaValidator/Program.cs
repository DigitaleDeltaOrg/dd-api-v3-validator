using System.Text.Json;
using DigitaleDeltaValidator;
using DigitaleDeltaValidator.Controllers;
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

app.MapGet("/currentversion", () => new CurrentVersionController().CurrentVersionHandler(app))
  .WithName("CurrentVersion")
  .WithSummary("Returns the current version.")
  .WithOpenApi();

app.MapGet("/versions", () => new VersionsController().VersionsHandler(app, folderName))
  .WithName("Versions")
  .WithSummary("Lists the available versions of Digitale Delta validation definitions.")
  .WithOpenApi();

app.MapGet("/validate", (HttpContext httpContext, [FromQuery] string url, [FromQuery] string? version) => new ValidationController(app, folderName).ValidateHandler(httpContext, url, version))
  .WithName("Validate")
  .WithSummary("Validates the specified Url for compliance with the Digitale Delta Property Definitions. URl is required. If version is omitted, the current default version is used.")
  .WithOpenApi();

app.Run();

return;
