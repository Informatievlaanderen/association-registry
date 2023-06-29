namespace AssociationRegistry.Admin.ProjectionHost.Projections.Search;

using System.Threading.Tasks;
using Nest;
using Schema.Search;

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

    public void Update<TDocument>(string id, TDocument update) where TDocument : class
    {
        var response = _elasticClient.Update<TDocument>(id, u => u.Doc(update));

        if (!response.IsValid)
        {
            // todo: log ? (should never happen in test/staging/production)
            throw new IndexDocumentFailed(response.DebugInformation);
        }
    }
    public async Task UpdateAsync<TDocument>(string id, TDocument update) where TDocument : class
    {
        var response = await _elasticClient.UpdateAsync<TDocument>(id, u => u.Doc(update));

        if (!response.IsValid)
        {
            // todo: log ? (should never happen in test/staging/production)
            throw new IndexDocumentFailed(response.DebugInformation);
        }
    }

    public async Task AppendLocatie(string id, VerenigingZoekDocument.Locatie locatie)
    {
        var response = await _elasticClient.UpdateAsync<VerenigingZoekDocument>(
            id,
            u => u.Script(
                s => s
                    .Source("ctx._source.locaties.add(params.item)")
                    .Params(objects => objects.Add("item", locatie))));

        if (!response.IsValid)
        {
            // todo: log ? (should never happen in test/staging/production)
            throw new IndexDocumentFailed(response.DebugInformation);
        }
    }

    public async Task RemoveLocatie(string id, int locatieId)
    {
        var response = await _elasticClient.UpdateAsync<VerenigingZoekDocument>(
            id,
            u => u.Script(
                s => s
                    .Source("ctx._source.locaties.removeIf(l -> l.locatieId == params.locatieId)")
                    .Params(objects => objects.Add("locatieId", locatieId))));

        if (!response.IsValid)
        {
            // todo: log ? (should never happen in test/staging/production)
            throw new IndexDocumentFailed(response.DebugInformation);
        }
    }
}
