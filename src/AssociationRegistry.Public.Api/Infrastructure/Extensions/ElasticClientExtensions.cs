namespace AssociationRegistry.Public.Api.Infrastructure.Extensions;

using ConfigurationBindings;
using Nest;
using Nest.Specification.IndicesApi;
using Schema.Search;
using System.Threading.Tasks;

public static class ElasticClientExtensions
{
    public static async Task<CreateIndexResponse> CreateVerenigingIndex(this IndicesNamespace indicesNamespace, IndexName index)
        => await indicesNamespace.CreateAsync(
            index,
            selector: descriptor =>
                descriptor.Map<VerenigingZoekDocument>(VerenigingZoekDocumentMapping.Get));

}
