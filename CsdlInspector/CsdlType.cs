namespace CsdlPropertyInspector;

public class CsdlType(string name)
{
  public string Name        { internal set;          get; } = name;
  public bool   IsTemporal  { internal set; get; }
  public bool   IsDecimal   { internal set; get; }
  public bool   IsString    { internal set; get; }
  public bool   IsUntyped   { internal set; get; }
  public bool   IsBinary    { internal set; get; }
  public bool   IsStream    { internal set; get; }
  public bool   IsSpatial   { internal set; get; }
  public bool   IsGeography { internal set; get; }
  public bool   IsGeometry  { internal set; get; }
  public bool   IsAbstract  { internal set; get; }
  public bool   IsOpen      { internal set; get; }
  public bool   IsRequired  { internal set; get; }
  public List<CsdlProperty>? Properties        { set;           get; } = [];
}