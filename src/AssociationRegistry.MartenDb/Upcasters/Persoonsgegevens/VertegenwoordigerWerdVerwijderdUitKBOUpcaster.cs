namespace AssociationRegistry.MartenDb.Upcasters.Persoonsgegevens;

using AssociationRegistry.Events;
using AssociationRegistry.Persoonsgegevens;
using Marten;

public class VertegenwoordigerWerdVerwijderdUitKBOUpcaster
{
    private readonly Func<IQuerySession> _querySessionFunc;

    public VertegenwoordigerWerdVerwijderdUitKBOUpcaster(Func<IQuerySession> querySessionFunc)
    {
        _querySessionFunc = querySessionFunc;
    }

    public async Task<VertegenwoordigerWerdVerwijderdUitKBO> UpcastAsync(
        VertegenwoordigerWerdVerwijderdUitKBOZonderPersoonsgegevens vertegenwoordigerWerdVerwijderdUitKboZonderPersoonsgegevens,
        CancellationToken ct)
    {
        var session = _querySessionFunc();

        var refId = vertegenwoordigerWerdVerwijderdUitKboZonderPersoonsgegevens.RefId;
        var vertegenwoordigerPersoonsgegevens = await session.Query<VertegenwoordigerPersoonsgegevensDocument>()
                                                             .Where(x => x.RefId == refId)
                                                             .SingleOrDefaultAsync(ct);

        return new VertegenwoordigerWerdVerwijderdUitKBO(
            VertegenwoordigerId: vertegenwoordigerWerdVerwijderdUitKboZonderPersoonsgegevens.VertegenwoordigerId,
            vertegenwoordigerPersoonsgegevens.Insz,
            vertegenwoordigerPersoonsgegevens.Voornaam,
            vertegenwoordigerPersoonsgegevens.Achternaam
        );
    }
}
