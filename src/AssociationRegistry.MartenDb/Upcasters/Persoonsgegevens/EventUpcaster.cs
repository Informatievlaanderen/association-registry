namespace AssociationRegistry.MartenDb.Upcasters.Persoonsgegevens;

using Events;
using Marten;

public static class EventUpcaster
{
    public static StoreOptions UpcastEvents(this StoreOptions opts, Func<IQuerySession> querySessionFunc)
    {
        var vertegenwoordigerWerdToegevoegdUpcaster = new VertegenwoordigerWerdToegevoegdUpcaster(querySessionFunc);
        opts.Events.Upcast<VertegenwoordigerWerdToegevoegdZonderPersoonsgegevens, VertegenwoordigerWerdToegevoegd>(
            vertegenwoordigerWerdToegevoegdUpcaster.UpcastAsync);

        var vertegenwoordigerWerdGewijzigdUpcaster = new VertegenwoordigerWerdGewijzigdUpcaster(querySessionFunc);
        opts.Events.Upcast<VertegenwoordigerWerdGewijzigdZonderPersoonsgegevens, VertegenwoordigerWerdGewijzigd>(
            vertegenwoordigerWerdGewijzigdUpcaster.UpcastAsync);

        var vertegenwoordigerWerdVerwijderdUpcaster = new VertegenwoordigerWerdVerwijderdUpcaster(querySessionFunc);
        opts.Events.Upcast<VertegenwoordigerWerdVerwijderdZonderPersoonsgegevens, VertegenwoordigerWerdVerwijderd>(
            vertegenwoordigerWerdVerwijderdUpcaster.UpcastAsync);

        var vertegenwoordigerWerdOvergenomenUitKBOUpcaster = new VertegenwoordigerWerdOvergenomenUitKBOUpcaster(querySessionFunc);
        opts.Events.Upcast<VertegenwoordigerWerdOvergenomenUitKBOZonderPersoonsgegevens, VertegenwoordigerWerdOvergenomenUitKBO>(
            vertegenwoordigerWerdOvergenomenUitKBOUpcaster.UpcastAsync);

        var vertegenwoordigerWerdGewijzigdInKBOUpcaster = new VertegenwoordigerWerdGewijzigdInKBOUpcaster(querySessionFunc);
        opts.Events.Upcast<VertegenwoordigerWerdGewijzigdInKBOZonderPersoonsgegevens, VertegenwoordigerWerdGewijzigdInKBO>(
            vertegenwoordigerWerdGewijzigdInKBOUpcaster.UpcastAsync);

        var vertegenwoordigerWerdToegevoegdVanuitKBOUpcaster = new VertegenwoordigerWerdToegevoegdVanuitKBOUpcaster(querySessionFunc);
        opts.Events.Upcast<VertegenwoordigerWerdToegevoegdVanuitKBOZonderPersoonsgegevens, VertegenwoordigerWerdToegevoegdVanuitKBO>(
            vertegenwoordigerWerdToegevoegdVanuitKBOUpcaster.UpcastAsync);

        var vertegenwoordigerWerdVerwijderdUitKboUpcaster = new VertegenwoordigerWerdVerwijderdUitKBOUpcaster(querySessionFunc);
        opts.Events.Upcast<VertegenwoordigerWerdVerwijderdUitKBOZonderPersoonsgegevens, VertegenwoordigerWerdVerwijderdUitKBO>(
            vertegenwoordigerWerdVerwijderdUitKboUpcaster.UpcastAsync);

        var feitelijkeVerenigingWerdGeregistreerdUpcaster = new FeitelijkeVerenigingWerdGeregistreerdUpcaster(querySessionFunc);
        opts.Events.Upcast<FeitelijkeVerenigingWerdGeregistreerdZonderPersoonsgegevens, FeitelijkeVerenigingWerdGeregistreerd>(
            feitelijkeVerenigingWerdGeregistreerdUpcaster.UpcastAsync);

        var verenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdUpcaster = new VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdUpcaster(querySessionFunc);
        opts.Events.Upcast<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdZonderPersoonsgegevens, VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>(
            verenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdUpcaster.UpcastAsync);

        return opts;
    }
}
