namespace AssociationRegistry.MartenDb.Upcasters.Persoonsgegevens;

using AssociationRegistry.Events;
using AssociationRegistry.Persoonsgegevens;
using Marten;

public class VertegenwoordigerWerdOvergenomenUitKBOUpcaster
{
    private readonly Func<IQuerySession> _querySessionFunc;

    public VertegenwoordigerWerdOvergenomenUitKBOUpcaster(Func<IQuerySession> querySessionFunc)
    {
        _querySessionFunc = querySessionFunc;
    }

    public async Task<VertegenwoordigerWerdOvergenomenUitKBO> UpcastAsync(
        VertegenwoordigerWerdOvergenomenUitKBOZonderPersoonsgegevens vertegenwoordigerWerdOvergenomenUitKboZonderPersoonsgegevens,
        CancellationToken ct)
    {
        await using var session = _querySessionFunc();

        var refId = vertegenwoordigerWerdOvergenomenUitKboZonderPersoonsgegevens.RefId;
        var vertegenwoordigerPersoonsgegevens = await session.Query<VertegenwoordigerPersoonsgegevensDocument>()
                                                             .Where(x => x.RefId == refId)
                                                             .SingleOrDefaultAsync(ct);

        return new VertegenwoordigerWerdOvergenomenUitKBO(
            VertegenwoordigerId: vertegenwoordigerWerdOvergenomenUitKboZonderPersoonsgegevens.VertegenwoordigerId,
            vertegenwoordigerPersoonsgegevens.Insz,
            vertegenwoordigerPersoonsgegevens.Voornaam,
            vertegenwoordigerPersoonsgegevens.Achternaam
        );
    }
}
