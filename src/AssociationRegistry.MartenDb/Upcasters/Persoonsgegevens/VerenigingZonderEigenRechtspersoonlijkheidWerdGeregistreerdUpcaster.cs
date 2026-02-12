namespace AssociationRegistry.MartenDb.Upcasters.Persoonsgegevens;

using Admin.Schema.Persoonsgegevens;
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
        CancellationToken ct
    )
    {
        var session = _querySessionFunc();

        Registratiedata.Vertegenwoordiger[] vertegenwoordigers = await GetVertegenwoordigersPersoonsgegevens(
            verenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdZonderPersoonsgegevens,
            ct,
            session
        );
        Registratiedata.Bankrekeningnummer[] bankrekeningnummers = await GetBankrekeningnummerPersoonsgegevens(
            verenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdZonderPersoonsgegevens,
            ct,
            session
        );

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
            Bankrekeningnummers: bankrekeningnummers,
            DuplicatieInfo: verenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdZonderPersoonsgegevens.DuplicatieInfo
        );
    }

    private async Task<Registratiedata.Bankrekeningnummer?[]> GetBankrekeningnummerPersoonsgegevens(
        VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdZonderPersoonsgegevens verenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdZonderPersoonsgegevens,
        CancellationToken ct,
        IQuerySession session
    )
    {
        if (
            verenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdZonderPersoonsgegevens
                .Bankrekeningnummers
                ?.Length
            is null
                or 0
        )
            return [];

        var bankrekeningRefIds = verenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdZonderPersoonsgegevens
            .Bankrekeningnummers.Select(x => x.RefId)
            .ToList();

        var bankrekeningDocs = await session
            .Query<BankrekeningnummerPersoonsgegevensDocument>()
            .Where(x => bankrekeningRefIds.Contains(x.RefId))
            .ToListAsync(ct);

        var bankrekeningByRefId = bankrekeningDocs.ToDictionary(x => x.RefId);

        var bankrekeningnummers = verenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdZonderPersoonsgegevens
            .Bankrekeningnummers.Select(b =>
            {
                bankrekeningByRefId.TryGetValue(b.RefId, out var doc);

                return doc is null
                    ? null
                    : new Registratiedata.Bankrekeningnummer(doc.BankrekeningnummerId, doc.Iban, b.Doel, doc.Titularis);
            })
            .ToArray();

        return bankrekeningnummers;
    }

    private static async Task<Registratiedata.Vertegenwoordiger?[]> GetVertegenwoordigersPersoonsgegevens(
        VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdZonderPersoonsgegevens verenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdZonderPersoonsgegevens,
        CancellationToken ct,
        IQuerySession session
    )
    {
        if (
            verenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdZonderPersoonsgegevens.Vertegenwoordigers?.Length
            is null
                or 0
        )
            return [];

        var refIdsFromFeitelijkeVereniging =
            verenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdZonderPersoonsgegevens
                .Vertegenwoordigers.Select(x => x.RefId)
                .ToList();

        var vertegenwoordigerPersoonsgegevens = await session
            .Query<VertegenwoordigerPersoonsgegevensDocument>()
            .Where(x => refIdsFromFeitelijkeVereniging.Contains(x.RefId))
            .ToListAsync(ct);

        var persoonsgegevensByRefId = vertegenwoordigerPersoonsgegevens.ToDictionary(x => x.RefId);

        var vertegenwoordigers = verenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdZonderPersoonsgegevens
            .Vertegenwoordigers.Select(v =>
            {
                persoonsgegevensByRefId.TryGetValue(v.RefId, out var doc);

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
                        doc.SocialMedia
                    );

                return vertegenwoordiger;
            })
            .ToArray();

        return vertegenwoordigers;
    }
}
