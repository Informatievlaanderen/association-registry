namespace AssociationRegistry.MartenDb.Upcasters.Persoonsgegevens;

using Admin.Schema.Persoonsgegevens;
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
