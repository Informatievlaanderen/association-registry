namespace AssociationRegistry.Public.Schema.Search;

using Nest;

public static class VerenigingZoekDocumentMapping
{
    public const string PubliekZoekenNormalizer = "publiek_zoeken_normalizer";
    public const string PubliekZoekenAnalyzer = "publiek_zoeken_analyzer";


    public static TypeMappingDescriptor<VerenigingZoekDocument> Get(TypeMappingDescriptor<VerenigingZoekDocument> map)
        => map.Properties(
            descriptor => descriptor
                         .Keyword(
                              propertyDescriptor => propertyDescriptor
                                 .Name(document => document.VCode)
                                                   .Normalizer(PubliekZoekenNormalizer))
                         .Text(
                              propertyDescriptor => propertyDescriptor
                                                   .Name(document => document.Naam)
                                                   .WithKeyword(PubliekZoekenNormalizer)
                                                   .Analyzer(PubliekZoekenAnalyzer))
                         .Text(
                              propertyDescriptor => propertyDescriptor
                                                   .Name(document => document.Roepnaam)
                                                   .WithKeyword(PubliekZoekenNormalizer)
                                                   .Analyzer(PubliekZoekenAnalyzer))
                         .Text(
                              propertyDescriptor => propertyDescriptor
                                                   .Name(document => document.KorteNaam)
                                                   .WithKeyword(PubliekZoekenNormalizer)
                                                   .Analyzer(PubliekZoekenAnalyzer))
                         .Text(
                              propertyDescriptor => propertyDescriptor
                                                   .Name(document => document.KorteBeschrijving)
                                                   .WithKeyword(PubliekZoekenNormalizer)
                                                   .Analyzer(PubliekZoekenAnalyzer))
                         .Keyword(
                              propertyDescriptor => propertyDescriptor
                                 .Name(document => document.Status))
                         .Boolean(
                              propertyDescriptor => propertyDescriptor
                                 .Name(document => document.IsUitgeschrevenUitPubliekeDatastroom))
                         .Boolean(
                              propertyDescriptor => propertyDescriptor
                                 .Name(document => document.IsVerwijderd))
                         .Nested<VerenigingZoekDocument.VerenigingsType>(
                              propertyDescriptor => propertyDescriptor
                                                   .Name(document => document.Verenigingstype)
                                                   .IncludeInRoot()
                                                   .Properties(VerenigingsTypeMapping.Get))
                         .Nested<Doelgroep>(
                              propertyDescriptor => propertyDescriptor
                                                   .Name(document => document.Doelgroep)
                                                   .IncludeInRoot()
                                                   .Properties(DoelgroepMapping.Get))
                         .Nested<VerenigingZoekDocument.Locatie>(
                              propertyDescriptor => propertyDescriptor
                                                   .Name(document => document.Locaties)
                                                   .IncludeInRoot()
                                                   .Properties(LocationMapping.Get))
                         .Nested<VerenigingZoekDocument.HoofdactiviteitVerenigingsloket>(
                              propertyDescriptor => propertyDescriptor
                                                   .Name(document => document.HoofdactiviteitenVerenigingsloket)
                                                   .IncludeInRoot()
                                                   .Properties(HoofdactiviteitMapping.Get))
                         .Nested<VerenigingZoekDocument.Sleutel>(
                              propertyDescriptor => propertyDescriptor
                                                   .Name(document => document.Sleutels)
                                                   .IncludeInRoot()
                                                   .Properties(SleutelMapping.Get))
                         .Nested<Relatie>(
                              propertyDescriptor => propertyDescriptor
                                                   .Name(document => document.Relaties)
                                                   .IncludeInRoot()
                                                   .Properties(RelatieMapping.Get))
        );

    private static class LocationMapping
    {
        public static IPromise<IProperties> Get(PropertiesDescriptor<VerenigingZoekDocument.Locatie> map)
            => map
              .Text(
                   propertyDescriptor => propertyDescriptor
                                        .Name(document => document.LocatieId)
                                        .WithKeyword())
              .Text(
                   propertyDescriptor => propertyDescriptor
                                        .Name(document => document.Naam)
                                        .WithKeyword(PubliekZoekenNormalizer)
                                        .Analyzer(PubliekZoekenAnalyzer))
              .Text(
                   propertyDescriptor => propertyDescriptor
                                        .Name(document => document.Adresvoorstelling)
                                        .WithKeyword(PubliekZoekenNormalizer)
                                        .Analyzer(PubliekZoekenAnalyzer))
              .Boolean(
                   propertyDescriptor => propertyDescriptor
                                        .Name(document => document.IsPrimair)
                                        .WithKeyword())
              .Text(
                   propertyDescriptor => propertyDescriptor
                                        .Name(document => document.Locatietype)
                                        .WithKeyword(PubliekZoekenNormalizer))
              .Text(
                   propertyDescriptor => propertyDescriptor
                      .Name(document => document.Postcode)
                                        .WithKeyword())
              .Text(
                   propertyDescriptor => propertyDescriptor
                                        .Name(document => document.Gemeente)
                                        .WithKeyword(PubliekZoekenNormalizer)
                                        .Analyzer(PubliekZoekenAnalyzer));
    }

    private static class HoofdactiviteitMapping
    {
        public static IPromise<IProperties> Get(PropertiesDescriptor<VerenigingZoekDocument.HoofdactiviteitVerenigingsloket> map)
            => map
              .Keyword(
                   propertiesDescriptor => propertiesDescriptor
                      .Name(document => document.Code))
              .Text(
                   propertiesDescriptor => propertiesDescriptor
                                          .Name(document => document.Naam)
                                          .WithKeyword(PubliekZoekenNormalizer)
                                          .Analyzer(PubliekZoekenAnalyzer));
    }

    private static class VerenigingsTypeMapping
    {
        public static IPromise<IProperties> Get(PropertiesDescriptor<VerenigingZoekDocument.VerenigingsType> map)
            => map
              .Keyword(
                   propertiesDescriptor => propertiesDescriptor
                      .Name(document => document.Code)
                                          .Normalizer(PubliekZoekenNormalizer))
              .Text(
                   propertiesDescriptor => propertiesDescriptor
                                          .Name(document => document.Naam)
                                          .WithKeyword(PubliekZoekenNormalizer)
                                          .Analyzer(PubliekZoekenAnalyzer));
    }

    private static class DoelgroepMapping
    {
        public static IPromise<IProperties> Get(PropertiesDescriptor<Doelgroep> map)
            => map
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
        public static IPromise<IProperties> Get(PropertiesDescriptor<VerenigingZoekDocument.Sleutel> map)
            => map
              .Keyword(
                   propertiesDescriptor => propertiesDescriptor
                      .Name(document => document.Bron)
                                          .Normalizer(PubliekZoekenNormalizer))
              .Text(
                   propertiesDescriptor => propertiesDescriptor
                                          .Name(document => document.Waarde)
                                          .WithKeyword(PubliekZoekenNormalizer)
                                          .Analyzer(PubliekZoekenAnalyzer));
    }

    private static class RelatieMapping
    {
        public static IPromise<IProperties> Get(PropertiesDescriptor<Relatie> map)
            => map
              .Keyword(
                   propertiesDescriptor => propertiesDescriptor
                      .Name(document => document.Relatietype))
              .Nested<GerelateerdeVereniging>(
                   propertyDescriptor => propertyDescriptor
                                        .Name(document => document.AndereVereniging)
                                        .IncludeInRoot()
                                        .Properties(GerelateerdeVerenigingMapping.Get));

        private static class GerelateerdeVerenigingMapping
        {
            public static IPromise<IProperties> Get(PropertiesDescriptor<GerelateerdeVereniging> map)
                => map
                  .Keyword(
                       propertiesDescriptor => propertiesDescriptor
                          .Name(document => document.KboNummer))
                  .Keyword(
                       propertiesDescriptor => propertiesDescriptor
                          .Name(document => document.VCode))
                  .Text(
                       propertiesDescriptor => propertiesDescriptor
                          .Name(document => document.Naam));
        }
    }
}
