namespace AssociationRegistry.Public.Api.Projections;

using System.Threading.Tasks;
using Nest;

public class ElasticRepository : IElasticRepository
{
    private readonly IElasticClient _elasticClient;

    public ElasticRepository(IElasticClient elasticClient)
    {
        _elasticClient = elasticClient;
    }

    public void Index<TDocument>(TDocument document)
        where TDocument : class
    {
        var response = _elasticClient.IndexDocument(document);

        if (!response.IsValid)
        {
            // todo: log ? (should never happen in test/staging/production)
            throw new IndexDocumentFailed(response.DebugInformation);
        }
    }

    public async Task IndexAsync<TDocument>(TDocument document)
        where TDocument : class
    {
        var response = await _elasticClient.IndexDocumentAsync(document);

        if (!response.IsValid)
        {
            // todo: log ? (should never happen in test/staging/production)
            throw new IndexDocumentFailed(response.DebugInformation);
        }
    }
}
