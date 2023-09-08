namespace AssociationRegistry.Public.Schema.Search;

using Nest;

public static class VerenigingZoekDocumentMapping
{
    public static TypeMappingDescriptor<VerenigingZoekDocument> Get(TypeMappingDescriptor<VerenigingZoekDocument> map)
        => map.Properties(
            descriptor => descriptor
                         .Keyword(
                              propertyDescriptor => propertyDescriptor
                                 .Name(document => document.VCode))
                         .Text(
                              propertyDescriptor => propertyDescriptor
                                 .Name(document => document.Naam))
                         .Text(
                              propertyDescriptor => propertyDescriptor
                                 .Name(document => document.Roepnaam))
                         .Text(
                              propertyDescriptor => propertyDescriptor
                                 .Name(document => document.KorteNaam))
                         .Keyword(
                              propertyDescriptor => propertyDescriptor
                                 .Name(document => document.Status))
                         .Boolean(
                              propertyDescriptor => propertyDescriptor
                                 .Name(document => document.IsUitgeschrevenUitPubliekeDatastroom))
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
              .Text(
                   propertyDescriptor => propertyDescriptor
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
                      .Name(document => document.Gemeente));
    }

    private static class HoofdactiviteitMapping
    {
        public static IPromise<IProperties> Get(PropertiesDescriptor<VerenigingZoekDocument.HoofdactiviteitVerenigingsloket> map)
            => map
              .Text(
                   propertiesDescriptor => propertiesDescriptor
                                          .Name(document => document.Code)
                                          .Fields(x => x.Keyword(y => y.Name("keyword"))))
              .Text(
                   propertiesDescriptor => propertiesDescriptor
                      .Name(document => document.Naam));
    }

    private static class VerenigingsTypeMapping
    {
        public static IPromise<IProperties> Get(PropertiesDescriptor<VerenigingZoekDocument.VerenigingsType> map)
            => map
              .Text(
                   propertiesDescriptor => propertiesDescriptor
                                          .Name(document => document.Code)
                                          .Fields(x => x.Keyword(y => y.Name("keyword"))))
              .Text(
                   propertiesDescriptor => propertiesDescriptor
                      .Name(document => document.Beschrijving));
    }

    private static class DoelgroepMapping
    {
        public static IPromise<IProperties> Get(PropertiesDescriptor<Doelgroep> map)
            => map
              .Number(
                   propertiesDescriptor => propertiesDescriptor
                                          .Name(document => document.Minimumleeftijd)
                                          .Type(NumberType.Integer))
              .Number(
                   propertiesDescriptor => propertiesDescriptor
                                          .Name(document => document.Maximumleeftijd)
                                          .Type(NumberType.Integer));
    }

    private static class SleutelMapping
    {
        public static IPromise<IProperties> Get(PropertiesDescriptor<VerenigingZoekDocument.Sleutel> map)
            => map
              .Text(
                   propertiesDescriptor => propertiesDescriptor
                                          .Name(document => document.Bron)
                                          .Fields(x => x.Keyword(y => y.Name("keyword"))))
              .Text(
                   propertiesDescriptor => propertiesDescriptor
                      .Name(document => document.Waarde));
    }
}
