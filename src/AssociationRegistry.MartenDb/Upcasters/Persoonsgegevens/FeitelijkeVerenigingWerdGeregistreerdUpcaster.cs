namespace AssociationRegistry.MartenDb.Upcasters.Persoonsgegevens;

using Admin.Schema.Persoonsgegevens;
using AssociationRegistry.Events;
using AssociationRegistry.Persoonsgegevens;
using Marten;

public class FeitelijkeVerenigingWerdGeregistreerdUpcaster
{
    private readonly Func<IQuerySession> _querySessionFunc;

    public FeitelijkeVerenigingWerdGeregistreerdUpcaster(Func<IQuerySession> querySessionFunc)
    {
        _querySessionFunc = querySessionFunc;
    }

    public async Task<FeitelijkeVerenigingWerdGeregistreerd> UpcastAsync(
        FeitelijkeVerenigingWerdGeregistreerdZonderPersoonsgegevens feitelijkeVerenigingWerdGeregistreerdZonderPersoonsgegevens,
        CancellationToken ct)
    {
        var session = _querySessionFunc();

        var refIdsFromFeitelijkeVereniging = feitelijkeVerenigingWerdGeregistreerdZonderPersoonsgegevens.Vertegenwoordigers
                                                                                  .Select(x => x.RefId)
                                                                                  .ToList();

        var vertegenwoordigerPersoonsgegevens = await session
                                                     .Query<VertegenwoordigerPersoonsgegevensDocument>()
                                                     .Where(x => refIdsFromFeitelijkeVereniging.Contains(x.RefId))
                                                     .ToListAsync(ct);

        var persoonsgegevensByRefId = vertegenwoordigerPersoonsgegevens
           .ToDictionary(x => x.RefId);

        var vertegenwoordigers = feitelijkeVerenigingWerdGeregistreerdZonderPersoonsgegevens.Vertegenwoordigers
                                                                              .Select(v =>
                                                                               {
                                                                                   persoonsgegevensByRefId.TryGetValue(
                                                                                       v.RefId, out var doc);

                                                                                   var vertegenwoordiger = new Registratiedata.Vertegenwoordiger(
                                                                                           v.VertegenwoordigerId,
                                                                                           doc?.Insz ?? WellKnownAnonymousFields.Geanonimiseerd,
                                                                                           v.IsPrimair,
                                                                                           doc?.Roepnaam ?? WellKnownAnonymousFields.Geanonimiseerd,
                                                                                           doc?.Rol ?? WellKnownAnonymousFields.Geanonimiseerd,
                                                                                           doc?.Voornaam ?? WellKnownAnonymousFields.Geanonimiseerd,
                                                                                           doc?.Achternaam ?? WellKnownAnonymousFields.Geanonimiseerd,
                                                                                           doc?.Email ?? WellKnownAnonymousFields.Geanonimiseerd,
                                                                                           doc?.Telefoon ?? WellKnownAnonymousFields.Geanonimiseerd,
                                                                                           doc?.Mobiel ?? WellKnownAnonymousFields.Geanonimiseerd,
                                                                                           doc?.SocialMedia ?? WellKnownAnonymousFields.Geanonimiseerd);

                                                                                   return vertegenwoordiger;
                                                                               })
                                                                              .ToArray();

        return new FeitelijkeVerenigingWerdGeregistreerd(
            VCode: feitelijkeVerenigingWerdGeregistreerdZonderPersoonsgegevens.VCode,
            Naam: feitelijkeVerenigingWerdGeregistreerdZonderPersoonsgegevens.Naam,
            KorteNaam: feitelijkeVerenigingWerdGeregistreerdZonderPersoonsgegevens.KorteNaam,
            KorteBeschrijving: feitelijkeVerenigingWerdGeregistreerdZonderPersoonsgegevens.KorteBeschrijving,
            Startdatum: feitelijkeVerenigingWerdGeregistreerdZonderPersoonsgegevens.Startdatum,
            Doelgroep: feitelijkeVerenigingWerdGeregistreerdZonderPersoonsgegevens.Doelgroep,
            IsUitgeschrevenUitPubliekeDatastroom: feitelijkeVerenigingWerdGeregistreerdZonderPersoonsgegevens.IsUitgeschrevenUitPubliekeDatastroom,
            Contactgegevens: feitelijkeVerenigingWerdGeregistreerdZonderPersoonsgegevens.Contactgegevens,
            Locaties: feitelijkeVerenigingWerdGeregistreerdZonderPersoonsgegevens.Locaties,
            Vertegenwoordigers: vertegenwoordigers,
            HoofdactiviteitenVerenigingsloket: feitelijkeVerenigingWerdGeregistreerdZonderPersoonsgegevens.HoofdactiviteitenVerenigingsloket
        );
    }
}
