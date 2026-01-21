namespace AssociationRegistry.MartenDb.VertegenwoordigerPersoonsgegevens;

using Admin.Schema.Persoonsgegevens;
using AssociationRegistry.Persoonsgegevens;
using Marten;

public class VertegenwoordigerPersoonsgegevensQuery : IVertegenwoordigerPersoonsgegevensQuery
{
    private readonly IDocumentSession _session;

    public VertegenwoordigerPersoonsgegevensQuery(IDocumentSession session)
    {
        _session = session;
    }

    public async Task<VertegenwoordigerPersoonsgegevensDocument?> ExecuteAsync(
        VertegenwoordigerPersoonsgegevensByRefIdFilter byRefIdFilter,
        CancellationToken cancellationToken)
        => _session.PendingChanges.Inserts().OfType<VertegenwoordigerPersoonsgegevensDocument>()
                   .SingleOrDefault(x => x.RefId == byRefIdFilter.RefId) ??
           await _session.LoadAsync<VertegenwoordigerPersoonsgegevensDocument>(byRefIdFilter.RefId, cancellationToken);

    public async Task<VertegenwoordigerPersoonsgegevensDocument[]> ExecuteAsync(VertegenwoordigerPersoonsgegevensByRefIdsFilter filter, CancellationToken cancellationToken)
    {
        var fromDb = await _session.Query<VertegenwoordigerPersoonsgegevensDocument>()
                                   .Where(x => filter.RefIds.Contains(x.RefId))
                                   .ToListAsync(cancellationToken);

        var pending = _session.PendingChanges.Inserts()
                              .OfType<VertegenwoordigerPersoonsgegevensDocument>()
                              .Where(x => filter.RefIds.Contains(x.RefId));

        return fromDb
              .Concat(pending)
              .ToArray();
    }

    public async Task<VertegenwoordigerPersoonsgegevensDocument[]> ExecuteAsync(VertegenwoordigerPersoonsgegevensByInszFilter filter, CancellationToken cancellationToken)
    {
        var fromDb = await _session.Query<VertegenwoordigerPersoonsgegevensDocument>()
                                   .Where(x => filter.Insz == x.Insz)
                                   .ToListAsync(cancellationToken);

        var pending = _session.PendingChanges.Inserts()
                              .OfType<VertegenwoordigerPersoonsgegevensDocument>()
                              .Where(x => filter.Insz == x.Insz);

        return fromDb
              .Concat(pending)
              .ToArray();
    }
}
