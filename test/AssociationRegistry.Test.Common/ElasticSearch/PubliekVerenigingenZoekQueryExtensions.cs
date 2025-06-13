namespace AssociationRegistry.Test.Common.ElasticSearch;

using Admin.Api.Queries;
using Nest;
using Public.Api.Queries;
using Public.Schema.Search;

public static class PubliekVerenigingenZoekQueryExtensions
{

    public static async Task<ExplainResponse<VerenigingZoekDocument>> ExplainQuery(this IElasticClient client, string vCode, PubliekVerenigingenZoekFilter filter)
    {
        return await client.ExplainAsync<VerenigingZoekDocument>(vCode, descriptor => descriptor.Query(q => PubliekVerenigingenZoekQuery.MainQuery(filter, q)));
    }

    public static async Task<ExplainResponse<Admin.Schema.Search.VerenigingZoekDocument>> ExplainAdminQuery(this IElasticClient client, string vCode, BeheerVerenigingenZoekFilter filter)
    {
        return await client.ExplainAsync<Admin.Schema.Search.VerenigingZoekDocument>(vCode, descriptor => descriptor.Query(q => BeheerVerenigingenZoekQuery.MainQuery(filter, q)));
    }
}
