namespace AssociationRegistry.Public.Schema.Search;

using Detail;
using Nest;

public static class VerenigingZoekDocumentMapping
{
    public static TypeMappingDescriptor<VerenigingZoekDocument> Get(TypeMappingDescriptor<VerenigingZoekDocument> map)
        => map.Properties(
            descriptor => descriptor
                         .Keyword(
                              propertyDescriptor => propertyDescriptor
                                 .Name(document => document.VCode))
                         .Nested<JsonLdMetadata>(
                              propertyDescriptor => propertyDescriptor
                                                   .Name(document => document.JsonLdMetadata)
                                                   .IncludeInRoot()
                                                   .Properties(JsonLdMetadataMapping.Get))
                         .Text(
                              propertyDescriptor => propertyDescriptor
                                                   .Name(document => document.Naam)
                                                   .WithKeyword())
                         .Text(
                              propertyDescriptor => propertyDescriptor
                                                   .Name(document => document.Roepnaam)
                                                   .WithKeyword())
                         .Text(
                              propertyDescriptor => propertyDescriptor
                                                   .Name(document => document.KorteNaam)
                                                   .WithKeyword())
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
            => map.Nested<JsonLdMetadata>(
                       propertyDescriptor => propertyDescriptor
                                            .Name(document => document.JsonLdMetadata)
                                            .IncludeInRoot()
                                            .Properties(JsonLdMetadataMapping.Get))
                  .Text(
                       propertyDescriptor => propertyDescriptor
                                            .Name(document => document.LocatieId)
                                            .WithKeyword())
                  .Text(
                       propertyDescriptor => propertyDescriptor
                                            .Name(document => document.Naam)
                                            .WithKeyword())
                  .Text(
                       propertyDescriptor => propertyDescriptor
                                            .Name(document => document.Adresvoorstelling)
                                            .WithKeyword())
                  .Boolean(
                       propertyDescriptor => propertyDescriptor
                                            .Name(document => document.IsPrimair)
                                            .WithKeyword())
                  .Text(
                       propertyDescriptor => propertyDescriptor
                                            .Name(document => document.Locatietype)
                                            .WithKeyword())
                  .Keyword(
                       propertyDescriptor => propertyDescriptor
                          .Name(document => document.Postcode))
                  .Text(
                       propertyDescriptor => propertyDescriptor
                                            .Name(document => document.Gemeente)
                                            .WithKeyword());
    }

    private static class HoofdactiviteitMapping
    {
        public static IPromise<IProperties> Get(PropertiesDescriptor<VerenigingZoekDocument.HoofdactiviteitVerenigingsloket> map)
            => map
              .Keyword(
                   propertiesDescriptor => propertiesDescriptor
                      .Name(document => document.Code))
              .Nested<JsonLdMetadata>(
                   propertyDescriptor => propertyDescriptor
                                        .Name(document => document.JsonLdMetadata)
                                        .IncludeInRoot()
                                        .Properties(JsonLdMetadataMapping.Get))
              .Text(
                   propertiesDescriptor => propertiesDescriptor
                                          .Name(document => document.Naam)
                                          .WithKeyword());
    }

    private static class VerenigingsTypeMapping
    {
        public static IPromise<IProperties> Get(PropertiesDescriptor<VerenigingZoekDocument.VerenigingsType> map)
            => map
              .Keyword(
                   propertiesDescriptor => propertiesDescriptor
                      .Name(document => document.Code))
              .Text(
                   propertiesDescriptor => propertiesDescriptor
                                          .Name(document => document.Naam)
                                          .WithKeyword());
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
                      .Name(document => document.Bron))
              .Nested<JsonLdMetadata>(
                   propertyDescriptor => propertyDescriptor
                                        .Name(document => document.JsonLdMetadata)
                                        .IncludeInRoot()
                                        .Properties(JsonLdMetadataMapping.Get))
              .Text(
                   propertiesDescriptor => propertiesDescriptor
                                          .Name(document => document.Waarde)
                                          .WithKeyword())
              .Nested<VerenigingZoekDocument.GestructureerdeIdentificator>(
                   propertyDescriptor => propertyDescriptor
                                        .Name(document => document.GestructureerdeIdentificator)
                                        .IncludeInRoot()
                                        .Properties(GestructureerdeIdentificatorMapping.Get));
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

    private static class GestructureerdeIdentificatorMapping
    {
        public static IPromise<IProperties> Get(PropertiesDescriptor<VerenigingZoekDocument.GestructureerdeIdentificator> map)
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
}
