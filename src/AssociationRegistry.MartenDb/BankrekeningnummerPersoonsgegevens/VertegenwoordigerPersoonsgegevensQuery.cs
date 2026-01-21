namespace AssociationRegistry.MartenDb.BankrekeningnummerPersoonsgegevens;

using Admin.Schema.Persoonsgegevens;
using Marten;

public class BankrekeningnummerPersoonsgegevensQuery : IBankrekeningnummerPersoonsgegevensQuery
{
    private readonly IDocumentSession _session;

    public BankrekeningnummerPersoonsgegevensQuery(IDocumentSession session)
    {
        _session = session;
    }

    public async Task<BankrekeningnummerPersoonsgegevensDocument?> ExecuteAsync(
        BankrekeningnummerPersoonsgegevensByRefIdFilter byRefIdFilter,
        CancellationToken cancellationToken)
        => _session.PendingChanges.Inserts().OfType<BankrekeningnummerPersoonsgegevensDocument>()
                   .SingleOrDefault(x => x.RefId == byRefIdFilter.RefId) ??
           await _session.LoadAsync<BankrekeningnummerPersoonsgegevensDocument>(byRefIdFilter.RefId, cancellationToken);

    public async Task<BankrekeningnummerPersoonsgegevensDocument[]> ExecuteAsync(BankrekeningnummerPersoonsgegevensByRefIdsFilter filter, CancellationToken cancellationToken)
    {
        var fromDb = await _session.Query<BankrekeningnummerPersoonsgegevensDocument>()
                                   .Where(x => filter.RefIds.Contains(x.RefId))
                                   .ToListAsync(cancellationToken);

        var pending = _session.PendingChanges.Inserts()
                              .OfType<BankrekeningnummerPersoonsgegevensDocument>()
                              .Where(x => filter.RefIds.Contains(x.RefId));

        return fromDb
              .Concat(pending)
              .ToArray();
    }

    public async Task<BankrekeningnummerPersoonsgegevensDocument[]> ExecuteAsync(BankrekeningnummerPersoonsgegevensByIbanFilter filter, CancellationToken cancellationToken)
    {
        var fromDb = await _session.Query<BankrekeningnummerPersoonsgegevensDocument>()
                                   .Where(x => filter.Iban.Value == x.Iban)
                                   .ToListAsync(cancellationToken);

        var pending = _session.PendingChanges.Inserts()
                              .OfType<BankrekeningnummerPersoonsgegevensDocument>()
                              .Where(x => filter.Iban.Value == x.Iban);

        return fromDb
              .Concat(pending)
              .ToArray();
    }
}
