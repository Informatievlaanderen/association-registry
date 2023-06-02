namespace AssociationRegistry.Public.Schema.Search;

using Nest;

public static class VerenigingDocumentMapping
{
    public static TypeMappingDescriptor<VerenigingDocument> Get(TypeMappingDescriptor<VerenigingDocument> map)
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
                .Nested<VerenigingDocument.VerenigingsType>(
                    propertyDescriptor => propertyDescriptor
                        .Name(document => document.Type)
                        .IncludeInRoot()
                        .Properties(VerenigingsTypeMapping.Get))
                .Nested<VerenigingDocument.Locatie>(
                    propertyDescriptor => propertyDescriptor
                        .Name(document => document.Locaties)
                        .IncludeInRoot()
                        .Properties(LocationMapping.Get))
                .Nested<VerenigingDocument.HoofdactiviteitVerenigingsloket>(
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
                .Nested<VerenigingDocument.Sleutel>(
                    propertyDescriptor => propertyDescriptor
                        .Name(document => document.Type)
                        .IncludeInRoot()
                        .Properties(SleutelMapping.Get))
                .Nested<VerenigingDocument.Relatie>(
                    propertyDescriptor => propertyDescriptor
                        .Name(document => document.Type)
                        .IncludeInRoot()
                        .Properties(RelatieMapping.Get))
        );

    private static class LocationMapping
    {
        public static IPromise<IProperties> Get(PropertiesDescriptor<VerenigingDocument.Locatie> map)
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
        public static IPromise<IProperties> Get(PropertiesDescriptor<VerenigingDocument.HoofdactiviteitVerenigingsloket> map)
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
        public static IPromise<IProperties> Get(PropertiesDescriptor<VerenigingDocument.VerenigingsType> map)
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
        public static IPromise<IProperties> Get(PropertiesDescriptor<VerenigingDocument.Sleutel> map)
            => map
                .Text(
                    propertiesDescriptor => propertiesDescriptor
                        .Name(document => document.Bron)
                        .Fields(x => x.Keyword(y => y.Name("keyword"))))
                .Text(
                    propertiesDescriptor => propertiesDescriptor
                        .Name(document => document.Waarde));
    }

    private static class RelatieMapping
    {
        public static IPromise<IProperties> Get(PropertiesDescriptor<VerenigingDocument.Relatie> map)
            => map
                .Text(
                    propertiesDescriptor => propertiesDescriptor
                        .Name(document => document.Type)
                        .Fields(x => x.Keyword(y => y.Name("keyword"))))
                .Text(
                    propertiesDescriptor => propertiesDescriptor
                        .Name(document => document.Waarde));
    }
}
