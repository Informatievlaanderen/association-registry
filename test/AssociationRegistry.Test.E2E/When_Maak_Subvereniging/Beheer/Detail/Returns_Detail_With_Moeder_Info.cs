namespace AssociationRegistry.Test.E2E.When_Maak_Subvereniging.Beheer.Detail;

using Admin.Api.Verenigingen.Detail.ResponseModels;
using Admin.ProjectionHost.Infrastructure.Extensions;
using Admin.Schema.Constants;
using Events;
using Formats;
using Framework.AlbaHost;
using Framework.Comparison;
using JsonLdContext;
using KellermanSoftware.CompareNetObjects;
using NodaTime;
using Vereniging;
using Vereniging.Bronnen;
using Xunit;
using Contactgegeven = Admin.Api.Verenigingen.Detail.ResponseModels.Contactgegeven;
using HoofdactiviteitVerenigingsloket = Vereniging.HoofdactiviteitVerenigingsloket;
using Locatie = Admin.Api.Verenigingen.Detail.ResponseModels.Locatie;
using Vertegenwoordiger = Admin.Api.Verenigingen.Detail.ResponseModels.Vertegenwoordiger;
using Werkingsgebied = Vereniging.Werkingsgebied;

[Collection(FullBlownApiCollection.Name)]
public class Returns_Detail_With_Moeder_Info : IClassFixture<MaakSubverenigingContext>, IAsyncLifetime
{
    private readonly MaakSubverenigingContext _context;

    public Returns_Detail_With_Moeder_Info(MaakSubverenigingContext context)
    {
        _context = context;
    }

    [Fact]
    public void JsonContentMatches()
    {
        var comparisonConfig = new ComparisonConfig();
        comparisonConfig.MaxDifferences = 10;
        comparisonConfig.MaxMillisecondsDateDifference = (int)TimeSpan.FromSeconds(10).TotalMilliseconds;
    }

    [Fact(Skip = "Remove after implementation")]
    public void WithSubvereniging()
    {
        var expected = new VerenigingDetail
        {
            Bron = Bron.Initiator,
            type = JsonLdType.Subvereniging.Type,
            CorresponderendeVCodes = [],
            Doelgroep = new DoelgroepResponse
            {
                type = JsonLdType.Doelgroep.Type,
                id = JsonLdType.Doelgroep.CreateWithIdValues(_context.Scenario.FeitelijkeVerenigingWerdGeregistreerd.VCode),
                Minimumleeftijd = _context.Scenario.FeitelijkeVerenigingWerdGeregistreerd.Doelgroep.Minimumleeftijd,
                Maximumleeftijd = _context.Scenario.FeitelijkeVerenigingWerdGeregistreerd.Doelgroep.Maximumleeftijd,
            },
            VCode = _context.Scenario.FeitelijkeVerenigingWerdGeregistreerd.VCode,
            KorteBeschrijving = _context.Scenario.FeitelijkeVerenigingWerdGeregistreerd.KorteBeschrijving,
            KorteNaam = _context.Scenario.FeitelijkeVerenigingWerdGeregistreerd.KorteNaam,
            Verenigingstype = new VerenigingsType
            {
                Code = Verenigingstype.Subvereniging.Code,
                Naam = Verenigingstype.Subvereniging.Naam,
            },
            Naam = _context.Scenario.FeitelijkeVerenigingWerdGeregistreerd.Naam,
            Startdatum = _context.Scenario.FeitelijkeVerenigingWerdGeregistreerd.Startdatum.ToBelgianDate(),
            Einddatum = null,
            Status = VerenigingStatus.Actief,
            IsUitgeschrevenUitPubliekeDatastroom =
                _context.Scenario.FeitelijkeVerenigingWerdGeregistreerd.IsUitgeschrevenUitPubliekeDatastroom,
            Contactgegevens = MapContactgegevens(_context.Scenario.FeitelijkeVerenigingWerdGeregistreerd.Contactgegevens,
                                                 _context.Scenario.FeitelijkeVerenigingWerdGeregistreerd.VCode),
            HoofdactiviteitenVerenigingsloket =
                MapHoofdactiviteitenVerenigingsloket(_context.Scenario.FeitelijkeVerenigingWerdGeregistreerd
                                                             .HoofdactiviteitenVerenigingsloket),
            Werkingsgebieden = MapWerkingsgebieden(_context.Scenario.FeitelijkeVerenigingWerdGeregistreerd.Werkingsgebieden),
            Locaties = MapLocaties(_context.Scenario.FeitelijkeVerenigingWerdGeregistreerd.Locaties,
                                   _context.Scenario.FeitelijkeVerenigingWerdGeregistreerd.VCode),
            Vertegenwoordigers = MapVertegenwoordigers(_context.Scenario.FeitelijkeVerenigingWerdGeregistreerd.Vertegenwoordigers,
                                                       _context.Scenario.FeitelijkeVerenigingWerdGeregistreerd.VCode),
            Relaties = MapRelaties([], _context.Scenario.FeitelijkeVerenigingWerdGeregistreerd.VCode),
            Lidmaatschappen = [],
            Sleutels = MapSleutels(_context.Scenario.FeitelijkeVerenigingWerdGeregistreerd.VCode),
        };

        Response.Vereniging.ShouldCompare(expected, compareConfig: AdminDetailComparisonConfig.Instance);
    }

    private static Sleutel[] MapSleutels(string vCode)
        =>
        [
            new Sleutel
            {
                Bron = Sleutelbron.VR.Waarde,
                id = JsonLdType.Sleutel.CreateWithIdValues(vCode, Sleutelbron.VR.Waarde),
                type = JsonLdType.Sleutel.Type,
                Waarde = vCode,
                CodeerSysteem = CodeerSysteem.VR,
                GestructureerdeIdentificator = new GestructureerdeIdentificator
                {
                    id = JsonLdType.GestructureerdeSleutel.CreateWithIdValues(vCode, Sleutelbron.VR.Waarde),
                    type = JsonLdType.GestructureerdeSleutel.Type,
                    Nummer = vCode,
                },
            },
        ];

    private static Relatie[] MapRelaties(Relatie[] relaties, string vCode)
    {
        return relaties.Select((x, i) => new Relatie
        {
            AndereVereniging = x.AndereVereniging,
            Relatietype = x.Relatietype,
        }).ToArray();
    }

    private static Vertegenwoordiger[] MapVertegenwoordigers(Registratiedata.Vertegenwoordiger[] vertegenwoordigers, string vCode)
    {
        return vertegenwoordigers.Select((x, i) => new Vertegenwoordiger
        {
            id = JsonLdType.Vertegenwoordiger.CreateWithIdValues(
                vCode, x.VertegenwoordigerId.ToString()),
            type = JsonLdType.Vertegenwoordiger.Type,
            VertegenwoordigerId = x.VertegenwoordigerId,
            PrimairContactpersoon = x.IsPrimair,
            Achternaam = x.Achternaam,
            Email = x.Email,
            Insz = x.Insz,
            Voornaam = x.Voornaam,
            Roepnaam = x.Roepnaam,
            Rol = x.Rol,
            Telefoon = x.Telefoon,
            Mobiel = x.Mobiel,
            SocialMedia = x.SocialMedia,
            VertegenwoordigerContactgegevens = new VertegenwoordigerContactgegevens
            {
                id = JsonLdType.VertegenwoordigerContactgegeven.CreateWithIdValues(
                    vCode, $"{i + 1}"),
                type = JsonLdType.VertegenwoordigerContactgegeven.Type,
                IsPrimair = x.IsPrimair,
                Email = x.Email,
                Telefoon = x.Telefoon,
                Mobiel = x.Mobiel,
                SocialMedia = x.SocialMedia,
            },
            Bron = Bron.Initiator,
        }).ToArray();
    }

    private static Contactgegeven[] MapContactgegevens(Registratiedata.Contactgegeven[] toeTeVoegenContactgegevens, string vCode)
    {
        return toeTeVoegenContactgegevens.Select((x, i) => new Contactgegeven
        {
            id = JsonLdType.Contactgegeven.CreateWithIdValues(vCode, x.ContactgegevenId.ToString()),
            type = JsonLdType.Contactgegeven.Type,
            ContactgegevenId = x.ContactgegevenId,
            Contactgegeventype = x.Contactgegeventype,
            Waarde = x.Waarde,
            Beschrijving = x.Beschrijving!,
            IsPrimair = x.IsPrimair,
            Bron = Bron.Initiator,
        }).ToArray();
    }

    private static Locatie[] MapLocaties(Registratiedata.Locatie[] toeTeVoegenLocaties, string vCode)
    {
        return toeTeVoegenLocaties.Select((x, i) => new Locatie
        {
            id = JsonLdType.Locatie.CreateWithIdValues(
                vCode, x.LocatieId.ToString()),
            type = JsonLdType.Locatie.Type,
            LocatieId = x.LocatieId,
            Locatietype = x.Locatietype,
            Naam = x.Naam,
            Bron = Bron.Initiator,
            IsPrimair = x.IsPrimair,
        }).ToArray();
    }

    private static Admin.Api.Verenigingen.Detail.ResponseModels.HoofdactiviteitVerenigingsloket[] MapHoofdactiviteitenVerenigingsloket(
        Registratiedata.HoofdactiviteitVerenigingsloket[] hoofdactiviteitenVerenigingsloket)
    {
        return hoofdactiviteitenVerenigingsloket.Select(x =>
        {
            var hoofdactiviteitVerenigingsloket = HoofdactiviteitVerenigingsloket.Create(x.Code);

            return new Admin.Api.Verenigingen.Detail.ResponseModels.HoofdactiviteitVerenigingsloket
            {
                Code = hoofdactiviteitVerenigingsloket.Code,
                Naam = hoofdactiviteitVerenigingsloket.Naam,
                id = JsonLdType.Hoofdactiviteit.CreateWithIdValues(hoofdactiviteitVerenigingsloket.Code),
                type = JsonLdType.Hoofdactiviteit.Type,
            };
        }).ToArray();
    }

    private static Admin.Api.Verenigingen.Detail.ResponseModels.Werkingsgebied[] MapWerkingsgebieden(
        Registratiedata.Werkingsgebied[] werkingsgebieden)
    {
        return werkingsgebieden.Select(x =>
        {
            var werkingsgebied = Werkingsgebied.Create(x.Code);

            return new Admin.Api.Verenigingen.Detail.ResponseModels.Werkingsgebied
            {
                Code = werkingsgebied.Code,
                Naam = werkingsgebied.Naam,
                id = JsonLdType.Werkingsgebied.CreateWithIdValues(werkingsgebied.Code),
                type = JsonLdType.Werkingsgebied.Type,
            };
        }).ToArray();
    }

    public DetailVerenigingResponse Response { get; set; }

    public async Task InitializeAsync()
    {
        Response = _context.ApiSetup.AdminApiHost.GetBeheerDetail(_context.VCode);
    }

    public async Task DisposeAsync()
    {
    }
}
