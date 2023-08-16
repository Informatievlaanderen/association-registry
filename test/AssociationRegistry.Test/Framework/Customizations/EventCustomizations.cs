namespace AssociationRegistry.Test.Framework.Customizations;

using AutoFixture;
using Events;
using Vereniging;
using Vereniging.Emails;
using Vereniging.SocialMedias;
using Vereniging.TelefoonNummers;

public static class EventCustomizations
{
    public static void CustomizeEvents(Fixture fixture)
    {
        fixture.CustomizeFeitelijkeVerenigingWerdGeregistreerd();
        fixture.CustomizeAfdelingWerdGeregistreerd();
        fixture.CustomizeVerenigingMetRechtspersoonlijkheidWerdGeregistreerd();
        fixture.CustomizeContactgegevenWerdToegevoegd();
        fixture.CustomizeVertegenwoordigerWerdToegevoegd();
        fixture.CustomizeContactgegevenWerdOvergenomenUitKBO();
        fixture.CustomizeContactgegevenKonNietOvergenomenWordenUitKBO();
    }

    private static void CustomizeVerenigingMetRechtspersoonlijkheidWerdGeregistreerd(this IFixture fixture)
    {
        fixture.Customize<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>(
            composer => composer.FromFactory<int>(
                (i) => new VerenigingMetRechtspersoonlijkheidWerdGeregistreerd(
                    fixture.Create<VCode>().ToString(),
                    fixture.Create<KboNummer>(),
                    new[] { Verenigingstype.IVZW, Verenigingstype.VZW, Verenigingstype.PrivateStichting, Verenigingstype.StichtingVanOpenbaarNut }[i % 4].Code,
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
                            contactgegeven.Type,
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
                        var contactgegeven = fixture.Create<Contactgegeven>();
                        return new ContactgegevenWerdOvergenomenUitKBO(
                            contactgegeven.ContactgegevenId,
                            ContactgegevenTypeVolgensKbo.All[i % (ContactgegevenTypeVolgensKbo.All.Length - 1)],
                            contactgegeven.Type,
                            contactgegeven.Waarde);
                    })
                .OmitAutoProperties());
    }

    private static void CustomizeContactgegevenKonNietOvergenomenWordenUitKBO(this IFixture fixture)
    {
        fixture.Customize<ContactgegevenKonNietOvergenomenWordenUitKBO>(
            composer => composer.FromFactory(
                    () => new ContactgegevenKonNietOvergenomenWordenUitKBO(
                        fixture.Create<ContactgegevenType>().Waarde,
                        fixture.Create<string>()))
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

    private static void CustomizeAfdelingWerdGeregistreerd(this IFixture fixture)
    {
        fixture.Customize<AfdelingWerdGeregistreerd>(
            composer => composer.FromFactory(
                () => new AfdelingWerdGeregistreerd(
                    VCode: fixture.Create<VCode>().ToString(),
                    Naam: fixture.Create<string>(),
                    Moedervereniging: new AfdelingWerdGeregistreerd.MoederverenigingsData(
                        fixture.Create<KboNummer>(),
                        fixture.Create<VCode>(),
                        fixture.Create<VerenigingsNaam>()),
                    KorteNaam: fixture.Create<string>(),
                    KorteBeschrijving: fixture.Create<string>(),
                    Startdatum: fixture.Create<DateOnly?>(),
                    Doelgroep: fixture.Create<Registratiedata.Doelgroep>(),
                    Contactgegevens: fixture.CreateMany<Registratiedata.Contactgegeven>().ToArray(),
                    Locaties: fixture.CreateMany<Registratiedata.Locatie>().ToArray(),
                    Vertegenwoordigers: fixture.CreateMany<Registratiedata.Vertegenwoordiger>().ToArray(),
                    HoofdactiviteitenVerenigingsloket: fixture.CreateMany<Registratiedata.HoofdactiviteitVerenigingsloket>().ToArray()
                )).OmitAutoProperties());
    }
}
