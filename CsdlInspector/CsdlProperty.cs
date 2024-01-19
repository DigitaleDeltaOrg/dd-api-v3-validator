// ReSharper disable UnusedAutoPropertyAccessor.Global -- Used externally.
namespace CsdlPropertyInspector;

public class CsdlProperty
{
  public string Name              { internal init; get; } = null!;
  public string Type              { internal set;  get; } = null!;
  public bool   IsCollection      { internal set;  get; }
  public bool   IsComplex         { internal set;  get; }
  public bool   IsNavigational    { internal set;  get; }
  public bool   IsNullable        { internal set;  get; }
  public bool   IsEntity          { internal set;  get; }
  public bool   IsStructured      { internal set;  get; }
  public bool   IsKey             { internal init; get; }
  public bool   IsPath            { internal set;  get; }
  public bool   IsEntityReference { internal set;  get; }
  public bool   IsEnum            { internal set;  get; }
  public bool   IsTypeDefinition  { internal set;  get; }
  public bool   IsPrimitive       { internal set;  get; }
  public bool   IsBoolean         { internal set;  get; }
  public bool   IsTemporal        { internal set;  get; }
  public bool   IsDuration        { internal set;  get; }
  public bool   IsDate            { internal set;  get; }
  public bool   IsDateTimeOffset  { internal set;  get; }
  public bool   IsDecimal         { internal set;  get; }
  public bool   IsFloating        { internal set;  get; }
  public bool   IsSingle          { internal set;  get; }
  public bool   IsTimeOfDay       { internal set;  get; }
  public bool   IsDouble          { internal set;  get; }
  public bool   IsGuid            { internal set;  get; }
  public bool   IsSignedIntegral  { internal set;  get; }
  public bool   IsSByte           { internal set;  get; }
  public bool   IsInt16           { internal set;  get; }
  public bool   IsInt32           { internal set;  get; }
  public bool   IsInt64           { internal set;  get; }
  public bool   IsIntegral        { internal set;  get; }
  public bool   IsByte            { internal set;  get; }
  public bool   IsString          { internal set;  get; }
  public bool   IsUntyped         { internal set;  get; }
  public bool   IsBinary          { internal set;  get; }
  public bool   IsStream          { internal set;  get; }
  public bool   IsSpatial         { internal set;  get; }
  public bool   IsGeography       { internal set;  get; }
  public bool   IsGeometry        { internal set;  get; }
}