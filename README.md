# This C# solution (C# 12/.NET 8) contains a number of projects that help with validation of an DD-API V3 implementation.

Project CsdlInspector examines a provided CSDL (in string form) and extracts the relevant Entities and types from it in a form of a dictionary. Data types and whether the property is required, is also exposed.

Project ExportCsdlPropertiesToValidationFormat uses CsdlInspector to produce a CSV-export from the specified CSDL.

Project DigitaleDeltaMetaDataValidator is a library that uses CSDL inspector to check compliance of a provided CSDL with the specification stored in CSV format. Official CSV formats will be published.

Project DigitalDeltaValidator is a REST service, used as a front-end for DigitaleDeltaMetaDataValidator. An official site using this code, will provide official CSV definitions, which will be maintained by the Digitale Delta community.
