namespace AssociationRegistry.Public.Api.Projections;

using Nest;

public class ElasticRepository : IElasticRepository
{
    private readonly IElasticClient _elasticClient;

    public ElasticRepository(IElasticClient elasticClient)
    {
        _elasticClient = elasticClient;
    }

    public void Save<TDocument>(TDocument document)
        where TDocument : class
    {
        var response = _elasticClient.IndexDocument(document);

        if (!response.IsValid)
        {
            // todo: log ? (should never happen in test/staging/production)
            throw new IndexDocumentFailed(response.DebugInformation);
        }
    }
}
