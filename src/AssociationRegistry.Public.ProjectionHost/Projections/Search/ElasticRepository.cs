namespace AssociationRegistry.Public.ProjectionHost.Projections.Search;

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
            throw new IndexDocumentFailed(response.DebugInformation);
    }

    public async Task IndexAsync<TDocument>(TDocument document)
        where TDocument : class
    {
        var response = await _elasticClient.IndexDocumentAsync(document);

        if (!response.IsValid)
            throw new IndexDocumentFailed(response.DebugInformation);
    }

    public void Update<TDocument>(string id, TDocument update) where TDocument : class
    {
        var response = _elasticClient.Update<TDocument>(id, selector: u => u.Doc(update));

        if (!response.IsValid)
            throw new IndexDocumentFailed(response.DebugInformation);
    }

    public async Task UpdateAsync<TDocument>(string id, TDocument update) where TDocument : class
    {
        var response = await _elasticClient.UpdateAsync<TDocument>(id, selector: u => u.Doc(update));

        if (!response.IsValid)
            throw new IndexDocumentFailed(response.DebugInformation);
    }

    public async Task AppendLocatie(string id, VerenigingZoekDocument.Locatie locatie)
    {
        var response = await _elasticClient.UpdateAsync<VerenigingZoekDocument>(
            id,
            selector: u => u.Script(
                s => s
                    .Source("ctx._source.locaties.add(params.locatie)")
                    .Params(objects => objects.Add(key: "locatie", locatie))));

        if (!response.IsValid)
            throw new IndexDocumentFailed(response.DebugInformation);
    }

    public async Task ReplaceLocatie(string id, VerenigingZoekDocument.Locatie locatie)
    {
        var response = await _elasticClient.UpdateAsync<VerenigingZoekDocument>(
            id,
            selector: u => u.Script(
                s => s
                    .Source(
                         "ctx._source.locaties.removeIf(l -> l.locatieId == params.locatieId);" +
                         "ctx._source.locaties.add(params.item);" +
                         "ctx._source.locaties.sort((x,y) -> x.locatieId - y.locatieId);")
                    .Params(objects => objects.Add(key: "locatieId", locatie.LocatieId).Add(key: "item", locatie))));

        if (!response.IsValid)
            throw new IndexDocumentFailed(response.DebugInformation);
    }

    public async Task Remove(string id)
    {
        var deleteResponse = await _elasticClient.DeleteAsync<VerenigingZoekDocument>(id);

        if (!deleteResponse.IsValid)
            throw new IndexDocumentFailed(deleteResponse.DebugInformation);
    }

    public async Task RemoveLocatie(string id, int locatieId)
    {
        var response = await _elasticClient.UpdateAsync<VerenigingZoekDocument>(
            id,
            selector: u => u.Script(
                s => s
                    .Source("ctx._source.locaties.removeIf(l -> l.locatieId == params.locatieId)")
                    .Params(objects => objects.Add(key: "locatieId", locatieId))));

        if (!response.IsValid)
            throw new IndexDocumentFailed(response.DebugInformation);
    }
}
