namespace AssociationRegistry.MartenDb.Upcasters.Persoonsgegevens;

using Admin.Schema.Persoonsgegevens;
using AssociationRegistry.Events;
using AssociationRegistry.Persoonsgegevens;
using Marten;

public class BankrekeningnummerWerdToegevoegdUpcaster
{
    private readonly Func<IQuerySession> _querySessionFunc;

    public BankrekeningnummerWerdToegevoegdUpcaster(Func<IQuerySession> querySessionFunc)
    {
        _querySessionFunc = querySessionFunc;
    }

    public async Task<BankrekeningnummerWerdToegevoegd> UpcastAsync(
        BankrekeningnummerWerdToegevoegdZonderPersoonsgegevens bankrekeningnummerWerdToegevoegdZonderPersoonsgegevens,
        CancellationToken ct)
    {
        var session = _querySessionFunc();

        var persoonsgegevens = await session.Query<BankrekeningnummerPersoonsgegevensDocument>()
                                                             .Where(x => x.RefId == bankrekeningnummerWerdToegevoegdZonderPersoonsgegevens.RefId)
                                                             .SingleOrDefaultAsync(ct);

        return new BankrekeningnummerWerdToegevoegd(
            BankrekeningnummerId: bankrekeningnummerWerdToegevoegdZonderPersoonsgegevens.BankrekeningnummerId,
            persoonsgegevens.Iban,
            bankrekeningnummerWerdToegevoegdZonderPersoonsgegevens.Doel,
            persoonsgegevens.Titularis
        );
    }
}
