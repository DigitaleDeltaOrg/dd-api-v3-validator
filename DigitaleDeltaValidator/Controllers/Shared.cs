namespace DigitaleDeltaValidator.Controllers;

public class Shared
{
  internal static List<string> GetVersions(WebApplication webApplication, string folder)
  {
    var env        = webApplication.Environment;
    var folderPath = Path.Combine(env.WebRootPath, folder);
    var files      = Directory.GetFiles(folderPath);
  
    return files.Select(f => Path.GetFileName(f).Replace(".txt", "")).ToList();
  }
}