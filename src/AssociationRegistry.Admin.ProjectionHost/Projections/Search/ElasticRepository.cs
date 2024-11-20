namespace AssociationRegistry.Admin.ProjectionHost.Projections.Search;

using Constants;
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
            // todo: log ? (should never happen in test/staging/production)
            throw new IndexDocumentFailed(response.DebugInformation);
    }

    public async Task IndexAsync<TDocument>(TDocument document)
        where TDocument : class
    {
        var response = await _elasticClient.IndexDocumentAsync(document);

        if (!response.IsValid)
            // todo: log ? (should never happen in test/staging/production)
            throw new IndexDocumentFailed(response.DebugInformation);
    }

    public void Update<TDocument>(string id, TDocument update) where TDocument : class
    {
        var response = _elasticClient.Update<TDocument>(id, selector: u => u.Doc(update));

        if (!response.IsValid)
            // todo: log ? (should never happen in test/staging/production)
            throw new IndexDocumentFailed(response.DebugInformation);
    }

    public async Task UpdateAsync<TDocument>(string id, TDocument update) where TDocument : class
    {
        var response = await _elasticClient.UpdateAsync<TDocument>(id, selector: u => u.Doc(update));

        if (!response.IsValid)
            // todo: log ? (should never happen in test/staging/production)
            throw new IndexDocumentFailed(response.DebugInformation);
    }

    public async Task UpdateStartdatum<TDocument>(string id, DateOnly? startdatum) where TDocument : class
    {
        var response = await _elasticClient.UpdateAsync<TDocument>(
            id,
            selector: u => u.Script(
                s => startdatum.HasValue
                    ? s.Source("ctx._source.startdatum = params.item")
                       .Params(objects => objects.Add(key: "item", startdatum?.ToString(WellknownFormats.DateOnly)))
                    : s.Source("ctx._source.startdatum = null")
            ));

        if (!response.IsValid)
            // todo: log ? (should never happen in test/staging/production)
            throw new IndexDocumentFailed(response.DebugInformation);
    }

    public async Task<VerenigingZoekDocument.Locatie> GetLocatie(string id, int locatieId)
    {
        var response = await _elasticClient.GetAsync<VerenigingZoekDocument>(id);

        var locatie = response.Source.Locaties.SingleOrDefault(sod => sod.LocatieId == locatieId);
        return locatie;
    }

    public async Task AppendLocatie<T>(string id, ILocatie locatie) where T : class
    {
        var response = await _elasticClient.UpdateAsync<T>(
            id,
            selector: u => u.Script(
                s => s
                    .Source("if(! ctx._source.locaties.contains(params.item)){" +
                            "ctx._source.locaties.add(params.item)" +
                            "}")
                    .Params(objects => objects.Add(key: "item", locatie))));

        if (!response.IsValid)
            // todo: log ? (should never happen in test/staging/production)
            throw new IndexDocumentFailed(response.DebugInformation);
    }

    public async Task UpdateLocatie<T>(string id, ILocatie locatie) where T : class
    {
        var response = await _elasticClient.UpdateAsync<T>(
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
            // todo: log ? (should never happen in test/staging/production)
            throw new IndexDocumentFailed(response.DebugInformation);
    }

    public async Task UpdateAdres<T>(string id, int locatieId, string adresVoorstelling, string postcode, string gemeente) where T : class
    {
       var response = await _elasticClient.UpdateAsync<T>(
            id,
            selector: u => u.Script(
                s => s
                    .Source(
                         "for (l in ctx._source.locaties) {" +
                         "    if (l.locatieId == params.locatieId) {" +
                         "        if (params.adresvoorstelling != null) l.adresvoorstelling = params.adresvoorstelling;" +
                         "        if (params.postcode != null) l.postcode = params.postcode;" +
                         "        if (params.gemeente != null) l.gemeente = params.gemeente;" +
                         "    }" +
                         "}")
                    .Params(objects => objects
                                      .Add("locatieId", locatieId)
                                      .Add("adresvoorstelling", adresVoorstelling)
                                      .Add("postcode", postcode)
                                      .Add("gemeente", gemeente)
                     )));

        if (!response.IsValid)
            // todo: log ? (should never happen in test/staging/production)
            throw new IndexDocumentFailed(response.DebugInformation);
    }

    public async Task RemoveLocatie<T>(string id, int locatieId) where T : class
    {
        var response = await _elasticClient.UpdateAsync<T>(
            id,
            selector: u => u.Script(
                s => s
                    .Source("ctx._source.locaties.removeIf(l -> l.locatieId == params.locatieId)")
                    .Params(objects => objects.Add(key: "locatieId", locatieId))));

        if (!response.IsValid)
            // todo: log ? (should never happen in test/staging/production)
            throw new IndexDocumentFailed(response.DebugInformation);
    }

    public async Task AppendLidmaatschap<T>(string id, ILidmaatschap lidmaatschap) where T : class
    {
        var response = await _elasticClient.UpdateAsync<T>(
            id,
            selector: u => u.Script(
                s => s
                    .Source("ctx._source.lidmaatschappen.add(params.item)")
                    .Params(objects => objects.Add(key: "item", lidmaatschap))));

        if (!response.IsValid)
            // todo: log ? (should never happen in test/staging/production)
            throw new IndexDocumentFailed(response.DebugInformation);
    }

    public async Task UpdateLidmaatschap<T>(string id, ILidmaatschap lidmaatschap) where T : class
    {
        var response = await _elasticClient.UpdateAsync<T>(
            id,
            selector: u => u.Script(
                s => s
                    .Source(
                         "for(l in ctx._source.lidmaatschappen){" +
                         "   if(l.locatieId == params.lidmaatschapId){" +
                         "      for(p in params.lidmaatschap.entrySet()){" +
                         "         if(p.getValue() != null){" +
                         "            l[p.getKey()] = p.getValue();" +
                         "         }" +
                         "      }" +
                         "   }" +
                         "}")
                    .Params(objects => objects
                                      .Add(key: "locatieId", lidmaatschap.LidmaatschapId)
                                      .Add(key: "lidmaatschap", lidmaatschap))));

        if (!response.IsValid)
            // todo: log ? (should never happen in test/staging/production)
            throw new IndexDocumentFailed(response.DebugInformation);
    }

    public async Task RemoveLidmaatschap<T>(string id, int lidmaatschapId) where T : class
    {
        var response = await _elasticClient.UpdateAsync<T>(
            id,
            selector: u => u.Script(
                s => s
                    .Source("ctx._source.lidmaatschappen.removeIf(l -> l.lidmaatschapId == params.lidmaatschapId)")
                    .Params(objects => objects.Add(key: "lidmaatschapId", lidmaatschapId))));

        if (!response.IsValid)
            // todo: log ? (should never happen in test/staging/production)
            throw new IndexDocumentFailed(response.DebugInformation);
    }
}
