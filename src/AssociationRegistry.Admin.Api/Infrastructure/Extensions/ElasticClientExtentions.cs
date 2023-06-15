namespace AssociationRegistry.Admin.Api.Infrastructure.Extensions;

using Nest;
using Nest.Specification.IndicesApi;
using Projections.Search.Schema;

public static class ElasticClientExtentions
{
    public static void CreateVerenigingIndex(this IndicesNamespace indicesNamespace, IndexName index)
        => indicesNamespace.Create(
            index,
            descriptor =>
                descriptor.Map<VerenigingZoekDocument>(VerenigingZoekDocumentMapping.Get));
}
