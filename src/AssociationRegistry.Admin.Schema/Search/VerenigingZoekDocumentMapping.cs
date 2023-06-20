namespace AssociationRegistry.Admin.Schema.Search;

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
                        .Name(document => document.KorteNaam))
                .Nested<VerenigingZoekDocument.VerenigingsType>(
                    propertyDescriptor => propertyDescriptor
                        .Name(document => document.Type)
                        .IncludeInRoot()
                        .Properties(VerenigingsTypeMapping.Get))
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
                .Text(
                    propertyDescriptor => propertyDescriptor
                        .Name(document => document.Doelgroep))
                .Text(
                    propertyDescriptor => propertyDescriptor
                        .Name(document => document.Activiteiten))
                .Nested<VerenigingZoekDocument.Sleutel>(
                    propertyDescriptor => propertyDescriptor
                        .Name(document => document.Type)
                        .IncludeInRoot()
                        .Properties(SleutelMapping.Get))
        );

    private static class LocationMapping
    {
        public static IPromise<IProperties> Get(PropertiesDescriptor<VerenigingZoekDocument.Locatie> map)
            => map
                .Text(
                    propertyDescriptor => propertyDescriptor
                        .Name(document => document.Naam))
                .Text(
                    propertyDescriptor => propertyDescriptor
                        .Name(document => document.Adres))
                .Text(
                    propertyDescriptor => propertyDescriptor
                        .Name(document => document.Hoofdlocatie))
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
