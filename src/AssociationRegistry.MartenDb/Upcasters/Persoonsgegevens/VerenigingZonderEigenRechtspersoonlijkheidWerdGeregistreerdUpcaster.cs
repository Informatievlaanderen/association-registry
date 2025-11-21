namespace AssociationRegistry.MartenDb.Upcasters.Persoonsgegevens;

using AssociationRegistry.Events;
using AssociationRegistry.Persoonsgegevens;
using Marten;

public class VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdUpcaster
{
    private readonly Func<IQuerySession> _querySessionFunc;

    public VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdUpcaster(Func<IQuerySession> querySessionFunc)
    {
        _querySessionFunc = querySessionFunc;
    }

    public async Task<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd> UpcastAsync(
        VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdZonderPersoonsgegevens verenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdZonderPersoonsgegevens,
        CancellationToken ct)
    {
        var session = _querySessionFunc();

        var refIdsFromFeitelijkeVereniging = verenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdZonderPersoonsgegevens.Vertegenwoordigers
                                                                                  .Select(x => x.RefId)
                                                                                  .ToList();

        var vertegenwoordigerPersoonsgegevens = await session
                                                     .Query<VertegenwoordigerPersoonsgegevensDocument>()
                                                     .Where(x => refIdsFromFeitelijkeVereniging.Contains(x.RefId))
                                                     .ToListAsync(ct);

        var persoonsgegevensByRefId = vertegenwoordigerPersoonsgegevens
           .ToDictionary(x => x.RefId);

        var vertegenwoordigers = verenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdZonderPersoonsgegevens.Vertegenwoordigers
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

        return new VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd(
            VCode: verenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdZonderPersoonsgegevens.VCode,
            Naam: verenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdZonderPersoonsgegevens.Naam,
            KorteNaam: verenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdZonderPersoonsgegevens.KorteNaam,
            KorteBeschrijving: verenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdZonderPersoonsgegevens.KorteBeschrijving,
            Startdatum: verenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdZonderPersoonsgegevens.Startdatum,
            Doelgroep: verenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdZonderPersoonsgegevens.Doelgroep,
            IsUitgeschrevenUitPubliekeDatastroom: verenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdZonderPersoonsgegevens.IsUitgeschrevenUitPubliekeDatastroom,
            Contactgegevens: verenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdZonderPersoonsgegevens.Contactgegevens,
            Locaties: verenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdZonderPersoonsgegevens.Locaties,
            Vertegenwoordigers: vertegenwoordigers,
            HoofdactiviteitenVerenigingsloket: verenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdZonderPersoonsgegevens.HoofdactiviteitenVerenigingsloket,
            DuplicatieInfo: verenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdZonderPersoonsgegevens.DuplicatieInfo
        );
    }
}
