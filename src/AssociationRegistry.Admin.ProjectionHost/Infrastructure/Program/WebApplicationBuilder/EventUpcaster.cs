namespace AssociationRegistry.Admin.ProjectionHost.Infrastructure.Program.WebApplicationBuilder;

using Events;
using Events.Enriched;
using Marten;
using Marten.Services.Json.Transformations;
using Marten.Services.Json.Transformations.JsonNet;
using Schema.Persoonsgegevens;

public static class EventUpcaster
{
    public static StoreOptions UpcastEvents(this StoreOptions opts, Func<IQuerySession> querySessionFunc)
    {
        opts.Events.Upcast<VertegenwoordigerWerdToegevoegd, VertegenwoordigerWerdToegevoegdMetPersoonsgegevens>(
            async (vertegenwoordigerWerdToegevoegd, ct) =>
            {
                await using var session = querySessionFunc();

                var vertegenwoordigerPersoonsgegevens = await session.Query<VertegenwoordigerPersoonsgegevensDocument>()
                                                                     .Where(x => x.RefId == vertegenwoordigerWerdToegevoegd.RefId)
                                                                     .SingleOrDefaultAsync(ct);

                return new VertegenwoordigerWerdToegevoegdMetPersoonsgegevens(
                    VertegenwoordigerId: vertegenwoordigerWerdToegevoegd.VertegenwoordigerId,
                    IsPrimair: vertegenwoordigerWerdToegevoegd.IsPrimair,
                    vertegenwoordigerPersoonsgegevens is not null
                        ? new VertegenwoordigerPersoonsgegevens(
                            vertegenwoordigerPersoonsgegevens.Insz,
                            vertegenwoordigerPersoonsgegevens.Roepnaam,
                            vertegenwoordigerPersoonsgegevens.Rol,
                            vertegenwoordigerPersoonsgegevens.Voornaam,
                            vertegenwoordigerPersoonsgegevens.Achternaam,
                            vertegenwoordigerPersoonsgegevens.Email,
                            vertegenwoordigerPersoonsgegevens.Telefoon,
                            vertegenwoordigerPersoonsgegevens.Mobiel,
                            vertegenwoordigerPersoonsgegevens.SocialMedia
                        )
                        : null);
            });

        opts.Events.Upcast<VertegenwoordigerWerdGewijzigd, VertegenwoordigerWerdGewijzigdMetPersoonsgegevens>(
            async (vertegenwoordigerWerdToegevoegd, ct) =>
            {
                await using var session = querySessionFunc();

                var vertegenwoordigerPersoonsgegevens = await session.Query<VertegenwoordigerPersoonsgegevensDocument>()
                                                                     .Where(x => x.RefId == vertegenwoordigerWerdToegevoegd.RefId)
                                                                     .SingleOrDefaultAsync(ct);

                return new VertegenwoordigerWerdGewijzigdMetPersoonsgegevens(
                    VertegenwoordigerId: vertegenwoordigerWerdToegevoegd.VertegenwoordigerId,
                    IsPrimair: vertegenwoordigerWerdToegevoegd.IsPrimair,
                    vertegenwoordigerPersoonsgegevens is not null
                        ? new VertegenwoordigerPersoonsgegevens(
                            vertegenwoordigerPersoonsgegevens.Insz,
                            vertegenwoordigerPersoonsgegevens.Roepnaam,
                            vertegenwoordigerPersoonsgegevens.Rol,
                            vertegenwoordigerPersoonsgegevens.Voornaam,
                            vertegenwoordigerPersoonsgegevens.Achternaam,
                            vertegenwoordigerPersoonsgegevens.Email,
                            vertegenwoordigerPersoonsgegevens.Telefoon,
                            vertegenwoordigerPersoonsgegevens.Mobiel,
                            vertegenwoordigerPersoonsgegevens.SocialMedia
                        )
                        : null);
            });

        opts.Events.Upcast<VertegenwoordigerWerdVerwijderd, VertegenwoordigerWerdVerwijderdMetPersoonsgegevens>(
            async (vertegenwoordigerWerdToegevoegd, ct) =>
            {
                await using var session = querySessionFunc();

                var vertegenwoordigerPersoonsgegevens = await session.Query<VertegenwoordigerPersoonsgegevensDocument>()
                                                                     .Where(x => x.RefId == vertegenwoordigerWerdToegevoegd.RefId)
                                                                     .SingleOrDefaultAsync(ct);

                return new VertegenwoordigerWerdVerwijderdMetPersoonsgegevens(
                    VertegenwoordigerId: vertegenwoordigerWerdToegevoegd.VertegenwoordigerId,
                    vertegenwoordigerPersoonsgegevens?.Insz,
                    vertegenwoordigerPersoonsgegevens?.Voornaam,
                    vertegenwoordigerPersoonsgegevens?.Achternaam);
            });

        opts.Events.Upcast<FeitelijkeVerenigingWerdGeregistreerd, FeitelijkeVerenigingWerdGeristreerdMetPersoonsgegevens>(
            async (feitelijkeVerenigingWerdGeregistreerd, ct) =>
            {
                await using var session = querySessionFunc();

                var refIdsFromFeitelijkeVereniging = feitelijkeVerenigingWerdGeregistreerd.Vertegenwoordigers
                                                                                          .Select(x => x.RefId)
                                                                                          .ToList();

                var vertegenwoordigerPersoonsgegevens = await session
                                                             .Query<VertegenwoordigerPersoonsgegevensDocument>()
                                                             .Where(x => refIdsFromFeitelijkeVereniging.Contains(x.RefId))
                                                             .ToListAsync(ct);

                var persoonsgegevensByRefId = vertegenwoordigerPersoonsgegevens
                   .ToDictionary(x => x.RefId);

                var enrichedVertegenwoordigers = feitelijkeVerenigingWerdGeregistreerd.Vertegenwoordigers
                                                                                      .Select(v =>
                                                                                       {
                                                                                           persoonsgegevensByRefId.TryGetValue(
                                                                                               v.RefId, out var doc);

                                                                                           var vertegenwoordigerPersoongegevens = doc is null
                                                                                               ? null
                                                                                               : new VertegenwoordigerPersoonsgegevens(
                                                                                                   doc.Insz,
                                                                                                   doc.Roepnaam,
                                                                                                   doc.Rol,
                                                                                                   doc.Voornaam,
                                                                                                   doc.Achternaam,
                                                                                                   doc.Email,
                                                                                                   doc.Telefoon,
                                                                                                   doc.Mobiel,
                                                                                                   doc.SocialMedia);

                                                                                           return new EnrichedVertegenwoordiger(
                                                                                               v.RefId,
                                                                                               v.VertegenwoordigerId,
                                                                                               v.IsPrimair,
                                                                                               vertegenwoordigerPersoongegevens);
                                                                                       })
                                                                                      .ToArray();

                return new FeitelijkeVerenigingWerdGeristreerdMetPersoonsgegevens(
                    VCode: feitelijkeVerenigingWerdGeregistreerd.VCode,
                    Naam: feitelijkeVerenigingWerdGeregistreerd.Naam,
                    KorteNaam: feitelijkeVerenigingWerdGeregistreerd.KorteNaam,
                    KorteBeschrijving: feitelijkeVerenigingWerdGeregistreerd.KorteBeschrijving,
                    Startdatum: feitelijkeVerenigingWerdGeregistreerd.Startdatum,
                    Doelgroep: feitelijkeVerenigingWerdGeregistreerd.Doelgroep,
                    IsUitgeschrevenUitPubliekeDatastroom: feitelijkeVerenigingWerdGeregistreerd.IsUitgeschrevenUitPubliekeDatastroom,
                    Contactgegevens: feitelijkeVerenigingWerdGeregistreerd.Contactgegevens,
                    Locaties: feitelijkeVerenigingWerdGeregistreerd.Locaties,
                    Vertegenwoordigers: enrichedVertegenwoordigers,
                    HoofdactiviteitenVerenigingsloket: feitelijkeVerenigingWerdGeregistreerd.HoofdactiviteitenVerenigingsloket
                );
            });

        opts.Events.Upcast<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd, VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdMetPersoonsgegevens>(
            async (verenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd, ct) =>
            {
                await using var session = querySessionFunc();

                var refIdsFromFeitelijkeVereniging = verenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.Vertegenwoordigers
                                                                                          .Select(x => x.RefId)
                                                                                          .ToList();

                var vertegenwoordigerPersoonsgegevens = await session
                                                             .Query<VertegenwoordigerPersoonsgegevensDocument>()
                                                             .Where(x => refIdsFromFeitelijkeVereniging.Contains(x.RefId))
                                                             .ToListAsync(ct);

                var persoonsgegevensByRefId = vertegenwoordigerPersoonsgegevens
                   .ToDictionary(x => x.RefId);

                var enrichedVertegenwoordigers = verenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.Vertegenwoordigers
                                                                                      .Select(v =>
                                                                                       {
                                                                                           persoonsgegevensByRefId.TryGetValue(
                                                                                               v.RefId, out var doc);

                                                                                           var vertegenwoordigerPersoongegevens = doc is null
                                                                                               ? null
                                                                                               : new VertegenwoordigerPersoonsgegevens(
                                                                                                   doc.Insz,
                                                                                                   doc.Roepnaam,
                                                                                                   doc.Rol,
                                                                                                   doc.Voornaam,
                                                                                                   doc.Achternaam,
                                                                                                   doc.Email,
                                                                                                   doc.Telefoon,
                                                                                                   doc.Mobiel,
                                                                                                   doc.SocialMedia);

                                                                                           return new EnrichedVertegenwoordiger(
                                                                                               v.RefId,
                                                                                               v.VertegenwoordigerId,
                                                                                               v.IsPrimair,
                                                                                               vertegenwoordigerPersoongegevens);
                                                                                       })
                                                                                      .ToArray();

                return new VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdMetPersoonsgegevens(
                    VCode: verenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode,
                    Naam: verenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.Naam,
                    KorteNaam: verenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.KorteNaam,
                    KorteBeschrijving: verenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.KorteBeschrijving,
                    Startdatum: verenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.Startdatum,
                    Doelgroep: verenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.Doelgroep,
                    IsUitgeschrevenUitPubliekeDatastroom: verenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.IsUitgeschrevenUitPubliekeDatastroom,
                    Contactgegevens: verenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.Contactgegevens,
                    Locaties: verenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.Locaties,
                    Vertegenwoordigers: enrichedVertegenwoordigers,
                    HoofdactiviteitenVerenigingsloket: verenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.HoofdactiviteitenVerenigingsloket
                );
            });

        return opts;
    }
}
