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
    }

    private static void CustomizeVerenigingMetRechtspersoonlijkheidWerdGeregistreerd(this IFixture fixture)
    {
        fixture.Customize<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>(
            composer => composer.FromFactory(
                () => new VerenigingMetRechtspersoonlijkheidWerdGeregistreerd(
                    fixture.Create<VCode>().ToString(),
                    fixture.Create<KboNummer>(),
                    fixture.Create<string>(),
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
                    fixture.Create<VCode>().ToString(),
                    fixture.Create<string>(),
                    new AfdelingWerdGeregistreerd.MoederverenigingsData(
                        fixture.Create<KboNummer>(),
                        fixture.Create<VCode>(),
                        fixture.Create<VerenigingsNaam>()),
                    fixture.Create<string>(),
                    fixture.Create<string>(),
                    fixture.Create<DateOnly?>(),
                    fixture.CreateMany<Registratiedata.Contactgegeven>().ToArray(),
                    fixture.CreateMany<Registratiedata.Locatie>().ToArray(),
                    fixture.CreateMany<Registratiedata.Vertegenwoordiger>().ToArray(),
                    fixture.CreateMany<Registratiedata.HoofdactiviteitVerenigingsloket>().ToArray()
                )).OmitAutoProperties());
    }
}
