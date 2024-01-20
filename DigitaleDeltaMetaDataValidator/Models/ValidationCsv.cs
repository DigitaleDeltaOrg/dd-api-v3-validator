namespace DigitaleDeltaMetaDataValidator.Models;

public class ValidationCsv
{
  public string Entity           { init; get; } = null!;
  public bool   EntityRequired   { init; get; }
  public string Property         { init; get; } = null!;
  public string Type             { init; get; } = null!;
  public bool   PropertyRequired { init; get; }
}