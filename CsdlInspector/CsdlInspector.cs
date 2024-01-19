using System.Xml;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Csdl;

namespace CsdlPropertyInspector;

public static class CsdlInspector
{
  private static readonly Dictionary<string, CsdlType> Types         = new();

  private static readonly List<string> RequiredTypes = ["DigitaleDelta.Observation", "DigitaleDelta.ParameterContainer", "DigitaleDelta.Foi", "DigitaleDelta.Result" ];

  /// <summary>
  ///   Inspect the specified type within the CSDL and extract referenced types and their properties.
  /// </summary>
  /// <param name="csdl">CSDL to parse</param>
  /// <param name="type">Type to find in the CSDL.</param>
  /// <returns>Dictionary of type names with their type specifications and properties.</returns>
  /// <exception cref="Exception">
  ///   Exceptions are thrown when: CSDL is empty, type is not present in the CSDL, CSDL is
  ///   invalid.
  /// </exception>
  public static Dictionary<string, CsdlType> Inspect(string csdl, string type)
  {
    if (string.IsNullOrWhiteSpace(csdl))
    {
      throw new Exception("The provided CSDL cannot be null, empty, or consist only of white-space characters.");
    }

    var stringReader  = new StringReader(csdl);
    var xmlTextReader = new XmlTextReader(stringReader);
    var edmModel      = CsdlReader.Parse(xmlTextReader);
    var edmEntityType = GetTypeByName(edmModel, type);

    if (edmEntityType == null)
    {
      throw new Exception("Cannot find type in CSDL specification.");
    }

    InspectType(edmEntityType);

    return Types;
  }

  /// <summary>
  ///   Inspect the type. It checks the flags and properties. This function is recursive.
  /// </summary>
  /// <param name="edmType">EdmType to inspect.</param>
  private static void InspectType(IEdmStructuredType? edmType)
  {
    if (edmType == null || Types.ContainsKey(edmType.FullTypeName()))
    {
      return;
    }

    var csdlType = new CsdlType(edmType.FullTypeName())
    {
      IsTemporal  = edmType.IsTemporal(),
      IsDecimal   = edmType.IsDecimal(),
      IsString    = edmType.IsString(),
      IsUntyped   = edmType.IsUntyped(),
      IsBinary    = edmType.IsBinary(),
      IsStream    = edmType.IsStream(),
      IsSpatial   = edmType.IsSpatial(),
      IsGeography = edmType.IsGeography(),
      IsGeometry  = edmType.IsGeometry(),
      IsAbstract  = edmType.IsAbstract,
      IsOpen      = edmType.IsOpen,
      IsRequired  = RequiredTypes.Contains(edmType.FullTypeName())
    };

    Types.Add(edmType.FullTypeName(), csdlType);

    foreach (var property in edmType.DeclaredProperties)
    {
      InspectProperty(csdlType, property, false);
    }

    foreach (var navProperty in edmType.NavigationProperties())
    {
      InspectProperty(csdlType, navProperty, true);
    }
  }

  /// <summary>
  ///   Inspect a specific property.
  /// </summary>
  /// <param name="csdlType">CsdlType to fill</param>
  /// <param name="property">Property to inspect</param>
  /// <param name="isNavigational">Specifies if the property is a navigational property</param>
  private static void InspectProperty(CsdlType csdlType, IEdmProperty property, bool isNavigational)
  {
    IEdmStructuredType? structuredType = null;
    var                 csdlProperty   = CreateCsdlProperty(property, isNavigational);

    csdlType.Properties ??= [];

    if (property.Type.IsCollection())
    {
      var collectionElementType = property.Type.AsCollection().ElementType();
      if (collectionElementType.IsStructured())
      {
        csdlProperty.IsEntity = collectionElementType.IsEntity();
        structuredType        = collectionElementType.AsStructured().StructuredDefinition();
      }
    }
    else
    {
      if (property.Type.IsStructured())
      {
        structuredType = property.Type.AsStructured().StructuredDefinition();
      }
    }

    csdlType.Properties.Add(csdlProperty);
    InspectType(structuredType);
  }

  /// <summary>
  ///   Create a new CSDL property and fill it with the basic values taken from the specified property.
  /// </summary>
  /// <param name="property">EdmProperty</param>
  /// <param name="isNavigational">Specifies if the property is a navigational property</param>
  /// <returns>Newly created Csdl property</returns>
  private static CsdlProperty CreateCsdlProperty(IEdmProperty property, bool isNavigational)
  {
    return new CsdlProperty
    {
      IsNavigational    = isNavigational,
      IsCollection      = property.Type.IsCollection(),
      Name              = property.Name,
      Type              = property.Type.FullName(),
      IsKey             = property.IsKey(),
      IsStructured      = property.Type.IsStructured(),
      IsComplex         = property.Type.IsComplex(),
      IsNullable        = property.Type.IsNullable,
      IsEntity          = property.Type.IsEntity(),
      IsDate            = property.Type.IsDate(),
      IsBoolean         = property.Type.IsBoolean(),
      IsPath            = property.Type.IsPath(),
      IsEntityReference = property.Type.IsEntityReference(),
      IsEnum            = property.Type.IsEnum(),
      IsTypeDefinition  = property.Type.IsTypeDefinition(),
      IsPrimitive       = property.Type.IsPrimitive(),
      IsTemporal        = property.Type.IsTemporal(),
      IsDuration        = property.Type.IsDuration(),
      IsDateTimeOffset  = property.Type.IsDateTimeOffset(),
      IsDecimal         = property.Type.IsDecimal(),
      IsFloating        = property.Type.IsFloating(),
      IsSingle          = property.Type.IsSingle(),
      IsTimeOfDay       = property.Type.IsTimeOfDay(),
      IsDouble          = property.Type.IsDouble(),
      IsGuid            = property.Type.IsGuid(),
      IsSignedIntegral  = property.Type.IsSignedIntegral(),
      IsSByte           = property.Type.IsSByte(),
      IsInt16           = property.Type.IsInt16(),
      IsInt32           = property.Type.IsInt32(),
      IsInt64           = property.Type.IsInt64(),
      IsIntegral        = property.Type.IsIntegral(),
      IsByte            = property.Type.IsByte(),
      IsString          = property.Type.IsString(),
      IsUntyped         = property.Type.IsUntyped(),
      IsBinary          = property.Type.IsBinary(),
      IsStream          = property.Type.IsStream(),
      IsSpatial         = property.Type.IsSpatial(),
      IsGeography       = property.Type.IsGeography(),
      IsGeometry        = property.Type.IsGeometry()
    };
  }

  /// <summary>
  ///   Retrieve an EdmType by name from the model.
  /// </summary>
  /// <param name="edmModel">EdmModel</param>
  /// <param name="name">Name of the type to retrieve</param>
  /// <returns>EdmEntityType or null if type by name could not be found.</returns>
  private static IEdmEntityType? GetTypeByName(IEdmModel edmModel, string name)
  {
    return edmModel.SchemaElements.FirstOrDefault(a => a.Name == name) as IEdmEntityType;
  }
}