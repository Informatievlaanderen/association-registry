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

    public async Task UpdateAsync<TDocument>(string id, TDocument update, long sequence) where TDocument : class
    {
        var response = await _elasticClient.UpdateAsync<TDocument>(id, u => u
                                                                           .RetryOnConflict(3)
                                                                           .Script(s => s
                                                                                       .Source(@"
            if (ctx._source.sequence == null || params.seq > ctx._source.sequence) {
                ctx._source.putAll(params.doc);
                ctx._source.sequence = params.seq;
            }"
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

    public async Task UpdateVerenigingsTypeAndClearSubverenigingVan<TDocument>(string id, string code, string naam, long sequence) where TDocument : class
    {
        var response = await _elasticClient.UpdateAsync<TDocument>(
            id,
            u => u
                .RetryOnConflict(3)
                .Script(s => s
                            .Source(
                                 "if (ctx._source.sequence == null || params.seq > ctx._source.sequence) {" +
                                 "ctx._source.subverenigingVan = null;" +
                                 "ctx._source.verenigingssubtype.code = params.code;" +
                                 "ctx._source.verenigingssubtype.naam = params.naam;" +
                                 "ctx._source.sequence = params.seq;" +
                                 "}")
                            .Params(p => p
                                        .Add("code", code)
                                        .Add("naam", naam)
                                        .Add("seq", sequence)
                             )
            )
        );

        if (!response.IsValid)
            throw new IndexDocumentFailed(response.DebugInformation);
    }


    public async Task UpdateStartdatum<TDocument>(string id, DateOnly? startdatum, long sequence) where TDocument : class
    {
        var response = await _elasticClient.UpdateAsync<TDocument>(
            id,
            selector: u => u
                .RetryOnConflict(3)
               .Script(
                s => startdatum.HasValue
                    ? s.Source("if (ctx._source.sequence == null || params.seq > ctx._source.sequence) {" +
                               "ctx._source.startdatum = params.item;" +
                               "ctx._source.sequence = params.seq;" +
                               "}")
                       .Params(objects => objects
                                  .Add(key: "item", startdatum?.ToString(WellknownFormats.DateOnly))
                                  .Add(key: "seq", sequence)
                               )
                    : s.Source("ctx._source.startdatum = null")
            ));

        if (!response.IsValid)
            // todo: log ? (should never happen in test/staging/production)
            throw new IndexDocumentFailed(response.DebugInformation);
    }

    public async Task AppendLocatie<T>(string id, ILocatie locatie, long sequence) where T : class
    {
        var response = await _elasticClient.UpdateAsync<T>(
            id,
            selector: u => u
               .RetryOnConflict(3)
               .Script(
                s => s
                    .Source("if (ctx._source.sequence == null || params.seq > ctx._source.sequence) {" +
                            "if(! ctx._source.locaties.contains(params.item)){" +
                            "ctx._source.locaties.add(params.item)" +
                            "}" +
                            "ctx._source.sequence = params.seq;" +
                            "}")
                    .Params(objects => objects
                               .Add(key: "item", locatie)
                               .Add(key: "seq", sequence)
                            )));

        if (!response.IsValid)
            // todo: log ? (should never happen in test/staging/production)
            throw new IndexDocumentFailed(response.DebugInformation);
    }

    public async Task UpdateLocatie<T>(string id, ILocatie locatie, long sequence) where T : class
    {
        var response = await _elasticClient.UpdateAsync<T>(
            id,
            selector: u => u
                .RetryOnConflict(3)
               .Script(
                s => s
                    .Source("if (ctx._source.sequence == null || params.seq > ctx._source.sequence) {" +
                            "for(l in ctx._source.locaties){" +
                            "   if(l.locatieId == params.locatieId){" +
                            "      for(p in params.locatie.entrySet()){" +
                            "         if(p.getValue() != null){" +
                            "            l[p.getKey()] = p.getValue();" +
                            "         }" +
                            "      }" +
                            "   }" +
                            "}" +
                            "ctx._source.sequence = params.seq;" +
                            "}")
                    .Params(objects => objects
                                      .Add(key: "locatieId", locatie.LocatieId)
                                      .Add(key: "locatie", locatie)
                                      .Add(key: "seq", sequence)
                                       )));

        if (!response.IsValid)
            // todo: log ? (should never happen in test/staging/production)
            throw new IndexDocumentFailed(response.DebugInformation);
    }

    public async Task UpdateAdres<T>(string id, int locatieId, string adresVoorstelling, string postcode, string gemeente, long sequence) where T : class
    {
        var response = await _elasticClient.UpdateAsync<T>(
            id,
            selector: u => u
                 .RetryOnConflict(3)
               .Script(
                s => s
                    .Source(
                         "if (ctx._source.sequence == null || params.seq > ctx._source.sequence) {" +
                         "for (l in ctx._source.locaties) {" +
                         "    if (l.locatieId == params.locatieId) {" +
                         "        if (params.adresvoorstelling != null) l.adresvoorstelling = params.adresvoorstelling;" +
                         "        if (params.postcode != null) l.postcode = params.postcode;" +
                         "        if (params.gemeente != null) l.gemeente = params.gemeente;" +
                         "    }" +
                         "}" +
                         "ctx._source.sequence = params.seq;" +
                         "}")
                    .Params(objects => objects
                                      .Add("locatieId", locatieId)
                                      .Add("adresvoorstelling", adresVoorstelling)
                                      .Add("postcode", postcode)
                                      .Add("gemeente", gemeente)
                                      .Add("seq", sequence)
                     )));

        if (!response.IsValid)
            // todo: log ? (should never happen in test/staging/production)
            throw new IndexDocumentFailed(response.DebugInformation);
    }

    public async Task RemoveLocatie<T>(string id, int locatieId, long sequence) where T : class
    {
        var response = await _elasticClient.UpdateAsync<T>(
            id,
            selector: u => u
                .RetryOnConflict(3)
               .Script(
                s => s
                   .Source("if (ctx._source.sequence == null || params.seq > ctx._source.sequence) {" +
                           "ctx._source.locaties.removeIf(l -> l.locatieId == params.locatieId);" +
                           "ctx._source.sequence = params.seq;" +
                           "}")
    .Params(objects => objects
               .Add(key: "locatieId", locatieId)
               .Add(key: "seq", sequence)
            )));

        if (!response.IsValid)
            // todo: log ? (should never happen in test/staging/production)
            throw new IndexDocumentFailed(response.DebugInformation);
    }

    public async Task AppendLidmaatschap<T>(string id, ILidmaatschap lidmaatschap, long sequence) where T : class
    {
        var response = await _elasticClient.UpdateAsync<T>(
            id,
            selector: u => u
               .RetryOnConflict(3)
               .Script(
                s => s
                    .Source("if (ctx._source.sequence == null || params.seq > ctx._source.sequence) {" +
                            "ctx._source.lidmaatschappen.add(params.item);" +
                            "ctx._source.sequence = params.seq;" +
                            "}")
                    .Params(objects => objects
                               .Add(key: "item", lidmaatschap)
                               .Add(key: "seq", sequence)
                            )));

        if (!response.IsValid)
            // todo: log ? (should never happen in test/staging/production)
            throw new IndexDocumentFailed(response.DebugInformation);
    }

    public async Task UpdateLidmaatschap<T>(string id, ILidmaatschap lidmaatschap, long sequence) where T : class
    {
        var response = await _elasticClient.UpdateAsync<T>(
            id,
            selector: u => u
                .RetryOnConflict(3)
               .Script(
                s => s
                    .Source(
                         "if (ctx._source.sequence == null || params.seq > ctx._source.sequence) {" +
                         "for(l in ctx._source.lidmaatschappen){" +
                         "   if(l.lidmaatschapId == params.lidmaatschapId){" +
                         "      for(p in params.lidmaatschap.entrySet()){" +
                         "         if(p.getValue() != null){" +
                         "            l[p.getKey()] = p.getValue();" +
                         "         }" +
                         "      }" +
                         "   }" +
                         "}" +
                         "ctx._source.sequence = params.seq;" +
                         "}")
                    .Params(objects => objects
                                      .Add(key: "lidmaatschapId", lidmaatschap.LidmaatschapId)
                                      .Add(key: "lidmaatschap", lidmaatschap)
                                      .Add(key: "seq", sequence)
                                       )));

        if (!response.IsValid)
            // todo: log ? (should never happen in test/staging/production)
            throw new IndexDocumentFailed(response.DebugInformation);
    }

    public async Task RemoveLidmaatschap<T>(string id, int lidmaatschapId, long sequence) where T : class
    {
        var response = await _elasticClient.UpdateAsync<T>(
            id,
            selector: u => u
                .RetryOnConflict(3)
               .Script(
                s => s
                    .Source("if (ctx._source.sequence == null || params.seq > ctx._source.sequence) {" +
                            "ctx._source.lidmaatschappen.removeIf(l -> l.lidmaatschapId == params.lidmaatschapId);" +
                            "ctx._source.sequence = params.seq;" +
                            "}")
                    .Params(objects => objects
                               .Add(key: "lidmaatschapId", lidmaatschapId)
                               .Add(key: "seq", sequence)
                            )));

        if (!response.IsValid)
            // todo: log ? (should never happen in test/staging/production)
            throw new IndexDocumentFailed(response.DebugInformation);
    }

    public async Task AppendCorresponderendeVCodes<T>(string id, string vCodeDubbeleVereniging, long sequence) where T : class
    {
        var response = await _elasticClient.UpdateAsync<T>(
            id,
            selector: u => u
               .RetryOnConflict(3)
               .Script(
                s => s
                    .Source("if (ctx._source.sequence == null || params.seq > ctx._source.sequence) {" +
                            "if(! ctx._source.corresponderendeVCodes.contains(params.item)){" +
                            "ctx._source.corresponderendeVCodes.add(params.item)" +
                            "}" +
                            "ctx._source.sequence = params.seq;" +
                            "}")
                    .Params(objects => objects
                               .Add(key: "item", vCodeDubbeleVereniging)
                               .Add(key: "seq", sequence)
                            )));

        if (!response.IsValid)
            // todo: log ? (should never happen in test/staging/production)
            throw new IndexDocumentFailed(response.DebugInformation);
    }

    public async Task RemoveCorresponderendeVCode<T>(string id, string vCodeDubbeleVereniging, long sequence) where T : class
    {
        var response = await _elasticClient.UpdateAsync<T>(
            id,
            selector: u => u
               .RetryOnConflict(3)
               .Script(
                s => s
                    .Source("if (ctx._source.sequence == null || params.seq > ctx._source.sequence) {" +
                            "ctx._source.corresponderendeVCodes.removeIf(c -> c == params.item);" +
                            "ctx._source.sequence = params.seq;" +
                            "}")
                    .Params(objects => objects
                               .Add(key: "item", vCodeDubbeleVereniging)
                               .Add(key: "seq", sequence)
                            )));

        if (!response.IsValid)
            // todo: log ? (should never happen in test/staging/production)
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
            u => u
                .RetryOnConflict(3)
               .Script(s => s
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
            u => u.RetryOnConflict(3)
                  .Script(s => s
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
}
