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

    public static async Task EnsureIndexExists(this IElasticClient elasticClient, ElasticSearchOptionsSection options)
    {
        var verenigingenIndexName = options.Indices!.Verenigingen;

        if (!(await elasticClient.Indices.ExistsAsync(verenigingenIndexName)).Exists)
        {
            var response = await elasticClient.Indices.CreateVerenigingIndex(verenigingenIndexName);

            if (!response.IsValid)
                throw response.OriginalException;
        }
    }
}
