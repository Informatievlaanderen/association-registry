namespace AssociationRegistry.Public.Api.Extensions;

using Nest;
using Nest.Specification.IndicesApi;
using SearchVerenigingen;

public static class ElasticClientExtentions
{
    public static void CreateVerenigingIndex(this IndicesNamespace indicesNamespace, IndexName index)
    =>indicesNamespace.Create(
        index,
        descriptor =>
            descriptor.Map<VerenigingDocument>(VerenigingDocumentMapping.Get));
}
