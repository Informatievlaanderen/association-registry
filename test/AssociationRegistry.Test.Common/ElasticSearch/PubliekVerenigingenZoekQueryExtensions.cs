namespace AssociationRegistry.Test.Common.ElasticSearch;

using Nest;
using Public.Api.Queries;
using Public.Schema.Search;

public static class PubliekVerenigingenZoekQueryExtensions
{

    public static async Task<ExplainResponse<VerenigingZoekDocument>> Explain(this IElasticClient client, string vCode, string query, PubliekVerenigingenZoekFilter filter)
    {
        return await client.ExplainAsync<VerenigingZoekDocument>(vCode, descriptor => descriptor.Query(q => PubliekVerenigingenZoekQuery.MainQuery(filter, q)));
    }
}
