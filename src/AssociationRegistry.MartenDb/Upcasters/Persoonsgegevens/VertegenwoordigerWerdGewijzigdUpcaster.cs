namespace AssociationRegistry.MartenDb.Upcasters.Persoonsgegevens;

using AssociationRegistry.Events;
using AssociationRegistry.Persoonsgegevens;
using Marten;

public class VertegenwoordigerWerdGewijzigdUpcaster
{
    private readonly Func<IQuerySession> _querySessionFunc;

    public VertegenwoordigerWerdGewijzigdUpcaster(Func<IQuerySession> querySessionFunc)
    {
        _querySessionFunc = querySessionFunc;
    }

    public async Task<VertegenwoordigerWerdGewijzigd> UpcastAsync(
        VertegenwoordigerWerdGewijzigdZonderPersoongegevens vertegenwoordigerWerdGewijzigdZonderPersoongegevens,
        CancellationToken ct)
    {
        await using var session = _querySessionFunc();

        var vertegenwoordigerPersoonsgegevens = await session.Query<VertegenwoordigerPersoonsgegevensDocument>()
                                                             .Where(x => x.RefId == vertegenwoordigerWerdGewijzigdZonderPersoongegevens.RefId)
                                                             .SingleOrDefaultAsync(ct);

        return new VertegenwoordigerWerdGewijzigd(
            VertegenwoordigerId: vertegenwoordigerWerdGewijzigdZonderPersoongegevens.VertegenwoordigerId,
            vertegenwoordigerWerdGewijzigdZonderPersoongegevens.IsPrimair,
            vertegenwoordigerPersoonsgegevens.Roepnaam,
            vertegenwoordigerPersoonsgegevens.Rol,
            vertegenwoordigerPersoonsgegevens.Voornaam,
            vertegenwoordigerPersoonsgegevens.Achternaam,
            vertegenwoordigerPersoonsgegevens.Email,
            vertegenwoordigerPersoonsgegevens.Telefoon,
            vertegenwoordigerPersoonsgegevens.Mobiel,
            vertegenwoordigerPersoonsgegevens.SocialMedia
        );
    }
}