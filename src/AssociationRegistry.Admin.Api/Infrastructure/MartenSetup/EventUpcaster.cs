namespace AssociationRegistry.Admin.Api.Infrastructure.MartenSetup;

using Events;
using AssociationRegistry.MartenDb.Upcasters.Persoonsgegevens;
using Marten;

public static class EventUpcaster
{
    public static StoreOptions UpcastEvents(this StoreOptions opts, Func<IQuerySession> querySessionFunc)
    {
        var vertegenwoordigerWerdToegevoegdUpcaster = new VertegenwoordigerWerdToegevoegdUpcaster(querySessionFunc);
        opts.Events.Upcast<VertegenwoordigerWerdToegevoegdZonderPersoonsgegevens, VertegenwoordigerWerdToegevoegd>(
            vertegenwoordigerWerdToegevoegdUpcaster.UpcastAsync);

        var vertegenwoordigerWerdVerwijderdUpcaster = new VertegenwoordigerWerdVerwijderdUpcaster(querySessionFunc);
        opts.Events.Upcast<VertegenwoordigerWerdVerwijderdZonderPersoonsgegevens, VertegenwoordigerWerdVerwijderd>(
            vertegenwoordigerWerdVerwijderdUpcaster.UpcastAsync);
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
        var feitelijkeVerenigingWerdGeregistreerdUpcaster = new FeitelijkeVerenigingWerdGeregistreerdUpcaster(querySessionFunc);
        opts.Events.Upcast<FeitelijkeVerenigingWerdGeregistreerdZonderPersoonsgegevens, FeitelijkeVerenigingWerdGeregistreerd>(
            feitelijkeVerenigingWerdGeregistreerdUpcaster.UpcastAsync);

        var verenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdUpcaster = new VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdUpcaster(querySessionFunc);
        opts.Events.Upcast<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdZonderPersoonsgegevens, VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>(
            verenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdUpcaster.UpcastAsync);

        return opts;
    }
}
