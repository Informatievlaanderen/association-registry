﻿namespace AssociationRegistry.Admin.Api.GrarSync;

using Marten;
using Schema.Detail;

public class LocatieFinder : ILocatieFinder
{
    private readonly IDocumentStore _documentStore;

    public LocatieFinder(IDocumentStore documentStore)
    {
        _documentStore = documentStore;
    }

    public async Task<IEnumerable<LocatieLookupDocument>> FindLocaties(string[] adresIds)
    {
        await using var session = _documentStore.LightweightSession();

        return session.Query<LocatieLookupDocument>().Where(x => adresIds.Contains(x.AdresId));
    }
}
