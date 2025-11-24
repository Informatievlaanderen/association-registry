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
        VertegenwoordigerWerdGewijzigdZonderPersoonsgegevens vertegenwoordigerWerdGewijzigdZonderPersoonsgegevens,
        CancellationToken ct)
    {
        var session = _querySessionFunc();

        var vertegenwoordigerPersoonsgegevens = await session.Query<VertegenwoordigerPersoonsgegevensDocument>()
                                                             .Where(x => x.RefId == vertegenwoordigerWerdGewijzigdZonderPersoonsgegevens.RefId)
                                                             .SingleOrDefaultAsync(ct);

        return new VertegenwoordigerWerdGewijzigd(
            VertegenwoordigerId: vertegenwoordigerWerdGewijzigdZonderPersoonsgegevens.VertegenwoordigerId,
            vertegenwoordigerWerdGewijzigdZonderPersoonsgegevens.IsPrimair,
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
