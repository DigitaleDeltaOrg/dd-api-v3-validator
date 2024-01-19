﻿namespace DigitaleDeltaMetaDataValidator.Models;

public class ValidationCsv
{
  public string Entity         { set; get; } = null!;
  public bool   EntityRequired { set; get; }
  public string Property { set; get; } = null!;
  public string Type     { set; get; } = null!;
  public bool   PropertyRequired { set; get; }
}