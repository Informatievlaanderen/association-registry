namespace AssociationRegistry.MartenDb.Upcasters.Persoonsgegevens;

using Admin.Schema.Persoonsgegevens;
using AssociationRegistry.Events;
using AssociationRegistry.Persoonsgegevens;
using Marten;

public class VertegenwoordigerWerdVerwijderdUpcaster
{
    private readonly Func<IQuerySession> _querySessionFunc;

    public VertegenwoordigerWerdVerwijderdUpcaster(Func<IQuerySession> querySessionFunc)
    {
        _querySessionFunc = querySessionFunc;
    }

    public async Task<VertegenwoordigerWerdVerwijderd> UpcastAsync(
        VertegenwoordigerWerdVerwijderdZonderPersoonsgegevens vertegenwoordigerWerdVerwijderd,
        CancellationToken ct)
    {
        var session = _querySessionFunc();

        var refId = vertegenwoordigerWerdVerwijderd.RefId;
        var vertegenwoordigerPersoonsgegevens = await session.Query<VertegenwoordigerPersoonsgegevensDocument>()
                                                             .Where(x => x.RefId == refId)
                                                             .SingleOrDefaultAsync(ct);

        return new VertegenwoordigerWerdVerwijderd(
            VertegenwoordigerId: vertegenwoordigerWerdVerwijderd.VertegenwoordigerId,
            vertegenwoordigerPersoonsgegevens.Insz,
            vertegenwoordigerPersoonsgegevens.Voornaam,
            vertegenwoordigerPersoonsgegevens.Achternaam
        );
    }
}
