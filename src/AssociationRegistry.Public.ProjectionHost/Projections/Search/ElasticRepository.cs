namespace AssociationRegistry.Public.ProjectionHost.Projections.Search;

using Nest;
using Newtonsoft.Json;
using Schema.Search;

public class ElasticRepository : IElasticRepository
{
    private readonly IElasticClient _elasticClient;

    public ElasticRepository(IElasticClient elasticClient)
    {
        _elasticClient = elasticClient;
    }

    public async Task IndexAsync(VerenigingZoekDocument document)
    {
        var response = await _elasticClient.IndexDocumentAsync(document);

        if (!response.IsValid)
            throw new IndexDocumentFailed(response.DebugInformation);
    }

    public async Task UpdateAsync(string id, VerenigingZoekUpdateDocument update, long sequence)
    {
        var response = await _elasticClient.UpdateAsync<VerenigingZoekUpdateDocument>(id, u => u
                                                                                         .RetryOnConflict(3)
                                                                                         .Script(s => s
                                                                                             .Source(@"
            if (ctx._source.sequence == null || params.seq > ctx._source.sequence) {
                ctx._source.putAll(params.doc);
                ctx._source.sequence = params.seq;
            }
"
                                                                                              )
                                                                                             .Params(p => p
                                                                                                 .Add("doc", update)
                                                                                                 .Add("seq", sequence)
                                                                                              )
                                                                                          )
        );

        if (!response.IsValid)
            throw new IndexDocumentFailed(response.DebugInformation);
    }

    public async Task UpdateVerenigingsTypeAndClearSubverenigingVan<TDocument>(
        string id,
        string code,
        string naam,
        long sequence)
        where TDocument : class
    {
        var response = await _elasticClient.UpdateAsync<TDocument>(
            id,
            u => u.Script(s => s
                              .Source(@"
                    if (ctx._source.sequence == null || params.seq > ctx._source.sequence) {
                        ctx._source.subverenigingVan = null;
                        ctx._source.verenigingssubtype.code = params.code;
                        ctx._source.verenigingssubtype.naam = params.naam;
                        ctx._source.sequence = params.seq;
                    }")
                                                  .Params(p => p
                                                              .Add("code", code)
                                                              .Add("naam", naam)
                                                              .Add("seq", sequence))
                                       ));

        if (!response.IsValid)
            throw new IndexDocumentFailed(response.DebugInformation);
    }

    public async Task UpdateSubverenigingVanRelatie<TDocument>(
        string id,
        string andereVereniging,
        long sequence)
        where TDocument : class
    {
        var response = await _elasticClient.UpdateAsync<TDocument>(
            id,
            u => u.Script(s => s
                              .Source(@"
                    if (ctx._source.sequence == null || params.seq > ctx._source.sequence) {
                        ctx._source.subverenigingVan.andereVereniging = params.andereVereniging;
                        ctx._source.sequence = params.seq;
                    }")
                                                  .Params(p => p
                                                              .Add("andereVereniging", andereVereniging)
                                                              .Add("seq", sequence))
                                       ));

        if (!response.IsValid)
            throw new IndexDocumentFailed(response.DebugInformation);
    }
    public async Task UpdateSubverenigingVanDetail<TDocument>(
        string id,
        string identificatie,
        string beschrijving,
        long sequence)
        where TDocument : class
    {
        var response = await _elasticClient.UpdateAsync<TDocument>(
            id,
            u => u.Script(s => s
                              .Source(@"
                    if (ctx._source.sequence == null || params.seq > ctx._source.sequence) {
                        ctx._source.subverenigingVan.identificatie = params.identificatie;
                        ctx._source.subverenigingVan.beschrijving = params.beschrijving;
                        ctx._source.sequence = params.seq;
                    }")
                                                  .Params(p => p
                                                              .Add("identificatie", identificatie)
                                                              .Add("beschrijving", beschrijving)
                                                              .Add("seq", sequence))
                                       ));

        if (!response.IsValid)
            throw new IndexDocumentFailed(response.DebugInformation);
    }

    public async Task AppendLocatie(string id, VerenigingZoekDocument.Types.Locatie locatie, long sequence)
    {
        var response = await _elasticClient.UpdateAsync<VerenigingZoekDocument>(
            id,
            selector: u => u.Script(
                s => s
                    .Source("if (ctx._source.sequence == null || params.seq > ctx._source.sequence) {" +
                            "ctx._source.locaties.add(params.locatie);" +
                            "ctx._source.sequence = params.seq;"+
                            "}")
                    .Params(objects => objects.Add(key: "locatie", locatie).Add("seq", sequence))));

        if (!response.IsValid)
            throw new IndexDocumentFailed(response.DebugInformation);
    }

    public async Task AppendLidmaatschap(string id, VerenigingZoekDocument.Types.Lidmaatschap lidmaatschap, long sequence)
    {
        var response = await _elasticClient.UpdateAsync<VerenigingZoekDocument>(
            id,
            selector: u => u.Script(
                s => s
                    .Source("if (ctx._source.sequence == null || params.seq > ctx._source.sequence) {" +
                            "ctx._source.lidmaatschappen.add(params.lidmaatschap);" +
                            "ctx._source.sequence = params.seq;"+
                            "}")
                    .Params(objects => objects.Add(key: "lidmaatschap", lidmaatschap).Add("seq", sequence))));

        if (!response.IsValid)
            throw new IndexDocumentFailed(response.DebugInformation);
    }

    public async Task UpdateLidmaatschap(string id, VerenigingZoekDocument.Types.Lidmaatschap lidmaatschap, long sequence)
    {
        var response = await _elasticClient.UpdateAsync<VerenigingZoekDocument>(
            id,
            selector: u => u.Script(
                s => s
                    .Source(
                        "if (ctx._source.sequence == null || params.seq > ctx._source.sequence) {"+
                         "for(l in ctx._source.lidmaatschappen){" +
                         "   if(l.lidmaatschapId == params.lidmaatschapId){" +
                         "      for(p in params.lidmaatschap.entrySet()){" +
                         "         if(p.getValue() != null){" +
                         "            l[p.getKey()] = p.getValue();" +
                         "         }" +
                         "      }" +
                         "   }" +
                         "}"+
                         "ctx._source.sequence = params.seq;"+
                        "}")
                    .Params(objects => objects
                                      .Add(key: "lidmaatschapId", lidmaatschap.LidmaatschapId)
                                      .Add(key: "lidmaatschap", lidmaatschap)
                                      .Add("seq", sequence))));

        if (!response.IsValid)
            throw new IndexDocumentFailed(response.DebugInformation);
    }

    public async Task RemoveLidmaatschap(string id, int lidmaatschapId, long sequence)
    {
        var response = await _elasticClient.UpdateAsync<VerenigingZoekDocument>(
            id,
            selector: u => u.Script(
                s => s
                    .Source("if (ctx._source.sequence == null || params.seq > ctx._source.sequence) {"+
                            "ctx._source.lidmaatschappen.removeIf(l -> l.lidmaatschapId == params.lidmaatschapId);"+
                            "ctx._source.sequence = params.seq;"+
                     "}")
                    .Params(objects => objects.Add(key: "lidmaatschapId", lidmaatschapId).Add("seq", sequence))));

        if (!response.IsValid)
            throw new IndexDocumentFailed(response.DebugInformation);
    }

    public async Task UpdateLocatie(string id, VerenigingZoekDocument.Types.Locatie locatie, long sequence)
    {
        var response = await _elasticClient.UpdateAsync<VerenigingZoekDocument>(
            id,
            selector: u => u.Script(
                s => s
                    .Source(
                         "if (ctx._source.sequence == null || params.seq > ctx._source.sequence) {"+
                         "for(l in ctx._source.locaties){" +
                         "   if(l.locatieId == params.locatieId){" +
                         "      for(p in params.locatie.entrySet()){" +
                         "         if(p.getValue() != null){" +
                         "            l[p.getKey()] = p.getValue();" +
                         "         }" +
                         "      }" +
                         "   }" +
                         "}" +
                         "ctx._source.sequence = params.seq;"+
                         "}")
                    .Params(objects => objects
                                      .Add(key: "locatieId", locatie.LocatieId)
                                      .Add(key: "locatie", locatie)
                                      .Add("seq", sequence))));

        if (!response.IsValid)
            throw new IndexDocumentFailed(response.DebugInformation);
    }

    public async Task UpdateAdres(string id, int locatieId, string adresVoorstelling, string postcode, string gemeente, long sequence)
    {
        var response = await _elasticClient.UpdateAsync<VerenigingZoekDocument>(
            id,
            selector: u => u.Script(
                s => s
                    .Source(
                         "if (ctx._source.sequence == null || params.seq > ctx._source.sequence) {"+
                         "for (l in ctx._source.locaties) {" +
                         "    if (l.locatieId == params.locatieId) {" +
                         "        if (params.adresvoorstelling != null) l.adresvoorstelling = params.adresvoorstelling;" +
                         "        if (params.postcode != null) l.postcode = params.postcode;" +
                         "        if (params.gemeente != null) l.gemeente = params.gemeente;" +
                         "    }" +
                         "}"+
                         "ctx._source.sequence = params.seq;"+
                         "}")
                    .Params(objects => objects
                                      .Add("locatieId", locatieId)
                                      .Add("adresvoorstelling", adresVoorstelling)
                                      .Add("postcode", postcode)
                                      .Add("gemeente", gemeente)
                                       .Add("seq", sequence)
                     )));

        if (!response.IsValid)
            throw new IndexDocumentFailed(response.DebugInformation);
    }

    public async Task Remove(string id)
    {
        var deleteResponse = await _elasticClient.DeleteAsync<VerenigingZoekDocument>(id);

        if (!deleteResponse.IsValid)
            throw new IndexDocumentFailed(deleteResponse.DebugInformation);
    }

    public async Task RemoveLocatie(string id, int locatieId, long sequence)
    {
        var response = await _elasticClient.UpdateAsync<VerenigingZoekDocument>(
            id,
            selector: u => u.Script(
                s => s
                    .Source("if (ctx._source.sequence == null || params.seq > ctx._source.sequence) {"+
                            "ctx._source.locaties.removeIf(l -> l.locatieId == params.locatieId);"+
                            "ctx._source.sequence = params.seq;"+
                            "}")
                    .Params(objects => objects.Add(key: "locatieId", locatieId).Add("seq", sequence))));

        if (!response.IsValid)
            throw new IndexDocumentFailed(response.DebugInformation);
    }

    public async Task AppendRelatie(string id, VerenigingZoekDocument.Types.Relatie relatie, long sequence)
    {
        var response = await _elasticClient.UpdateAsync<VerenigingZoekDocument>(
            id,
            selector: u => u.Script(
                s => s
                    .Source("if (ctx._source.sequence == null || params.seq > ctx._source.sequence) {"+
                            "   ctx._source.relaties.add(params.relatie);" +
                            "ctx._source.sequence = params.seq;"+
                            "}")
                    .Params(objects => objects.Add(key: "relatie", relatie).Add("seq", sequence))));

        if (!response.IsValid)
            throw new IndexDocumentFailed(response.DebugInformation);
    }
}
