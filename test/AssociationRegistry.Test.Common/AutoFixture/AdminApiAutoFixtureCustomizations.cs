namespace AssociationRegistry.Test.Common.AutoFixture;

using Admin.Api.Verenigingen.Common;
using Admin.Api.Verenigingen.Contactgegevens.FeitelijkeVereniging.VoegContactGegevenToe.RequestsModels;
using Admin.Api.Verenigingen.Lidmaatschap.VoegLidmaatschapToe.RequestModels;
using Admin.Api.Verenigingen.Lidmaatschap.WijzigLidmaatschap.RequestModels;
using Admin.Api.Verenigingen.Locaties.FeitelijkeVereniging.WijzigLocatie.RequestModels;
using Admin.Api.Verenigingen.Registreer.FeitelijkeVereniging.RequestModels;
using Admin.Api.Verenigingen.Registreer.MetRechtspersoonlijkheid.RequestModels;
using Admin.Api.Verenigingen.Registreer.VerenigingZonderEigenRechtspersoonlijkheid.RequestModels;
using Admin.Api.Verenigingen.Subtype;
using Admin.Api.Verenigingen.Vertegenwoordigers.VerenigingOfAnyKind.WijzigVertegenwoordiger.RequestModels;
using Admin.Api.Verenigingen.WijzigBasisgegevens.FeitelijkeVereniging.RequestModels;
using Admin.Schema;
using Admin.Schema.Search;
using EventFactories;
using Events;
using Formats;
using global::AutoFixture;
using JsonLdContext;
using Primitives;
using Public.Api.Contexten;
using Vereniging;
using Vereniging.Emails;
using Vereniging.SocialMedias;
using Vereniging.TelefoonNummers;
using Adres = Admin.Api.Verenigingen.Common.Adres;
using Contactgegeven = Vereniging.Contactgegeven;
using HoofdactiviteitVerenigingsloket = Vereniging.HoofdactiviteitVerenigingsloket;
using Werkingsgebied = Vereniging.Werkingsgebied;

public static class AdminApiAutoFixtureCustomizations
{
    public static Fixture CustomizeAdminApi(this Fixture fixture)
    {
        fixture.CustomizeDomain();

        fixture.CustomizeRegistreerFeitelijkeVerenigingRequest();
        fixture.CustomizeRegistreerVerenigingZonderEigenRechtspersoonlijkheidRequest();
        fixture.CustomizeRegistreerVerenigingUitKboRequest();

        fixture.CustomizeWijzigBasisgegevensRequest();

        fixture.CustomizeVoegContactgegevenToeRequest();

        fixture.CustomizeDoelgroepRequest();
        fixture.CustomizeToeTeVoegenLocatie();
        fixture.CustomizeToeTeVoegenContactgegeven();
        fixture.CustomizeToeTeVoegenVertegenwoordiger();

        fixture.CustomizeWijzigVertegenwoordigerRequest();
        fixture.CustomizeWijzigLocatieRequest();

        fixture.CustomizeVoegLidmaatschapToeRequest();
        fixture.CustomizeWijzigLidmaatschapRequest();

        fixture.CustomizeLidmaatschapRegistratieData();
        fixture.CustomizeTestEvent(typeof(TestEvent<>));

        fixture.CustomizeMagdaResponses();

        fixture.CustomizeVerenigingZoekDocument();

        return fixture;
    }

    public static IFixture WithoutWerkingsgebieden(this IFixture fixture)
    {
        fixture.CustomizeRegistreerFeitelijkeVerenigingRequest(withoutWerkingsgebieden: true);
        fixture.CustomizeRegistreerFeitelijkeVerenigingCommand(withoutWerkingsgebieden: true);

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
                    Werkingsgebieden = fixture.CreateMany<Werkingsgebied>()
                                              .Distinct()
                                              .Select(x => x.Code)
                                              .ToArray(),
                }).OmitAutoProperties());

        fixture
           .Customize<Admin.Api.Verenigingen.WijzigBasisgegevens.MetRechtspersoonlijkheid.RequestModels.WijzigBasisgegevensRequest>(
                composer => composer.With(
                                         propertyPicker: e => e.HoofdactiviteitenVerenigingsloket,
                                         valueFactory: () => fixture
                                                            .CreateMany<HoofdactiviteitVerenigingsloket>()
                                                            .Distinct()
                                                            .Select(h => h.Code)
                                                            .ToArray())
                                    .With(
                                         propertyPicker: e => e.Werkingsgebieden,
                                         valueFactory: () => fixture.CreateMany<Werkingsgebied>()
                                                                    .Distinct()
                                                                    .Select(x => x.Code)
                                                                    .ToArray()));
    }

    private static void CustomizeRegistreerFeitelijkeVerenigingRequest(this IFixture fixture, bool withoutWerkingsgebieden = false)
    {
        fixture.Customize<RegistreerFeitelijkeVerenigingRequest>(
            composer => composer.FromFactory<int>(
                _ => CustomizeRegistreerVerenigingRequest<RegistreerFeitelijkeVerenigingRequest>(fixture, withoutWerkingsgebieden)).OmitAutoProperties());
    }

    private static void CustomizeRegistreerVerenigingZonderEigenRechtspersoonlijkheidRequest(this IFixture fixture, bool withoutWerkingsgebieden = false)
    {
        fixture.Customize<RegistreerVerenigingZonderEigenRechtspersoonlijkheidRequest>(
            composer => composer.FromFactory<int>(
                _ => CustomizeRegistreerVerenigingRequest<RegistreerVerenigingZonderEigenRechtspersoonlijkheidRequest>(fixture, withoutWerkingsgebieden)).OmitAutoProperties());
    }

    private static TRequest CustomizeRegistreerVerenigingRequest<TRequest>(IFixture fixture, bool withoutWerkingsgebieden)
        where TRequest : IRegistreerVereniging, new()
    {
        var datum = fixture.Create<Datum>();
        var startDatum = new DateOnly(new Random().Next(minValue: 1970, DateTime.Now.Year),
                                      datum.Value.Month,
                                      Math.Min(datum.Value.Day, 28));
        var request = new TRequest();

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

        request.Werkingsgebieden = withoutWerkingsgebieden
            ? []
            : fixture.CreateMany<Werkingsgebied>()
                     .Distinct()
                     .Select(x => x.Code).ToArray();

        return request;
    }

    private static void CustomizeVerenigingZoekDocument(this IFixture fixture, bool withoutWerkingsgebieden = false)
    {
        fixture.Customize<VerenigingZoekDocument>(
            composer => composer.FromFactory<int>(
                _ =>
                {

                    var datum = fixture.Create<Datum>();
                    var startDatum = new DateOnly(new Random().Next(minValue: 1970, DateTime.Now.Year),
                                                  datum.Value.Month,
                                                  Math.Min(datum.Value.Day, 28));
                    var document = new VerenigingZoekDocument();
                    document.VCode = fixture.Create<VCode>();
                    document.Locaties = fixture.CreateMany<Admin.Schema.Search.VerenigingZoekDocument.Locatie>().DistinctBy(l => l.Locatietype).ToArray();
                    document.Startdatum = startDatum.FormatAsBelgianDate();
                    document.Naam = fixture.Create<string>();
                    document.Doelgroep = fixture.Create<Admin.Schema.Search.Doelgroep>();
                    document.Lidmaatschappen = [];
                    document.HoofdactiviteitenVerenigingsloket = fixture.CreateMany<HoofdactiviteitVerenigingsloket>()
                                                                       .Select(x => new Admin.Schema.Search.VerenigingZoekDocument.HoofdactiviteitVerenigingsloket()
                                                                         {
                                                                             JsonLdMetadata = new JsonLdMetadata(
                                                                                 JsonLdType.Hoofdactiviteit.CreateWithIdValues(JsonLdType.Hoofdactiviteit.Type, x.Code),
                                                                                 JsonLdType.Hoofdactiviteit.Type),
                                                                             Code = x.Code,
                                                                             Naam = x.Naam,
                                                                         })
                                                                       .Distinct()
                                                                       .ToArray();

                    document.KorteNaam = fixture.Create<string>();
                    document.Sleutels = [];
                    document.Werkingsgebieden = withoutWerkingsgebieden
                        ? []
                        : fixture.CreateMany<Werkingsgebied>()
                                 .Distinct()
                                 .Select(x => new Admin.Schema.Search.VerenigingZoekDocument.Werkingsgebied()
                                  {
                                      JsonLdMetadata = new JsonLdMetadata(
                                          JsonLdType.Werkingsgebied.CreateWithIdValues(JsonLdType.Werkingsgebied.Type, x.Code),
                                          JsonLdType.Werkingsgebied.Type),
                                      Code = x.Code,
                                      Naam = x.Naam,
                                  }).ToArray();

                    return document;
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
                    AdresId = null,
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
                    Adres = fixture.Create<Adres>(),
                    AdresId = null,
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

    private static void CustomizeVoegLidmaatschapToeRequest(this IFixture fixture)
    {
        var date = fixture.Create<DateOnly>();

        fixture.Customize<VoegLidmaatschapToeRequest>(
            composer => composer.FromFactory(
                () => new VoegLidmaatschapToeRequest
                {
                    AndereVereniging = fixture.Create<VCode>(),
                    Van = date,
                    Tot = date.AddDays(new Random().Next(1, 99)),
                    Identificatie = fixture.Create<string>(),
                    Beschrijving = fixture.Create<string>(),
                }).OmitAutoProperties());
    }

    private static void CustomizeWijzigLidmaatschapRequest(this IFixture fixture)
    {
        var date = fixture.Create<DateOnly>();

        fixture.Customize<WijzigLidmaatschapRequest>(
            composer => composer.FromFactory(
                () => new WijzigLidmaatschapRequest
                {
                    Van = NullOrEmpty<DateOnly>.Create(date),
                    Tot = NullOrEmpty<DateOnly>.Create(date.AddDays(new Random().Next(1, 99))),
                    Identificatie = fixture.Create<string>(),
                    Beschrijving = fixture.Create<string>(),
                }).OmitAutoProperties());
    }

    private static void CustomizeLidmaatschapRegistratieData(this IFixture fixture)
    {
        var date = fixture.Create<DateOnly>();

        fixture.Customize<Registratiedata.Lidmaatschap>(
            composerTransformation: composer => composer.FromFactory(
                factory: () => new Registratiedata.Lidmaatschap(
                    fixture.Create<int>(),
                    fixture.Create<VCode>(),
                    fixture.Create<string>(),
                    date,
                    date.AddDays(value: new Random().Next(minValue: 1, maxValue: 99)),
                    fixture.Create<string>(), fixture.Create<string>()
                )).OmitAutoProperties());
    }
}
