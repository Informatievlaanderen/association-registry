namespace AssociationRegistry.Public.Api.Infrastructure.Extensions;

using SearchVerenigingen;
using Nest;
using Nest.Specification.IndicesApi;

public static class ElasticClientExtentions
{
    public static void CreateVerenigingIndex(this IndicesNamespace indicesNamespace, IndexName index)
    =>indicesNamespace.Create(
        index,
        descriptor =>
            descriptor.Map<VerenigingDocument>(VerenigingDocumentMapping.Get));
}
