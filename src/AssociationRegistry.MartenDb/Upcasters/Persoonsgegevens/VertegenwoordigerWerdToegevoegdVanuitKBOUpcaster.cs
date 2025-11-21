namespace AssociationRegistry.MartenDb.Upcasters.Persoonsgegevens;

using AssociationRegistry.Events;
using AssociationRegistry.Persoonsgegevens;
using Marten;

public class VertegenwoordigerWerdToegevoegdVanuitKBOUpcaster
{
    private readonly Func<IQuerySession> _querySessionFunc;

    public VertegenwoordigerWerdToegevoegdVanuitKBOUpcaster(Func<IQuerySession> querySessionFunc)
    {
        _querySessionFunc = querySessionFunc;
    }

    public async Task<VertegenwoordigerWerdToegevoegdVanuitKBO> UpcastAsync(
        VertegenwoordigerWerdToegevoegdVanuitKBOZonderPersoonsgegevens vertegenwoordigerWerdToegevoegdVanuitKboZonderPersoonsgegevens,
        CancellationToken ct)
    {
        await using var session = _querySessionFunc();

        var refId = vertegenwoordigerWerdToegevoegdVanuitKboZonderPersoonsgegevens.RefId;
        var vertegenwoordigerPersoonsgegevens = await session.Query<VertegenwoordigerPersoonsgegevensDocument>()
                                                             .Where(x => x.RefId == refId)
                                                             .SingleOrDefaultAsync(ct);

        return new VertegenwoordigerWerdToegevoegdVanuitKBO(
            VertegenwoordigerId: vertegenwoordigerWerdToegevoegdVanuitKboZonderPersoonsgegevens.VertegenwoordigerId,
            vertegenwoordigerPersoonsgegevens.Insz,
            vertegenwoordigerPersoonsgegevens.Voornaam,
            vertegenwoordigerPersoonsgegevens.Achternaam
        );
    }
}
