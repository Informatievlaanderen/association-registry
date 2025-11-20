namespace AssociationRegistry.MartenDb.Transformers;

using AssociationRegistry.Events;
using AssociationRegistry.Persoonsgegevens;
using Marten;

public static class EventUpcaster
{
    public static StoreOptions UpcastEvents(this StoreOptions opts, Func<IQuerySession> querySessionFunc)
    {
        opts.Events.Upcast<VertegenwoordigerWerdToegevoegdZonderPersoonsgegevens, VertegenwoordigerWerdToegevoegd>(
            async (vertegenwoordigerWerdToegevoegd, ct) =>
            {
                await using var session = querySessionFunc();

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
            });
        //
        // opts.Events.Upcast<VertegenwoordigerWerdToegevoegdVanuitKBOZonderPersoonsgegevens, VertegenwoordigerWerdToegevoegdVanuitKBO>(
        //     async (vertegenwoordigerWerdToegevoegdVanuitKboZonderPersoonsgegevens, ct) =>
        //     {
        //         await using var session = querySessionFunc();
        //
        //         var vertegenwoordigerPersoonsgegevens = await session.Query<VertegenwoordigerPersoonsgegevensDocument>()
        //                                                              .Where(x => x.RefId == vertegenwoordigerWerdToegevoegdVanuitKboZonderPersoonsgegevens.RefId)
        //                                                              .SingleOrDefaultAsync(ct);
        //
        //         return new VertegenwoordigerWerdToegevoegdVanuitKBO(
        //             VertegenwoordigerId: vertegenwoordigerWerdToegevoegdVanuitKboZonderPersoonsgegevens.VertegenwoordigerId,
        //             vertegenwoordigerPersoonsgegevens.Insz,
        //             vertegenwoordigerPersoonsgegevens.Voornaam,
        //             vertegenwoordigerPersoonsgegevens.Achternaam
        //         );
        //     });
        //
        // opts.Events.Upcast<VertegenwoordigerWerdGewijzigd, VertegenwoordigerWerdGewijzigdMetPersoonsgegevens>(
        //     async (vertegenwoordigerWerdToegevoegd, ct) =>
        //     {
        //         await using var session = querySessionFunc();
        //
        //         var vertegenwoordigerPersoonsgegevens = await session.Query<VertegenwoordigerPersoonsgegevensDocument>()
        //                                                              .Where(x => x.RefId == vertegenwoordigerWerdToegevoegd.RefId)
        //                                                              .SingleOrDefaultAsync(ct);
        //
        //         return new VertegenwoordigerWerdGewijzigdMetPersoonsgegevens(
        //             VertegenwoordigerId: vertegenwoordigerWerdToegevoegd.VertegenwoordigerId,
        //             IsPrimair: vertegenwoordigerWerdToegevoegd.IsPrimair,
        //             vertegenwoordigerPersoonsgegevens is not null
        //                 ? new VertegenwoordigerPersoonsgegevens(
        //                     vertegenwoordigerPersoonsgegevens.Insz,
        //                     vertegenwoordigerPersoonsgegevens.Roepnaam,
        //                     vertegenwoordigerPersoonsgegevens.Rol,
        //                     vertegenwoordigerPersoonsgegevens.Voornaam,
        //                     vertegenwoordigerPersoonsgegevens.Achternaam,
        //                     vertegenwoordigerPersoonsgegevens.Email,
        //                     vertegenwoordigerPersoonsgegevens.Telefoon,
        //                     vertegenwoordigerPersoonsgegevens.Mobiel,
        //                     vertegenwoordigerPersoonsgegevens.SocialMedia
        //                 )
        //                 : null);
        //     });
        //
        // opts.Events.Upcast<VertegenwoordigerWerdVerwijderd, VertegenwoordigerWerdVerwijderdMetPersoonsgegevens>(
        //     async (vertegenwoordigerWerdToegevoegd, ct) =>
        //     {
        //         await using var session = querySessionFunc();
        //
        //         var vertegenwoordigerPersoonsgegevens = await session.Query<VertegenwoordigerPersoonsgegevensDocument>()
        //                                                              .Where(x => x.RefId == vertegenwoordigerWerdToegevoegd.RefId)
        //                                                              .SingleOrDefaultAsync(ct);
        //
        //         return new VertegenwoordigerWerdVerwijderdMetPersoonsgegevens(
        //             VertegenwoordigerId: vertegenwoordigerWerdToegevoegd.VertegenwoordigerId,
        //             vertegenwoordigerPersoonsgegevens?.Insz,
        //             vertegenwoordigerPersoonsgegevens?.Voornaam,
        //             vertegenwoordigerPersoonsgegevens?.Achternaam);
        //     });
        //

        opts.Events.Upcast<FeitelijkeVerenigingWerdGeregistreerdZonderPersoonsgegevens, FeitelijkeVerenigingWerdGeregistreerd>(
            async (feitelijkeVerenigingWerdGeregistreerdZonderPersoonsgegevens, ct) =>
            {
                await using var session = querySessionFunc();

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
            });

        opts.Events.Upcast<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdZonderPersoonsgegevens, VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>(
            async (verenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdZonderPersoonsgegevens, ct) =>
            {
                await using var session = querySessionFunc();

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
            });

        return opts;
    }
}
