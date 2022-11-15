namespace AssociationRegistry.Public.Api.SearchVerenigingen;

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
                .Nested<VerenigingDocument.Locatie>(
                    propertyDescriptor => propertyDescriptor
                        .Name(document => document.Hoofdlocatie)
                        .IncludeInRoot()
                        .Properties(LocationMapping.Get))
                .Nested<VerenigingDocument.Locatie>(
                    propertyDescriptor => propertyDescriptor
                        .Name(document => document.Locaties)
                        .IncludeInRoot()
                        .Properties(LocationMapping.Get))
                .Nested<VerenigingDocument.Hoofdactiviteit>(
                    propertyDescriptor => propertyDescriptor
                        .Name(document => document.Hoofdactiviteiten)
                        .IncludeInRoot()
                        .Properties(HoofdactiviteitMapping.Get))
                .Text(
                    propertyDescriptor => propertyDescriptor
                        .Name(document => document.Doelgroep))
                .Text(
                    propertyDescriptor => propertyDescriptor
                        .Name(document => document.Activiteiten))
        );

    private static class LocationMapping
    {
        public static IPromise<IProperties> Get(PropertiesDescriptor<VerenigingDocument.Locatie> map)
            => map
                .Text(
                    propertyDescriptor => propertyDescriptor
                        .Name(document => document.Postcode))
                .Text(
                    propertyDescriptor => propertyDescriptor
                        .Name(document => document.Gemeente));
    }

    private static class HoofdactiviteitMapping
    {
        public static IPromise<IProperties> Get(PropertiesDescriptor<VerenigingDocument.Hoofdactiviteit> map)
            => map
                .Text(
                    propertiesDescriptor => propertiesDescriptor
                        .Name(document => document.Code)
                        .Fields(x => x.Keyword(y => y.Name("keyword"))))
                .Text(
                    propertiesDescriptor => propertiesDescriptor
                        .Name(document => document.Naam));
    }
}
