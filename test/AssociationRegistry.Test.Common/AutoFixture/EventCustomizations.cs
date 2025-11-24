namespace AssociationRegistry.Test.Common.AutoFixture;

using DecentraalBeheer.Vereniging;
using DecentraalBeheer.Vereniging.Emails;
using DecentraalBeheer.Vereniging.SocialMedias;
using DecentraalBeheer.Vereniging.TelefoonNummers;
using Events;
using Events.Factories;
using global::AutoFixture;
using Vereniging;

public static class EventCustomizations
{
    public static void CustomizeEvents(Fixture fixture)
    {
        fixture.CustomizeFeitelijkeVerenigingWerdGeregistreerd();
        fixture.CustomizeVerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd();
        fixture.CustomizeVerenigingMetRechtspersoonlijkheidWerdGeregistreerd();
        fixture.CustomizeContactgegevenWerdToegevoegd();
        fixture.CustomizeVertegenwoordigerWerdToegevoegd();
        fixture.CustomizeContactgegevenWerdOvergenomenUitKBO();
        fixture.CustomizeVertegenwoordigerWerdGewijzigdInKBO();
        fixture.CustomizeVertegenwoordigerWerdOvergenomenUitKBO();
        fixture.CustomizeContactgegevenKonNietOvergenomenWordenUitKBO();
        fixture.CustomizeRechtsvormWerdGewijzigdInKBO();
        fixture.CustomizeLidmaatschapWerdToegevoegd();
        fixture.CustomizeWeigeringDubbelDoorAuthentiekeVerenigingWerdVerwerkt();
        fixture.CustomizeVerenigingAanvaarddeDubbeleVereniging();
        fixture.CustomizeVertegenwoordigerWerdToegevoegdVanuitKBO();
    }

    private static void CustomizeLidmaatschapWerdToegevoegd(this IFixture fixture)
    {
        fixture.Customize<LidmaatschapWerdToegevoegd>(
            composer =>
                composer.FromFactory(
                             () => new LidmaatschapWerdToegevoegd(
                                 fixture.Create<VCode>(),
                                 EventFactory.Lidmaatschap(
                                     Lidmaatschap.Hydrate(
                                         fixture.Create<int>(),
                                         fixture.Create<VCode>(),
                                         fixture.Create<string>(),
                                         fixture.Create<Geldigheidsperiode>(),
                                         fixture.Create<string>(),
                                         fixture.Create<string>()
                                     ))))
                        .OmitAutoProperties()
        );
    }

    private static void CustomizeVertegenwoordigerWerdToegevoegdVanuitKBO(this IFixture fixture)
    {
        fixture.Customize<VertegenwoordigerWerdToegevoegdVanuitKBO>(
            composer =>
                composer.FromFactory(
                             () => new VertegenwoordigerWerdToegevoegdVanuitKBO(
                                 fixture.Create<int>(),
                                 fixture.Create<Insz>(),
                                 fixture.Create<Voornaam>(),
                                 fixture.Create<Achternaam>()))
                        .OmitAutoProperties()
        );
    }

    private static void CustomizeVerenigingMetRechtspersoonlijkheidWerdGeregistreerd(this IFixture fixture)
    {
        fixture.Customize<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>(
            composer => composer.FromFactory<int>(
                i => new VerenigingMetRechtspersoonlijkheidWerdGeregistreerd(
                    fixture.Create<VCode>().ToString(),
                    fixture.Create<KboNummer>(),
                    new[]
                    {
                        Verenigingstype.IVZW, Verenigingstype.VZW, Verenigingstype.PrivateStichting,
                        Verenigingstype.StichtingVanOpenbaarNut,
                    }[i % 4].Code,
                    fixture.Create<string>(),
                    fixture.Create<string>(),
                    fixture.Create<DateOnly?>()
                )).OmitAutoProperties());
    }

    private static void CustomizeContactgegevenWerdToegevoegd(this IFixture fixture)
    {
        fixture.Customize<ContactgegevenWerdToegevoegd>(
            composer => composer.FromFactory(
                                     () =>
                                     {
                                         var contactgegeven = fixture.Create<Contactgegeven>();

                                         return new ContactgegevenWerdToegevoegd(
                                             contactgegeven.ContactgegevenId,
                                             contactgegeven.Contactgegeventype,
                                             contactgegeven.Waarde,
                                             contactgegeven.Beschrijving,
                                             contactgegeven.IsPrimair);
                                     })
                                .OmitAutoProperties());
    }

    private static void CustomizeContactgegevenWerdOvergenomenUitKBO(this IFixture fixture)
    {
        fixture.Customize<ContactgegevenWerdOvergenomenUitKBO>(
            composer => composer.FromFactory<int>(
                                     i =>
                                     {
                                         var typeVolgensKbo = ContactgegeventypeVolgensKbo.All[i % ContactgegeventypeVolgensKbo.All.Length];
                                         var contactgegeven = fixture.CreateContactgegevenVolgensType(typeVolgensKbo.Contactgegeventype);

                                         return new ContactgegevenWerdOvergenomenUitKBO(
                                             contactgegeven.ContactgegevenId,
                                             typeVolgensKbo.Contactgegeventype.Waarde,
                                             typeVolgensKbo.Waarde,
                                             contactgegeven.Waarde);
                                     })
                                .OmitAutoProperties());
    }

    private static void CustomizeVertegenwoordigerWerdOvergenomenUitKBO(this IFixture fixture)
    {
        fixture.Customize<VertegenwoordigerWerdOvergenomenUitKBO>(
            composer => composer.FromFactory(() => new VertegenwoordigerWerdOvergenomenUitKBO(
                                                 fixture.Create<int>(),
                                                 fixture.Create<Insz>(),
                                                 fixture.Create<Voornaam>(),
                                                 fixture.Create<Achternaam>()))
                                .OmitAutoProperties());
    }

    private static void CustomizeVertegenwoordigerWerdGewijzigdInKBO(this IFixture fixture)
    {
        fixture.Customize<VertegenwoordigerWerdGewijzigdInKBO>(
            composer => composer.FromFactory(() => new VertegenwoordigerWerdGewijzigdInKBO(
                                                 fixture.Create<int>(),
                                                 fixture.Create<Insz>(),
                                                 fixture.Create<Voornaam>(),
                                                 fixture.Create<Achternaam>()))
                                .OmitAutoProperties());
    }

    private static void CustomizeContactgegevenKonNietOvergenomenWordenUitKBO(this IFixture fixture)
    {
        fixture.Customize<ContactgegevenKonNietOvergenomenWordenUitKBO>(
            composer => composer.FromFactory(
                                     () =>
                                     {
                                         var contactgegevenTypevolgensKbo = fixture.Create<ContactgegeventypeVolgensKbo>();

                                         return new ContactgegevenKonNietOvergenomenWordenUitKBO(
                                             contactgegevenTypevolgensKbo.Contactgegeventype.Waarde,
                                             contactgegevenTypevolgensKbo.Waarde,
                                             fixture.Create<string>());
                                     })
                                .OmitAutoProperties());
    }

    private static void CustomizeVertegenwoordigerWerdToegevoegd(this IFixture fixture)
    {
        fixture.Customize<VertegenwoordigerWerdToegevoegd>(
            composer => composer.FromFactory(
                                     () => new VertegenwoordigerWerdToegevoegd(
                                         fixture.Create<int>(),
                                         fixture.Create<Insz>(),
                                         IsPrimair: false,
                                         fixture.Create<string>(),
                                         fixture.Create<string>(),
                                         fixture.Create<Voornaam>(),
                                         fixture.Create<Achternaam>(),
                                         fixture.Create<Email>().Waarde,
                                         fixture.Create<TelefoonNummer>().Waarde,
                                         fixture.Create<TelefoonNummer>().Waarde,
                                         fixture.Create<SocialMedia>().Waarde
                                     ))
                                .OmitAutoProperties());
    }

    private static void CustomizeFeitelijkeVerenigingWerdGeregistreerd(this IFixture fixture)
    {
        fixture.Customize<FeitelijkeVerenigingWerdGeregistreerd>(
            composer => composer.FromFactory(
                () => new FeitelijkeVerenigingWerdGeregistreerd(
                    fixture.Create<VCode>().ToString(),
                    fixture.Create<string>(),
                    fixture.Create<string>(),
                    fixture.Create<string>(),
                    fixture.Create<DateOnly?>(),
                    fixture.Create<Registratiedata.Doelgroep>(),
                    IsUitgeschrevenUitPubliekeDatastroom: false,
                    fixture.CreateMany<Registratiedata.Contactgegeven>().ToArray(),
                    fixture.CreateMany<Registratiedata.Locatie>().ToArray(),
                    fixture.CreateMany<Registratiedata.Vertegenwoordiger>().ToArray(),
                    fixture.CreateMany<Registratiedata.HoofdactiviteitVerenigingsloket>().ToArray()
                )).OmitAutoProperties());
    }

    private static void CustomizeVerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd(this IFixture fixture)
    {
        fixture.Customize<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>(
            composer => composer.FromFactory(
                () => new VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd(
                    fixture.Create<VCode>().ToString(),
                    fixture.Create<string>(),
                    fixture.Create<string>(),
                    fixture.Create<string>(),
                    fixture.Create<DateOnly?>(),
                    fixture.Create<Registratiedata.Doelgroep>(),
                    IsUitgeschrevenUitPubliekeDatastroom: false,
                    fixture.CreateMany<Registratiedata.Contactgegeven>()
                           .OrderBy(x => x.ContactgegevenId).ToArray(),
                    fixture.CreateMany<Registratiedata.Locatie>()
                           .OrderBy(x => x.LocatieId).ToArray(),
                    fixture.CreateMany<Registratiedata.Vertegenwoordiger>()
                           .OrderBy(x => x.VertegenwoordigerId).ToArray(),
                    fixture.CreateMany<Registratiedata.HoofdactiviteitVerenigingsloket>().ToArray(),
                    Registratiedata.DuplicatieInfo.GeenDuplicaten
                )).OmitAutoProperties());
    }

    private static void CustomizeRechtsvormWerdGewijzigdInKBO(this IFixture fixture)
    {
        fixture.Customize<RechtsvormWerdGewijzigdInKBO>(
            composer => composer.FromFactory(
                () => new RechtsvormWerdGewijzigdInKBO(
                    fixture.Create<Verenigingstype>().Code)).OmitAutoProperties());
    }

    private static void CustomizeWeigeringDubbelDoorAuthentiekeVerenigingWerdVerwerkt(this IFixture fixture)
    {
        fixture.Customize<WeigeringDubbelDoorAuthentiekeVerenigingWerdVerwerkt>(
            composer => composer.FromFactory(
                () => new WeigeringDubbelDoorAuthentiekeVerenigingWerdVerwerkt(
                    fixture.Create<VCode>().ToString(),
                    fixture.Create<VCode>().ToString(),
                    VerenigingStatus.Actief.StatusNaam
                )).OmitAutoProperties());
    }

    private static void CustomizeVerenigingAanvaarddeDubbeleVereniging(this IFixture fixture)
    {
        fixture.Customize<VerenigingAanvaarddeDubbeleVereniging>(
            composer => composer.FromFactory(
                () => new VerenigingAanvaarddeDubbeleVereniging(
                    fixture.Create<VCode>().ToString(),
                    fixture.Create<VCode>().ToString()
                )).OmitAutoProperties());
    }
}
