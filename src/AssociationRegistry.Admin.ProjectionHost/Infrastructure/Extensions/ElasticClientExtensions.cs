namespace AssociationRegistry.Admin.ProjectionHost.Infrastructure.Extensions;

using Schema.Search;
using Nest;
using Nest.Specification.IndicesApi;

public static class ElasticClientExtensions
{
    public static void CreateVerenigingIndex(this IndicesNamespace indicesNamespace, IndexName index)
        => indicesNamespace.Create(
            index,
            descriptor =>
                descriptor.Map<VerenigingZoekDocument>(VerenigingZoekDocumentMapping.Get));
}
