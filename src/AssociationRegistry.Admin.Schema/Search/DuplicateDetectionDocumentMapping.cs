namespace AssociationRegistry.Admin.Schema.Search;

using Nest;

public static class DuplicateDetectionDocumentMapping
{
    public const string DuplicateAnalyzer = "duplicate_analyzer";

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
                                                       .Analyzer(DuplicateAnalyzer))
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
                                        .Analyzer(DuplicateAnalyzer));
    }
}
