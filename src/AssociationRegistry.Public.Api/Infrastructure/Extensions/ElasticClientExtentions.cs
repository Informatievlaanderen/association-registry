namespace AssociationRegistry.Public.Api.Infrastructure.Extensions;

using Nest;
using Nest.Specification.IndicesApi;
using Schema.Search;

public static class ElasticClientExtentions
{
    public static void CreateVerenigingIndex(this IndicesNamespace indicesNamespace, IndexName index)
    =>indicesNamespace.Create(
        index,
        descriptor =>
            descriptor.Map<VerenigingDocument>(VerenigingDocumentMapping.Get));
}
