namespace AssociationRegistry.MartenDb.Upcasters.Persoonsgegevens;

using Admin.Schema.Persoonsgegevens;
using Be.Vlaanderen.Basisregisters.Utilities;
using Events;
using Marten;

public class BankrekeningnummerWerdGevalideerdUpcaster
{
    private readonly Func<IQuerySession> _querySessionFunc;

    public BankrekeningnummerWerdGevalideerdUpcaster(Func<IQuerySession> querySessionFunc)
    {
        _querySessionFunc = querySessionFunc;
    }

    public async Task<BankrekeningnummerWerdGevalideerd> UpcastAsync(
        BankrekeningnummerWerdGevalideerdZonderPersoonsgegevens bankrekeningnummerWerdGevalideerdZonderPersoonsgegevens,
        CancellationToken ct)
    {
        var session = _querySessionFunc();

        var persoonsgegevens = await session.Query<BankrekeningnummerPersoonsgegevensDocument>()
                                                             .Where(x => x.RefId == bankrekeningnummerWerdGevalideerdZonderPersoonsgegevens.RefId)
                                                             .SingleOrDefaultAsync(ct);

        return new BankrekeningnummerWerdGevalideerd(
            BankrekeningnummerId: bankrekeningnummerWerdGevalideerdZonderPersoonsgegevens.BankrekeningnummerId,
            persoonsgegevens.Iban,
            persoonsgegevens.Titularis,
            bankrekeningnummerWerdGevalideerdZonderPersoonsgegevens.GevalideerdDoor
        );
    }
}
