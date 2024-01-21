namespace DigitaleDeltaValidator.Controllers;

public class VersionsController
{
  internal IResult VersionsHandler(WebApplication webApplication, string s)
  {
    return Results.Ok(Shared.GetVersions(webApplication, s));
  }
}