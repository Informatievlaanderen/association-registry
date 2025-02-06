namespace AssociationRegistry.Admin.Schema.Search;

using Nest;

public static class DuplicateDetectionDocumentMapping
{
    public const string DuplicateMunicipalityAnalyzer = "duplicate_mmunicipality_analyzer";
    public const string DuplicateAnalyzer = "duplicate_analyzer";
    public const string DuplicateFullNameAnalyzer = "duplicate_fullname_analyzer";

    public static TypeMappingDescriptor<DuplicateDetectionDocument> Get(TypeMappingDescriptor<DuplicateDetectionDocument> map)
        => map
           .Properties(
                descriptor => descriptor
                             .Keyword(
                                  propertyDescriptor => propertyDescriptor
                                     .Name(document => document.VCode))
                             .Text(
                                  propertyDescriptor => propertyDescriptor
                                       .Name(document => document.Naam)
                                                       .WithKeyword(DuplicateAnalyzer)
                                       .Fields(fields => fields
                                                        .Text(subField => subField
                                                                         .Name("naam")
                                                                         .Analyzer(DuplicateAnalyzer)
                                                         )
                                                        .Text(subField => subField
                                                                         .Name("naamFull")
                                                                         .Analyzer(DuplicateFullNameAnalyzer)
                                                         )))
                             .Text(propertyDescriptor => propertyDescriptor
                                      .Name(document => document.KorteNaam)
                              )
                             .Text(propertyDescriptor => propertyDescriptor
                                      .Name(document => document.VerenigingsTypeCode)
                              )
                             .Text(propertyDescriptor => propertyDescriptor
                                      .Name(document => document.HoofdactiviteitVerenigingsloket))
                             .Boolean(
                                  propertyDescriptor => propertyDescriptor
                                     .Name(document => document.IsGestopt))
                             .Boolean(
                                  propertyDescriptor => propertyDescriptor
                                     .Name(document => document.IsVerwijderd))
                             .Boolean(
                                  propertyDescriptor => propertyDescriptor
                                     .Name(document => document.IsDubbel))
                             .Nested<DuplicateDetectionDocument.Locatie>(
                                  propertyDescriptor => propertyDescriptor
                                                       .Name(document => document.Locaties)
                                                       .IncludeInRoot()
                                                       .Properties(LocationMapping.Get))
            );

    private static class LocationMapping
    {
        public static IPromise<IProperties> Get(PropertiesDescriptor<DuplicateDetectionDocument.Locatie> map)
            => map
              .Text(
                   descriptor => descriptor
                      .Name(document => document.LocatieId))
              .Text(
                   propertyDescriptor => propertyDescriptor
                      .Name(document => document.Naam))
              .Text(
                   propertyDescriptor => propertyDescriptor
                      .Name(document => document.Adresvoorstelling))
              .Text(
                   propertyDescriptor => propertyDescriptor
                      .Name(document => document.IsPrimair))
              .Text(
                   propertyDescriptor => propertyDescriptor
                      .Name(document => document.Locatietype))
              .Text(
                   propertyDescriptor => propertyDescriptor
                      .Name(document => document.Postcode))
              .Text(
                   propertyDescriptor => propertyDescriptor
                                        .Name(document => document.Gemeente)
                                        .Analyzer(DuplicateMunicipalityAnalyzer));
    }
}
