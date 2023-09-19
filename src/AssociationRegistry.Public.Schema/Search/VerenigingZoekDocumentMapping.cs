namespace AssociationRegistry.Public.Schema.Search;

using Nest;

public static class VerenigingZoekDocumentMapping
{
    public static TypeMappingDescriptor<VerenigingZoekDocument> Get(TypeMappingDescriptor<VerenigingZoekDocument> map)
        => map.Properties(
            descriptor => descriptor
                         .Keyword(
                              propertyDescriptor => propertyDescriptor
                                                   .Name(document => document.VCode)
                                                   .WithKeyword())
                         .Text(
                              propertyDescriptor => propertyDescriptor
                                                   .Name(document => document.Naam)
                                                   .WithKeyword())
                         .Text(
                              propertyDescriptor => propertyDescriptor
                                 .Name(document => document.Roepnaam))
                         .Text(
                              propertyDescriptor => propertyDescriptor
                                                   .Name(document => document.KorteNaam)
                                                   .WithKeyword())
                         .Keyword(
                              propertyDescriptor => propertyDescriptor
                                 .Name(document => document.Status))
                         .Boolean(
                              propertyDescriptor => propertyDescriptor
                                                   .Name(document => document.IsUitgeschrevenUitPubliekeDatastroom)
                                                   .WithKeyword())
                         .Nested<VerenigingZoekDocument.VerenigingsType>(
                              propertyDescriptor => propertyDescriptor
                                                   .Name(document => document.Type)
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
        );

    private static class LocationMapping
    {
        public static IPromise<IProperties> Get(PropertiesDescriptor<VerenigingZoekDocument.Locatie> map)
            => map
              .Number(
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
                                        .Name(document => document.Postcode)
                                        .WithKeyword())
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
                                          .Name(document => document.Code)
                                          .WithKeyword())
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
                                          .Name(document => document.Code)
                                          .WithKeyword())
              .Text(
                   propertiesDescriptor => propertiesDescriptor
                                          .Name(document => document.Beschrijving)
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
                                          .Name(document => document.Bron)
                                          .WithKeyword())
              .Text(
                   propertiesDescriptor => propertiesDescriptor
                                          .Name(document => document.Waarde)
                                          .WithKeyword());
    }
}
