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
            vertegenwoordigerPersoonsgegevens?.Insz ?? WellKnownAnonymousFields.Geanonimiseerd,
            vertegenwoordigerWerdToegevoegd.IsPrimair,
            vertegenwoordigerPersoonsgegevens?.Roepnaam ?? WellKnownAnonymousFields.Geanonimiseerd,
            vertegenwoordigerPersoonsgegevens?.Rol ?? WellKnownAnonymousFields.Geanonimiseerd,
            vertegenwoordigerPersoonsgegevens?.Voornaam ?? WellKnownAnonymousFields.Geanonimiseerd,
            vertegenwoordigerPersoonsgegevens?.Achternaam ?? WellKnownAnonymousFields.Geanonimiseerd,
            vertegenwoordigerPersoonsgegevens?.Email ?? WellKnownAnonymousFields.Geanonimiseerd,
            vertegenwoordigerPersoonsgegevens?.Telefoon ?? WellKnownAnonymousFields.Geanonimiseerd,
            vertegenwoordigerPersoonsgegevens?.Mobiel ?? WellKnownAnonymousFields.Geanonimiseerd,
            vertegenwoordigerPersoonsgegevens?.SocialMedia ?? WellKnownAnonymousFields.Geanonimiseerd
        );

    }
}
