using System;
using System.Linq;
using Xunit;

namespace CsdlPropertyInspector.Tests;

public class CsdlInspectorTests
{   
  private const string Edmx = 
    """
       <edmx:Edmx xmlns:edmx="http://docs.oasis-open.org/odata/ns/edmx" Version="4.0">
         <edmx:DataServices>
         <Schema xmlns="http://docs.oasis-open.org/odata/ns/edm" Namespace="DigitaleDelta">
           <EntityType Name="Reference">
             <Key>
               <PropertyRef Name="Id"/>
             </Key>
             <Property Name="Id" Type="Edm.String" Nullable="false"/>
             <Property Name="Type" Type="Edm.String"/>
             <Property Name="Organisation" Type="Edm.String"/>
             <Property Name="Code" Type="Edm.String" Nullable="false"/>
             <Property Name="Geometry" Type="Edm.GeometryPoint"/>
             <Property Name="Description" Type="Edm.String" Nullable="false"/>
             <Property Name="Role" Type="Edm.String"/>
           </EntityType>
           <EntityType Name="Observation">
             <Key>
               <PropertyRef Name="Id"/>
             </Key>
             <Property Name="Id" Type="Edm.String" Nullable="false"/>
             <Property Name="Type" Type="Edm.String" Nullable="false"/>
             <Property Name="ResultTime" Type="Edm.DateTimeOffset" Nullable="false"/>
             <Property Name="PhenomenonTime" Type="Edm.DateTimeOffset" Nullable="false"/>
             <Property Name="ValidTime" Type="Edm.DateTimeOffset"/>
             <Property Name="ResultQuality" Type="Collection(DigitaleDelta.DqElement)"/>
             <Property Name="Parameter" Type="DigitaleDelta.ParameterContainer" Nullable="false"/>
             <Property Name="Metadata" Type="DigitaleDelta.MetadataContainer"/>
             <NavigationProperty Name="Foi" Type="DigitaleDelta.Foi"/>
             <NavigationProperty Name="Result" Type="DigitaleDelta.Result" Nullable="false"/>
           </EntityType>
           <EntityType Name="RelatedObservation" BaseType="DigitaleDelta.Observation">
             <Property Name="Role" Type="Edm.String"/>
           </EntityType>
           <ComplexType Name="DqElement"/>
           <EntityType Name="Foi">
             <Key>
               <PropertyRef Name="Id"/>
             </Key>
             <Property Name="Id" Type="Edm.String" Nullable="false"/>
             <Property Name="Code" Type="Edm.String" Nullable="false"/>
             <Property Name="Description" Type="Edm.String" Nullable="false"/>
             <Property Name="Geometry" Type="Edm.GeometryPoint" Nullable="false"/>
           </EntityType>
           <ComplexType Name="ParameterContainer">
             <Property Name="Parameter" Type="Edm.String"/>
             <Property Name="Compartment" Type="Edm.String"/>
             <Property Name="Quantity" Type="Edm.String"/>
             <Property Name="Organisation" Type="Edm.String"/>
             <Property Name="LimitSymbol" Type="Edm.String"/>
             <Property Name="ParameterType" Type="Edm.String"/>
             <Property Name="Observed" Type="Edm.String"/>
             <Property Name="LifeForm" Type="Edm.String"/>
             <Property Name="LifeStage" Type="Edm.String"/>
             <Property Name="Gender" Type="Edm.String"/>
             <Property Name="Capacity" Type="Edm.String"/>
             <Property Name="LengthClass" Type="Edm.String"/>
             <Property Name="Statistics" Type="Edm.String"/>
             <Property Name="Sediment" Type="Edm.String"/>
             <Property Name="Wavelength" Type="Edm.String"/>
             <Property Name="Appearance" Type="Edm.String"/>
             <Property Name="Individuals" Type="Edm.String"/>
             <Property Name="GrainSizeFraction" Type="Edm.String"/>
             <Property Name="GrainDiameter" Type="Edm.String"/>
             <Property Name="Condition" Type="Edm.String"/>
             <Property Name="QualityAssessment" Type="Edm.String"/>
             <Property Name="LengthClassInCm" Type="Edm.String"/>
             <Property Name="LengthClassInMm" Type="Edm.String"/>
             <Property Name="WidthClassInCm" Type="Edm.String"/>
             <Property Name="WidthClassInMm" Type="Edm.String"/>
             <Property Name="ValuationTechnique" Type="Edm.String"/>
             <Property Name="Habitat" Type="Edm.String"/>
             <Property Name="MeasurementPosition" Type="Edm.String"/>
             <Property Name="ValuationMethod" Type="Edm.String"/>
             <Property Name="ValueProcessingMethod" Type="Edm.String"/>
           </ComplexType>
           <ComplexType Name="MetadataContainer">
             <Property Name="CollectionReference" Type="Edm.String"/>
             <Property Name="MeasurementSetNumber" Type="Edm.String"/>
             <Property Name="SampleComment" Type="Edm.String"/>
             <Property Name="MeasurementComment" Type="Edm.String"/>
           </ComplexType>
           <EntityType Name="Result">
             <Key>
               <PropertyRef Name="Id"/>
             </Key>
             <Property Name="Id" Type="Edm.String" Nullable="false"/>
             <Property Name="Truth" Type="Edm.Boolean"/>
             <Property Name="Count" Type="Edm.Int64"/>
             <Property Name="Measure" Type="DigitaleDelta.Measure"/>
             <Property Name="Vocab" Type="DigitaleDelta.CategoryVerb"/>
             <Property Name="Geometry" Type="Edm.GeometryPoint"/>
             <NavigationProperty Name="Timeseries" Type="DigitaleDelta.TimeseriesResult"/>
           </EntityType>
           <ComplexType Name="Measure">
             <Property Name="Uom" Type="Edm.String" Nullable="false"/>
             <Property Name="Value" Type="Edm.Decimal" Nullable="false" Scale="Variable"/>
           </ComplexType>
           <ComplexType Name="CategoryVerb">
             <Property Name="Vocabulary" Type="Edm.String" Nullable="false"/>
             <Property Name="Term" Type="Edm.String" Nullable="false"/>
           </ComplexType>
           <EntityType Name="TimeseriesResult">
             <Key>
               <PropertyRef Name="Id"/>
             </Key>
             <Property Name="Id" Type="Edm.String" Nullable="false"/>
             <Property Name="Type" Type="Edm.String" Nullable="false"/>
             <Property Name="MetaData" Type="DigitaleDelta.TimeseriesMetadata"/>
             <Property Name="DefaultPointMetaData" Type="DigitaleDelta.PointMetadata"/>
             <Property Name="Points" Type="Collection(DigitaleDelta.PointData)" Nullable="false"/>
           </EntityType>
           <ComplexType Name="TimeseriesMetadata" Abstract="true">
             <Property Name="TemporalExtent" Type="Edm.String"/>
             <Property Name="BaseTime" Type="Edm.DateTimeOffset" Nullable="false"/>
             <Property Name="Spacing" Type="Edm.String"/>
             <Property Name="CommentBlock" Type="DigitaleDelta.CommentBlock"/>
             <Property Name="CommentBlocks" Type="Collection(DigitaleDelta.CommentBlock)"/>
             <Property Name="IntendedObservationSpacing" Type="Edm.String"/>
             <Property Name="Cumulative" Type="Edm.Boolean"/>
             <Property Name="AccumulationAnchorTime" Type="Edm.DateTimeOffset"/>
             <Property Name="StartAnchorPoint" Type="Edm.DateTimeOffset"/>
             <Property Name="EndAnchorPoint" Type="Edm.DateTimeOffset"/>
             <Property Name="MaxGapPeriod" Type="Edm.DateTimeOffset"/>
             <NavigationProperty Name="Status" Type="DigitaleDelta.Reference"/>
           </ComplexType>
           <ComplexType Name="CommentBlock">
             <Property Name="ApplicablePeriod" Type="Edm.String" Nullable="false"/>
             <Property Name="Comment" Type="Edm.String" Nullable="false"/>
           </ComplexType>
           <ComplexType Name="PointMetadata">
             <Property Name="Comment" Type="Edm.String"/>
             <Property Name="Accuracy" Type="DigitaleDelta.Measure"/>
             <Property Name="AggregationDuration" Type="Edm.DateTimeOffset"/>
             <NavigationProperty Name="Quality" Type="DigitaleDelta.Reference"/>
             <NavigationProperty Name="Uom" Type="DigitaleDelta.Reference"/>
             <NavigationProperty Name="InterpolationType" Type="DigitaleDelta.Reference"/>
             <NavigationProperty Name="NilReason" Type="DigitaleDelta.Reference"/>
             <NavigationProperty Name="CensoredReason" Type="DigitaleDelta.Reference"/>
             <NavigationProperty Name="RelatedObservation" Type="DigitaleDelta.Observation"/>
             <NavigationProperty Name="Qualifier" Type="DigitaleDelta.Reference"/>
             <NavigationProperty Name="Processing" Type="DigitaleDelta.Reference"/>
             <NavigationProperty Name="Source" Type="DigitaleDelta.Reference"/>
           </ComplexType>
           <ComplexType Name="PointData">
             <Property Name="Time" Type="Edm.DateTimeOffset" Nullable="false"/>
             <Property Name="Value" Type="Edm.Double" Nullable="false"/>
             <Property Name="MetaData" Type="DigitaleDelta.PointMetadata"/>
           </ComplexType>
         </Schema>
         <Schema xmlns="http://docs.oasis-open.org/odata/ns/edm" Namespace="Default">
           <EntityContainer Name="Container">
             <EntitySet Name="references" EntityType="DigitaleDelta.Reference">
               <NavigationPropertyBinding Path="TaxonGroup" Target="references"/>
               <NavigationPropertyBinding Path="TaxonParent" Target="references"/>
               <NavigationPropertyBinding Path="TaxonType" Target="references"/>
               <Annotation Term="Org.OData.Capabilities.V1.SortRestrictions">
               <Record>
                 <PropertyValue Property="Sortable" Bool="true"/>
                 <PropertyValue Property="AscendingOnlyProperties">
                   <Collection/>
                 </PropertyValue>
                 <PropertyValue Property="DescendingOnlyProperties">
                   <Collection/>
                 </PropertyValue>
                 <PropertyValue Property="NonSortableProperties">
                   <Collection>
                     <PropertyPath>TaxonTypeId</PropertyPath>
                     <PropertyPath>TaxonGroupId</PropertyPath>
                     <PropertyPath>TaxonParentId</PropertyPath>
                     <PropertyPath>Geometry</PropertyPath>
                   </Collection>
                 </PropertyValue>
               </Record>
             </Annotation>
           </EntitySet>
           <EntitySet Name="observations" EntityType="DigitaleDelta.Observation">
             <NavigationPropertyBinding Path="RelatedObservations" Target="observations"/>
           </EntitySet>
         </EntityContainer>
         </Schema>
       </edmx:DataServices>
    </edmx:Edmx>
    """;
    
  [Fact]
  public void Inspect_ShouldThrowException_WhenCsdlIsEmpty()
  {
    Assert.Throws<Exception>(() => CsdlInspector.Inspect("", "someType"));
  }

  [Fact]
  public void Inspect_ShouldThrowException_WhenTypeNotInCsdl()
  {
    // This depends on GetTypeByName method implementation.
    // You could setup a mock to simulate the scenario
    // where type is not found in the CSDL.
    Assert.Throws<Exception>(() => CsdlInspector.Inspect(Edmx, "NonexistentType"));
  }
        
  [Fact]
  public void Inspect_ShouldNotReturnNull_WhenValidInputs()
  {
    var result = CsdlInspector.Inspect(Edmx, "Observation");
    Assert.NotNull(result);
  }

  [Fact] // More of an integration test... Observation should have an Id field, it must be a key, and it must be a string.
  public void Observation_ShouldHave()
  {
    var result = CsdlInspector.Inspect(Edmx, "Observation");
    Assert.NotNull(result);
    Assert.Contains("DigitaleDelta.Observation", result);
    Assert.NotNull(result["DigitaleDelta.Observation"].Properties);
    var id = result["DigitaleDelta.Observation"].Properties.SingleOrDefault(a => a is { Name: "Id", IsKey: true, IsString: true });
    Assert.NotNull(id);
      
    // Add test for required fields.
  }
}