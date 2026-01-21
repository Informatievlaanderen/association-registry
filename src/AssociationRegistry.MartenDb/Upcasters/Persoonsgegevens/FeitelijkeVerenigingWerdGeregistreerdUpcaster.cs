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

                                                                                   var vertegenwoordiger = doc is null
                                                                                       ? null // RegistratieData.VertegenwoordigerZonderPersoonsgegevens();
                                                                                       : new Registratiedata.Vertegenwoordiger(
                                                                                           doc.VertegenwoordigerId,
                                                                                           doc.Insz,
                                                                                           v.IsPrimair,
                                                                                           doc.Roepnaam,
                                                                                           doc.Rol,
                                                                                           doc.Voornaam,
                                                                                           doc.Achternaam,
                                                                                           doc.Email,
                                                                                           doc.Telefoon,
                                                                                           doc.Mobiel,
                                                                                           doc.SocialMedia);

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
