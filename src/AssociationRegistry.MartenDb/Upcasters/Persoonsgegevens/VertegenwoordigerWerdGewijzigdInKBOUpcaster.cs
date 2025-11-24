namespace AssociationRegistry.MartenDb.Upcasters.Persoonsgegevens;

using AssociationRegistry.Persoonsgegevens;
using Events;
using Marten;

public class VertegenwoordigerWerdGewijzigdInKBOUpcaster
{
    private readonly Func<IQuerySession> _querySessionFunc;

    public VertegenwoordigerWerdGewijzigdInKBOUpcaster(Func<IQuerySession> querySessionFunc)
    {
        _querySessionFunc = querySessionFunc;
    }

    public async Task<VertegenwoordigerWerdGewijzigdInKBO> UpcastAsync(
        VertegenwoordigerWerdGewijzigdInKBOZonderPersoonsgegevens vertegenwoordigerWerdGewijzigdInKboZonderPersoonsgegevens,
        CancellationToken ct)
    {
        await using var session = _querySessionFunc();

        var vertegenwoordigerPersoonsgegevens = await session.Query<VertegenwoordigerPersoonsgegevensDocument>()
                                                             .Where(x => x.RefId == vertegenwoordigerWerdGewijzigdInKboZonderPersoonsgegevens.RefId)
                                                             .SingleOrDefaultAsync(ct);

        return new VertegenwoordigerWerdGewijzigdInKBO(
            VertegenwoordigerId: vertegenwoordigerWerdGewijzigdInKboZonderPersoonsgegevens.VertegenwoordigerId,
            vertegenwoordigerPersoonsgegevens.Insz,
            vertegenwoordigerPersoonsgegevens.Voornaam,
            vertegenwoordigerPersoonsgegevens.Achternaam
        );
    }
}
