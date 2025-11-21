namespace AssociationRegistry.MartenDb.Upcasters.Persoonsgegevens;

using AssociationRegistry.Events;
using AssociationRegistry.Persoonsgegevens;
using Marten;

public static class EventUpcaster
{
    public static StoreOptions UpcastEvents(this StoreOptions opts, Func<IQuerySession> querySessionFunc)
    {
        var vertegenwoordigerWerdToegevoegdUpcaster = new VertegenwoordigerWerdToegevoegdUpcaster(querySessionFunc);
        opts.Events.Upcast<VertegenwoordigerWerdToegevoegdZonderPersoonsgegevens, VertegenwoordigerWerdToegevoegd>(
            vertegenwoordigerWerdToegevoegdUpcaster.UpcastAsync);

        var vertegenwoordigerWerdGewijzigdUpcaster = new VertegenwoordigerWerdGewijzigdUpcaster(querySessionFunc);
        opts.Events.Upcast<VertegenwoordigerWerdGewijzigdZonderPersoongegevens, VertegenwoordigerWerdGewijzigd>(
            vertegenwoordigerWerdGewijzigdUpcaster.UpcastAsync);

        var vertegenwoordigerWerdVerwijderdUpcaster = new VertegenwoordigerWerdVerwijderdUpcaster(querySessionFunc);
        opts.Events.Upcast<VertegenwoordigerWerdVerwijderdZonderPersoonsgegevens, VertegenwoordigerWerdVerwijderd>(
            vertegenwoordigerWerdVerwijderdUpcaster.UpcastAsync);

        var vertegenwoordigerWerdOvergenomenUitKBOUpcaster = new VertegenwoordigerWerdOvergenomenUitKBOUpcaster(querySessionFunc);
        opts.Events.Upcast<VertegenwoordigerWerdOvergenomenUitKBOZonderPersoonsgegevens, VertegenwoordigerWerdOvergenomenUitKBO>(
            vertegenwoordigerWerdOvergenomenUitKBOUpcaster.UpcastAsync);

        var vertegenwoordigerWerdToegevoegdVanuitKBO = new VertegenwoordigerWerdToegevoegdVanuitKBOUpcaster(querySessionFunc);
        opts.Events.Upcast<VertegenwoordigerWerdToegevoegdVanuitKBOZonderPersoonsgegevens, VertegenwoordigerWerdToegevoegdVanuitKBO>(
            vertegenwoordigerWerdToegevoegdVanuitKBO.UpcastAsync);

        var feitelijkeVerenigingWerdGeregistreerdUpcaster = new FeitelijkeVerenigingWerdGeregistreerdUpcaster(querySessionFunc);
        opts.Events.Upcast<FeitelijkeVerenigingWerdGeregistreerdZonderPersoonsgegevens, FeitelijkeVerenigingWerdGeregistreerd>(
            feitelijkeVerenigingWerdGeregistreerdUpcaster.UpcastAsync);

        var verenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdUpcaster = new VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdUpcaster(querySessionFunc);
        opts.Events.Upcast<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdZonderPersoonsgegevens, VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>(
            verenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdUpcaster.UpcastAsync);

        return opts;
    }
}
