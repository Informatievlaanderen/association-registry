namespace AssociationRegistry.Public.ProjectionHost.Projections.Search;

using Elasticsearch.Net;
using Nest;
using Schema.Search;
using Vereniging;

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
                    .Source("if(! ctx._source.locaties.contains(params.locatie)){" +
                            "ctx._source.locaties.add(params.locatie)" +
                            "}")
                    .Params(objects => objects.Add(key: "locatie", locatie))));

        if (!response.IsValid)
            throw new IndexDocumentFailed(response.DebugInformation);
    }

    public async Task UpdateLocatie(string id, VerenigingZoekDocument.Locatie locatie)
    {
        var response = await _elasticClient.UpdateAsync<VerenigingZoekDocument>(
            id,
            selector: u => u.Script(
                s => s
                    .Source(
                         "for(l in ctx._source.locaties){" +
                         "   if(l.locatieId == params.locatieId){" +
                         "      for(p in params.locatie.entrySet()){" +
                         "         if(p.getValue() != null){" +
                         "            l[p.getKey()] = p.getValue();" +
                         "         }" +
                         "      }" +
                         "   }" +
                         "}")
                    .Params(objects => objects
                                      .Add(key: "locatieId", locatie.LocatieId)
                                      .Add(key: "locatie", locatie))));

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

    public async Task AppendRelatie(string id, Relatie relatie)
    {
        var response = await _elasticClient.UpdateAsync<VerenigingZoekDocument>(
            id,
            selector: u => u.Script(
                s => s
                    .Source("if(! ctx._source.relaties.contains(params.relatie)){" +
                            "   ctx._source.relaties.add(params.relatie)" +
                            "}")
                    .Params(objects => objects.Add(key: "relatie", relatie))));

        if (!response.IsValid)
            throw new IndexDocumentFailed(response.DebugInformation);
    }
    public async Task WijzigNaamAfdeling(string vCode, string nieuweNaam)
    {
        var afdeling = _elasticClient.Get<VerenigingZoekDocument>(vCode);

        if (afdeling.Source.Verenigingstype.Code == Verenigingstype.Afdeling.Code)
        {
            var bulkResponse = await _elasticClient.BulkAsync(b => b
                                                                   // Update the Association's name and perform version check
                                                                  .Update<VerenigingZoekDocument>(u => u
                                                                          .Id(vCode)
                                                                          .IfSequenceNumber(afdeling.SequenceNumber)
                                                                      .IfPrimaryTerm(afdeling.PrimaryTerm)
                                                                          .Doc(new VerenigingZoekDocument
                                                                                   { Naam = nieuweNaam }))
                                                                   // Update the Mother Association's relation's name and perform version check
                                                                  .Update<VerenigingZoekDocument>(u => u
                                                                          .Id(afdeling.Source.Relaties.First().AndereVereniging.VCode)
                                                                          .ScriptedUpsert()
                                                                          .Script(s => s
                                                                                      .Source(
                                                                                           "for (relatie in ctx._source.relaties) { if (relatie.andereVereniging.vCode == params.associationId) { relatie.andereVereniging.naam = params.newName; } }")
                                                                                      .Params(p => p
                                                                                              .Add(key: "newName", nieuweNaam)
                                                                                              .Add(key: "associationId", vCode))))
                                                                  .Refresh(Refresh.True));
        }
        else
        {
            await UpdateAsync(
                vCode,
                new VerenigingZoekDocument
                {
                    Naam = nieuweNaam,
                }
            );
        }
    }
}
