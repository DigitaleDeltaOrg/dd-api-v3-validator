namespace DigitaleDeltaValidator.Controllers;

public class CurrentVersionController
{
  public IResult CurrentVersionHandler(WebApplication webApplication)
  {
    return Results.Ok(webApplication.Configuration["CurrentVersion"]);
  }
}