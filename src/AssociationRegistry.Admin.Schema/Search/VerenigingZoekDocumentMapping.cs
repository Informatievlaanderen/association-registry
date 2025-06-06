namespace AssociationRegistry.Admin.Schema.Search;

using Nest;

public static class VerenigingZoekDocumentMapping
{
    public const string BeheerZoekenNormalizer = "beheer_zoeken_normalizer";
    public const string BeheerZoekenAnalyzer = "beheer_zoeken_analyzer";

    public static TypeMappingDescriptor<VerenigingZoekDocument> Get(TypeMappingDescriptor<VerenigingZoekDocument> map)
        => map.Properties(
            descriptor => descriptor
                         .Keyword(
                              propertyDescriptor => propertyDescriptor
                                                   .Normalizer(BeheerZoekenNormalizer)
                                                   .Name(document => document.VCode))
                         .Number(
                              propertyDescriptor => propertyDescriptor
                                                   .Name(document => document.Sequence)
                                                   .Index(false)
                                                   .DocValues(false)
                          )
                         .Text(
                              propertyDescriptor => propertyDescriptor
                                                   .Name(document => document.Naam)
                                                   .WithKeyword(BeheerZoekenNormalizer))
                         .Text(
                              propertyDescriptor => propertyDescriptor
                                                   .Name(document => document.KorteNaam)
                                                   .WithKeyword(BeheerZoekenNormalizer))
                         .Text(
                              propertyDescriptor => propertyDescriptor
                                                   .Name(document => document.Roepnaam)
                                                   .WithKeyword(BeheerZoekenNormalizer)
                                                   .Analyzer(BeheerZoekenAnalyzer))
                         .Date(
                              propertyDescriptor => propertyDescriptor
                                 .Name(document => document.Startdatum))
                         .Date(
                              propertyDescriptor => propertyDescriptor
                                 .Name(document => document.Einddatum))
                         .Keyword(
                              propertyDescriptor => propertyDescriptor
                                 .Name(document => document.Status))
                         .Boolean(
                              propertyDescriptor => propertyDescriptor
                                 .Name(document => document.IsUitgeschrevenUitPubliekeDatastroom))
                         .Boolean(
                              propertyDescriptor => propertyDescriptor
                                 .Name(document => document.IsVerwijderd))
                         .Boolean(
                              propertyDescriptor => propertyDescriptor
                                 .Name(document => document.IsDubbel))
                         .Text(
                              propertyDescriptor => propertyDescriptor
                                 .Name(document => document.JsonLdMetadataType))
                         .Nested<VerenigingZoekDocument.Types.Doelgroep>(
                              propertyDescriptor => propertyDescriptor
                                                   .Name(document => document.Doelgroep)
                                                   .IncludeInRoot()
                                                   .Properties(DoelgroepMapping.Get))
                         .Nested<VerenigingZoekDocument.Types.VerenigingsType>(
                              propertyDescriptor => propertyDescriptor
                                                   .Name(document => document.Verenigingstype)
                                                   .IncludeInRoot()
                                                   .Properties(VerenigingsTypeMapping.Get))
                         .Nested<VerenigingZoekDocument.Types.Locatie>(
                              propertyDescriptor => propertyDescriptor
                                                   .Name(document => document.Locaties)
                                                   .IncludeInRoot()
                                                   .Properties(LocationMapping.Get))
                         .Nested<VerenigingZoekDocument.Types.HoofdactiviteitVerenigingsloket>(
                              propertyDescriptor => propertyDescriptor
                                                   .Name(document => document.HoofdactiviteitenVerenigingsloket)
                                                   .IncludeInRoot()
                                                   .Properties(HoofdactiviteitMapping.Get))
                         .Nested<VerenigingZoekDocument.Types.Sleutel>(
                              propertyDescriptor => propertyDescriptor
                                                   .Name(document => document.Sleutels)
                                                   .IncludeInRoot()
                                                   .Properties(SleutelMapping.Get))
                         .Nested<VerenigingZoekDocument.Types.Lidmaatschap>(
                              propertyDescriptor => propertyDescriptor
                                                   .Name(document => document.Lidmaatschappen)
                                                   .IncludeInRoot()
                                                   .Properties(LidmaatschapMapping.Get))
                         .Nested<VerenigingZoekDocument.Types.Geotag>(
                              propertyDescriptor => propertyDescriptor
                                                   .Name(document => document.Geotags)
                                                   .IncludeInRoot()
                                                   .Properties(GeotagMapping.Get))

        );

    private static class LocationMapping
    {
        public static IPromise<IProperties> Get(PropertiesDescriptor<VerenigingZoekDocument.Types.Locatie> map)
            => map
              .Nested<JsonLdMetadata>(
                   propertyDescriptor => propertyDescriptor
                                        .Name(document => document.JsonLdMetadata)
                                        .IncludeInRoot()
                                        .Properties(JsonLdMetadataMapping.Get))
              .Text(
                   descriptor => descriptor
                                .Name(document => document.LocatieId)
                                .WithKeyword(BeheerZoekenNormalizer))
              .Text(
                   propertyDescriptor => propertyDescriptor
                                        .Name(document => document.Naam)
                                        .WithKeyword(BeheerZoekenNormalizer))
              .Text(
                   propertyDescriptor => propertyDescriptor
                                        .Name(document => document.Adresvoorstelling)
                                        .WithKeyword(BeheerZoekenNormalizer))
              .Text(
                   propertyDescriptor => propertyDescriptor
                                        .Name(document => document.IsPrimair)
                                        .WithKeyword())
              .Text(
                   propertyDescriptor => propertyDescriptor
                                        .Name(document => document.Postcode)
                                        .WithKeyword())
              .Text(
                   propertyDescriptor => propertyDescriptor
                                        .Name(document => document.Gemeente)
                                        .WithKeyword(BeheerZoekenNormalizer))
              .Text(
                   propertyDescriptor => propertyDescriptor
                                        .Name(document => document.Locatietype)
                                        .WithKeyword(BeheerZoekenNormalizer));
    }

    private static class LocationTypeMapping
    {
        public static IPromise<IProperties> Get(PropertiesDescriptor<VerenigingZoekDocument.Types.Locatie.LocatieType> map)
            => map.Nested<JsonLdMetadata>(
                       propertyDescriptor => propertyDescriptor
                                            .Name(document => document.JsonLdMetadata)
                                            .IncludeInRoot()
                                            .Properties(JsonLdMetadataMapping.Get))
                  .Text(
                       propertyDescriptor => propertyDescriptor
                                            .Name(document => document.Naam)
                                            .WithKeyword());
    }

    private static class HoofdactiviteitMapping
    {
        public static IPromise<IProperties> Get(PropertiesDescriptor<VerenigingZoekDocument.Types.HoofdactiviteitVerenigingsloket> map)
            => map
              .Nested<JsonLdMetadata>(
                   propertyDescriptor => propertyDescriptor
                                        .Name(document => document.JsonLdMetadata)
                                        .IncludeInRoot()
                                        .Properties(JsonLdMetadataMapping.Get))
              .Text(
                   propertiesDescriptor => propertiesDescriptor
                                          .Name(document => document.Code)
                                          .WithKeyword(BeheerZoekenNormalizer))
              .Text(
                   propertiesDescriptor => propertiesDescriptor
                                          .Name(document => document.Naam)
                                          .WithKeyword(BeheerZoekenNormalizer));
    }

    private static class VerenigingsTypeMapping
    {
        public static IPromise<IProperties> Get(PropertiesDescriptor<VerenigingZoekDocument.Types.VerenigingsType> map)
            => map
              .Keyword(
                   propertiesDescriptor => propertiesDescriptor
                                          .Name(document => document.Code)
                                          .Normalizer(BeheerZoekenNormalizer)
               )
              .Text(
                   propertiesDescriptor => propertiesDescriptor
                                          .Name(document => document.Naam)
                                          .WithKeyword(BeheerZoekenNormalizer));
    }

    private static class JsonLdMetadataMapping
    {
        public static IPromise<IProperties> Get(PropertiesDescriptor<JsonLdMetadata> map)
            => map
              .Text(
                   propertiesDescriptor => propertiesDescriptor
                      .Name(document => document.Id))
              .Text(
                   propertiesDescriptor => propertiesDescriptor
                      .Name(document => document.Type));
    }

    private static class DoelgroepMapping
    {
        public static IPromise<IProperties> Get(PropertiesDescriptor<VerenigingZoekDocument.Types.Doelgroep> map)
            => map
              .Nested<JsonLdMetadata>(
                   propertyDescriptor => propertyDescriptor
                                        .Name(document => document.JsonLdMetadata)
                                        .IncludeInRoot()
                                        .Properties(JsonLdMetadataMapping.Get))
              .Number(
                   propertiesDescriptor => propertiesDescriptor
                                          .Name(document => document.Minimumleeftijd)
                                          .Type(NumberType.Integer)
                                          .WithKeyword())
              .Number(
                   propertiesDescriptor => propertiesDescriptor
                                          .Name(document => document.Maximumleeftijd)
                                          .Type(NumberType.Integer)
                                          .WithKeyword());
    }

    private static class SleutelMapping
    {
        public static IPromise<IProperties> Get(PropertiesDescriptor<VerenigingZoekDocument.Types.Sleutel> map)
            => map
              .Nested<JsonLdMetadata>(
                   propertyDescriptor => propertyDescriptor
                                        .Name(document => document.JsonLdMetadata)
                                        .IncludeInRoot()
                                        .Properties(JsonLdMetadataMapping.Get))
              .Keyword(
                   propertiesDescriptor => propertiesDescriptor
                                          .Name(document => document.Bron)
                                          .Normalizer(BeheerZoekenNormalizer))
              .Text(
                   propertiesDescriptor => propertiesDescriptor
                                          .Name(document => document.Waarde)
                                          .WithKeyword(BeheerZoekenNormalizer))
              .Text(
                   propertiesDescriptor => propertiesDescriptor
                      .Name(document => document.CodeerSysteem))
              .Nested<VerenigingZoekDocument.Types.GestructureerdeIdentificator>(
                   propertyDescriptor => propertyDescriptor
                                        .Name(document => document.JsonLdMetadata)
                                        .IncludeInRoot()
                                        .Properties(GestructureerdeIdentificatorMapping.Get));
    }

    private static class LidmaatschapMapping
    {
        public static IPromise<IProperties> Get(PropertiesDescriptor<VerenigingZoekDocument.Types.Lidmaatschap> map)
            => map
              .Nested<JsonLdMetadata>(
                   propertyDescriptor => propertyDescriptor
                                        .Name(document => document.JsonLdMetadata)
                                        .IncludeInRoot()
                                        .Properties(JsonLdMetadataMapping.Get))
              .Text(
                   propertiesDescriptor => propertiesDescriptor
                                          .Name(document => document.AndereVereniging)
                                          .WithKeyword(BeheerZoekenNormalizer))
              .Text(
                   propertyDescriptor => propertyDescriptor
                      .Name(document => document.DatumVan))
              .Text(
                   propertyDescriptor => propertyDescriptor
                      .Name(document => document.DatumTot))
              .Text(
                   propertiesDescriptor => propertiesDescriptor
                                          .Name(document => document.Identificatie)
                                          .WithKeyword(BeheerZoekenNormalizer))
              .Text(
                   propertiesDescriptor => propertiesDescriptor
                                          .Name(document => document.Beschrijving)
                                          .WithKeyword(BeheerZoekenNormalizer));
    }

    private static class GestructureerdeIdentificatorMapping
    {
        public static IPromise<IProperties> Get(PropertiesDescriptor<VerenigingZoekDocument.Types.GestructureerdeIdentificator> map)
            => map
              .Nested<JsonLdMetadata>(
                   propertyDescriptor => propertyDescriptor
                                        .Name(document => document.JsonLdMetadata)
                                        .IncludeInRoot()
                                        .Properties(JsonLdMetadataMapping.Get))
              .Text(
                   propertiesDescriptor => propertiesDescriptor
                                          .Name(document => document.Nummer)
                                          .Fields(x => x.Keyword(y => y.Name("keyword"))));
    }

    private static class GeotagMapping
    {
        public static IPromise<IProperties> Get(PropertiesDescriptor<VerenigingZoekDocument.Types.Geotag> map)
            => map
               .Keyword(
                    propertyDescriptor => propertyDescriptor
                                         .Name(document => document.Identificatie)
                                         .Normalizer(BeheerZoekenNormalizer));
    }

}
