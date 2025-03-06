namespace AssociationRegistry.Test.Framework.Customizations;

using AutoFixture;
using Common.AutoFixture;
using Events;
using System;
using System.Linq;
using Vereniging;
using Vereniging.Emails;
using Vereniging.SocialMedias;
using Vereniging.TelefoonNummers;


public static class EventCustomizations
{
    public static void CustomizeEvents(Fixture fixture)
    {
        fixture.CustomizeFeitelijkeVerenigingWerdGeregistreerd();
        fixture.CustomizeVerenigingMetRechtspersoonlijkheidWerdGeregistreerd();
        fixture.CustomizeContactgegevenWerdToegevoegd();
        fixture.CustomizeVertegenwoordigerWerdToegevoegd();
        fixture.CustomizeContactgegevenWerdOvergenomenUitKBO();
        fixture.CustomizeVertegenwoordigerWerdOvergenomenUitKBO();
        fixture.CustomizeContactgegevenKonNietOvergenomenWordenUitKBO();
        fixture.CustomizeRechtsvormWerdGewijzigdInKBO();
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

    private static void CustomizeRechtsvormWerdGewijzigdInKBO(this IFixture fixture)
    {
        fixture.Customize<RechtsvormWerdGewijzigdInKBO>(
            composer => composer.FromFactory(
                () => new RechtsvormWerdGewijzigdInKBO(
                    fixture.Create<Verenigingstype>().Code)).OmitAutoProperties());
    }
}
