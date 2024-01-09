namespace AssociationRegistry.Test.Admin.Api.Framework;

using AssociationRegistry.Admin.Api.Verenigingen.Common;
using AssociationRegistry.Admin.Api.Verenigingen.Contactgegevens.FeitelijkeVereniging.VoegContactGegevenToe.RequestsModels;
using AssociationRegistry.Admin.Api.Verenigingen.Locaties.FeitelijkeVereniging.WijzigLocatie.RequestModels;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer.FeitelijkeVereniging.RequetsModels;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer.MetRechtspersoonlijkheid.RequestModels;
using AssociationRegistry.Admin.Api.Verenigingen.Vertegenwoordigers.FeitelijkeVereniging.WijzigVertegenwoordiger.RequestModels;
using AssociationRegistry.Admin.Api.Verenigingen.WijzigBasisgegevens.FeitelijkeVereniging.RequestModels;
using AutoFixture;
using Primitives;
using Test.Framework.Customizations;
using Vereniging;
using Vereniging.Emails;
using Vereniging.SocialMedias;
using Vereniging.TelefoonNummers;
using Adres = AssociationRegistry.Admin.Api.Verenigingen.Common.Adres;
using AdresId = AssociationRegistry.Admin.Api.Verenigingen.Common.AdresId;

public static class AutoFixtureCustomizations
{
    public static Fixture CustomizeAdminApi(this Fixture fixture)
    {
        fixture.CustomizeDomain();

        fixture.CustomizeRegistreerFeitelijkeVerenigingRequest();
        fixture.CustomizeRegistreerVerenigingUitKboRequest();

        fixture.CustomizeWijzigBasisgegevensRequest();

        fixture.CustomizeVoegContactgegevenToeRequest();

        fixture.CustomizeDoelgroepRequest();
        fixture.CustomizeToeTeVoegenLocatie();
        fixture.CustomizeToeTeVoegenContactgegeven();
        fixture.CustomizeToeTeVoegenVertegenwoordiger();

        fixture.CustomizeWijzigVertegenwoordigerRequest();
        fixture.CustomizeWijzigLocatieRequest();

        fixture.CustomizeTestEvent(typeof(TestEvent<>));

        fixture.CustomizeMagdaResponses();

        return fixture;
    }

    private static void CustomizeWijzigBasisgegevensRequest(this IFixture fixture)
    {
        fixture.Customize<WijzigBasisgegevensRequest>(
            composer => composer.FromFactory(
                () => new WijzigBasisgegevensRequest
                {
                    Naam = fixture.Create<string>(),
                    KorteNaam = fixture.Create<string>(),
                    KorteBeschrijving = fixture.Create<string>(),
                    Startdatum = NullOrEmpty<DateOnly>.Create(fixture.Create<DateOnly>()),
                    Doelgroep = new DoelgroepRequest
                    {
                        Minimumleeftijd = fixture.Create<int>() % 50,
                        Maximumleeftijd = 50 + fixture.Create<int>() % 50,
                    },
                    HoofdactiviteitenVerenigingsloket = fixture
                                                       .CreateMany<HoofdactiviteitVerenigingsloket>()
                                                       .Distinct()
                                                       .Select(h => h.Code)
                                                       .ToArray(),
                }).OmitAutoProperties());

        fixture
           .Customize<AssociationRegistry.Admin.Api.Verenigingen.WijzigBasisgegevens.MetRechtspersoonlijkheid.RequestModels.
                WijzigBasisgegevensRequest>(
                composer => composer.With(
                    propertyPicker: e => e.HoofdactiviteitenVerenigingsloket,
                    valueFactory: () => fixture
                                       .CreateMany<HoofdactiviteitVerenigingsloket>()
                                       .Distinct()
                                       .Select(h => h.Code)
                                       .ToArray()));
    }

    private static void CustomizeRegistreerFeitelijkeVerenigingRequest(this IFixture fixture)
    {
        fixture.Customize<RegistreerFeitelijkeVerenigingRequest>(
            composer => composer.FromFactory<int>(
                _ =>
                {
                    var datum = fixture.Create<Datum>();
                    var startDatum = new DateOnly(new Random().Next(1970, DateTime.Now.Year), datum.Value.Month, datum.Value.Day);
                    var request = new RegistreerFeitelijkeVerenigingRequest();

                    request.Contactgegevens = fixture.CreateMany<ToeTeVoegenContactgegeven>().ToArray();
                    request.Locaties = fixture.CreateMany<ToeTeVoegenLocatie>().DistinctBy(l => l.Locatietype).ToArray();
                    request.Startdatum = startDatum;
                    request.Naam = fixture.Create<string>();
                    request.Doelgroep = fixture.Create<DoelgroepRequest>();
                    request.Vertegenwoordigers = fixture.CreateMany<ToeTeVoegenVertegenwoordiger>().ToArray();

                    request.HoofdactiviteitenVerenigingsloket = fixture.CreateMany<HoofdactiviteitVerenigingsloket>()
                                                                       .Select(x => x.Code)
                                                                       .Distinct()
                                                                       .ToArray();

                    request.KorteBeschrijving = fixture.Create<string>();
                    request.KorteNaam = fixture.Create<string>();

                    return request;
                }).OmitAutoProperties());
    }

    private static void CustomizeToeTeVoegenContactgegeven(this IFixture fixture)
    {
        fixture.Customize<ToeTeVoegenContactgegeven>(
            composerTransformation: composer => composer.FromFactory(
                                                             factory: () =>
                                                             {
                                                                 var contactgegeven = fixture.Create<Contactgegeven>();

                                                                 return new ToeTeVoegenContactgegeven
                                                                 {
                                                                     Contactgegeventype = contactgegeven.Contactgegeventype,
                                                                     Waarde = contactgegeven.Waarde,
                                                                     Beschrijving = fixture.Create<string>(),
                                                                     IsPrimair = false,
                                                                 };
                                                             })
                                                        .OmitAutoProperties());
    }

    private static void CustomizeToeTeVoegenVertegenwoordiger(this IFixture fixture)
    {
        fixture.Customize<ToeTeVoegenVertegenwoordiger>(
            composerTransformation: composer => composer.FromFactory(
                                                             factory: () => new ToeTeVoegenVertegenwoordiger
                                                             {
                                                                 Insz = fixture.Create<Insz>(),
                                                                 Voornaam = fixture.Create<Voornaam>(),
                                                                 Achternaam = fixture.Create<Achternaam>(),
                                                                 Roepnaam = fixture.Create<string>(),
                                                                 Rol = fixture.Create<string>(),
                                                                 IsPrimair = false,
                                                                 Email = fixture.Create<Email>().Waarde,
                                                                 Telefoon = fixture.Create<TelefoonNummer>().Waarde,
                                                                 Mobiel = fixture.Create<TelefoonNummer>().Waarde,
                                                                 SocialMedia = fixture.Create<SocialMedia>().Waarde,
                                                             })
                                                        .OmitAutoProperties());
    }

    private static void CustomizeToeTeVoegenLocatie(this IFixture fixture)
    {
        fixture.Customize<ToeTeVoegenLocatie>(
            composer => composer.FromFactory<int>(
                value => new ToeTeVoegenLocatie
                {
                    Locatietype = fixture.Create<Locatietype>(),
                    Naam = fixture.Create<string>(),
                    Adres = new Adres
                    {
                        Straatnaam = fixture.Create<string>(),
                        Huisnummer = fixture.Create<int>().ToString(),
                        Busnummer = fixture.Create<string?>(),
                        Postcode = (fixture.Create<int>() % 10000).ToString(),
                        Gemeente = fixture.Create<string>(),
                        Land = fixture.Create<string>(),
                    },
                    AdresId = new AdresId
                    {
                        Broncode = Adresbron.All[value % Adresbron.All.Length],
                        Bronwaarde = new Uri("https://data.vlaanderen.be/id/adres/" + fixture.Create<int>()).ToString(),
                    },
                    IsPrimair = false,
                }).OmitAutoProperties());
    }

    private static void CustomizeDoelgroepRequest(this IFixture fixture)
    {
        fixture.Customize<DoelgroepRequest>(
            composer => composer.FromFactory(
                () => new DoelgroepRequest
                {
                    Minimumleeftijd = fixture.Create<int>() % 50,
                    Maximumleeftijd = 50 + fixture.Create<int>() % 50,
                }).OmitAutoProperties());
    }

    private static void CustomizeVoegContactgegevenToeRequest(this IFixture fixture)
    {
        fixture.Customize<VoegContactgegevenToeRequest>(
            composerTransformation: composer => composer.FromFactory(
                                                             factory: () => new VoegContactgegevenToeRequest
                                                             {
                                                                 Contactgegeven = fixture.Create<ToeTeVoegenContactgegeven>(),
                                                             }
                                                         )
                                                        .OmitAutoProperties());
    }

    private static void CustomizeWijzigVertegenwoordigerRequest(this IFixture fixture)
    {
        fixture.Customize<WijzigVertegenwoordigerRequest>(
            composerTransformation: composer => composer.FromFactory(
                                                             factory: () => new WijzigVertegenwoordigerRequest
                                                             {
                                                                 Vertegenwoordiger = new TeWijzigenVertegenwoordiger
                                                                 {
                                                                     Email = fixture.Create<Email>().Waarde,
                                                                     Telefoon = fixture.Create<TelefoonNummer>().Waarde,
                                                                     Mobiel = fixture.Create<TelefoonNummer>().Waarde,
                                                                     SocialMedia = fixture.Create<SocialMedia>().Waarde,
                                                                     Rol = fixture.Create<string>(),
                                                                     Roepnaam = fixture.Create<string>(),
                                                                     IsPrimair = false,
                                                                 },
                                                             })
                                                        .OmitAutoProperties());
    }

    private static void CustomizeWijzigLocatieRequest(this IFixture fixture)
    {
        fixture.Customize<TeWijzigenLocatie>(
            composer => composer.FromFactory<int>(
                value => new TeWijzigenLocatie
                {
                    Locatietype = fixture.Create<Locatietype>(),
                    Naam = fixture.Create<string>(),
                    Adres = new Adres
                    {
                        Straatnaam = fixture.Create<string>(),
                        Huisnummer = fixture.Create<int>().ToString(),
                        Busnummer = fixture.Create<string?>(),
                        Postcode = (fixture.Create<int>() % 10000).ToString(),
                        Gemeente = fixture.Create<string>(),
                        Land = fixture.Create<string>(),
                    },
                    AdresId = new AdresId
                    {
                        Broncode = Adresbron.All[value % Adresbron.All.Length],
                        Bronwaarde = new Uri("https://data.vlaanderen.be/id/adres/" + fixture.Create<int>()).ToString(),
                    },
                    IsPrimair = false,
                }).OmitAutoProperties());
    }

    private static void CustomizeRegistreerVerenigingUitKboRequest(this IFixture fixture)
    {
        fixture.Customize<RegistreerVerenigingUitKboRequest>(
            composerTransformation: composer => composer.FromFactory(
                                                             factory: () => new RegistreerVerenigingUitKboRequest
                                                             {
                                                                 KboNummer = fixture.Create<KboNummer>(),
                                                             })
                                                        .OmitAutoProperties());
    }
}
