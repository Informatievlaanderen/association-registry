namespace AssociationRegistry.MartenDb.Upcasters.Persoonsgegevens;

using Admin.Schema.Persoonsgegevens;
using AssociationRegistry.Events;
using AssociationRegistry.Persoonsgegevens;
using Marten;

public class VertegenwoordigerWerdToegevoegdUpcaster
{
    private readonly Func<IQuerySession> _querySessionFunc;

    public VertegenwoordigerWerdToegevoegdUpcaster(Func<IQuerySession> querySessionFunc)
    {
        _querySessionFunc = querySessionFunc;
    }

    public async Task<VertegenwoordigerWerdToegevoegd> UpcastAsync(
        VertegenwoordigerWerdToegevoegdZonderPersoonsgegevens vertegenwoordigerWerdToegevoegd,
        CancellationToken ct)
    {
        var session = _querySessionFunc();

        var vertegenwoordigerPersoonsgegevens = await session.Query<VertegenwoordigerPersoonsgegevensDocument>()
                                                             .Where(x => x.RefId == vertegenwoordigerWerdToegevoegd.RefId)
                                                             .SingleOrDefaultAsync(ct);

        return new VertegenwoordigerWerdToegevoegd(
            VertegenwoordigerId: vertegenwoordigerWerdToegevoegd.VertegenwoordigerId,
            vertegenwoordigerPersoonsgegevens.Insz,
            vertegenwoordigerWerdToegevoegd.IsPrimair,
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
