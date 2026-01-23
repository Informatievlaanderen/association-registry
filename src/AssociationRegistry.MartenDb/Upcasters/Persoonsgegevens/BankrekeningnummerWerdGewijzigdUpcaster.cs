namespace AssociationRegistry.MartenDb.Upcasters.Persoonsgegevens;

using Admin.Schema.Persoonsgegevens;
using Events;
using Marten;

public class BankrekeningnummerWerdGewijzigdUpcaster
{
    private readonly Func<IQuerySession> _querySessionFunc;

    public BankrekeningnummerWerdGewijzigdUpcaster(Func<IQuerySession> querySessionFunc)
    {
        _querySessionFunc = querySessionFunc;
    }

    public async Task<BankrekeningnummerWerdGewijzigd> UpcastAsync(
        BankrekeningnummerWerdGewijzigdZonderPersoonsgegevens bankrekeningnummerWerdToegevoegdZonderPersoonsgegevens,
        CancellationToken ct)
    {
        var session = _querySessionFunc();

        var persoonsgegevens = await session.Query<BankrekeningnummerPersoonsgegevensDocument>()
                                                             .Where(x => x.RefId == bankrekeningnummerWerdToegevoegdZonderPersoonsgegevens.RefId)
                                                             .SingleOrDefaultAsync(ct);

        return new BankrekeningnummerWerdGewijzigd(
            BankrekeningnummerId: bankrekeningnummerWerdToegevoegdZonderPersoonsgegevens.BankrekeningnummerId,
            bankrekeningnummerWerdToegevoegdZonderPersoonsgegevens.Doel,
            persoonsgegevens.Titularis
        );
    }
}
