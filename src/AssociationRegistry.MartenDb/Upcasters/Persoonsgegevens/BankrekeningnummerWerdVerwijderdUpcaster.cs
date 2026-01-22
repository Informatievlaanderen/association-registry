namespace AssociationRegistry.MartenDb.Upcasters.Persoonsgegevens;

using Admin.Schema.Persoonsgegevens;
using AssociationRegistry.Events;
using AssociationRegistry.Persoonsgegevens;
using Marten;

public class BankrekeningnummerWerdVerwijderdUpcaster
{
    private readonly Func<IQuerySession> _querySessionFunc;

    public BankrekeningnummerWerdVerwijderdUpcaster(Func<IQuerySession> querySessionFunc)
    {
        _querySessionFunc = querySessionFunc;
    }

    public async Task<BankrekeningnummerWerdVerwijderd> UpcastAsync(
        BankrekeningnummerWerdVerwijderdZonderPersoonsgegevens bankrekeningnummerWerdVerwijderdZonderPersoonsgegevens,
        CancellationToken ct)
    {
        var session = _querySessionFunc();

        var persoonsgegevens = await session.Query<BankrekeningnummerPersoonsgegevensDocument>()
                                                             .Where(x => x.RefId == bankrekeningnummerWerdVerwijderdZonderPersoonsgegevens.RefId)
                                                             .SingleOrDefaultAsync(ct);

        return new BankrekeningnummerWerdVerwijderd(
            BankrekeningnummerId: bankrekeningnummerWerdVerwijderdZonderPersoonsgegevens.BankrekeningnummerId,
            persoonsgegevens.Iban
        );
    }
}
