namespace AssociationRegistry.Test.E2E.V2.When_Registreer_FeitelijkeVereniging_With_Potential_Duplicates.Beheer_Detail;

using AssociationRegistry.Admin.Api.Verenigingen.Common;
using Admin.Api.Verenigingen.Detail.ResponseModels;
using Admin.Api.Verenigingen.Registreer.FeitelijkeVereniging.RequetsModels;
using Admin.Schema.Constants;
using Formats;
using JsonLdContext;
using Framework.AlbaHost;
using Framework.Comparison;
using Vereniging;
using Vereniging.Bronnen;
using E2E.When_Registreer_FeitelijkeVereniging_With_Potential_Duplicates;
using KellermanSoftware.CompareNetObjects;
using NodaTime;
using Xunit;
using Contactgegeven = Admin.Api.Verenigingen.Detail.ResponseModels.Contactgegeven;
using HoofdactiviteitVerenigingsloket = Vereniging.HoofdactiviteitVerenigingsloket;
using Locatie = Admin.Api.Verenigingen.Detail.ResponseModels.Locatie;
using Vertegenwoordiger = Admin.Api.Verenigingen.Detail.ResponseModels.Vertegenwoordiger;

[Collection(FullBlownApiCollection.Name)]
public class Returns_DetailResponse : IClassFixture<RegistreerFeitelijkeVerenigingWithPotentialDuplicatesContext>, IAsyncLifetime
{
    private readonly RegistreerFeitelijkeVerenigingWithPotentialDuplicatesContext _context;

    public Returns_DetailResponse(RegistreerFeitelijkeVerenigingWithPotentialDuplicatesContext context)
    {
        _context = context;
    }

    [Fact]
    public void With_Context()
    {
        Response.Context.ShouldCompare("http://127.0.0.1:11003/v1/contexten/beheer/detail-vereniging-context.json");
    }

    [Fact]
    public void With_Metadata_DatumLaatsteAanpassing()
    {
        Response.Metadata.DatumLaatsteAanpassing.ShouldCompare(Instant.FromDateTimeOffset(DateTimeOffset.Now).FormatAsBelgianDate(),
                                                               compareConfig: new ComparisonConfig
                                                                   { MaxMillisecondsDateDifference = 5000 });
    }

    [Fact]
    public async Task WithFeitelijkeVereniging()
        => Response.Vereniging.ShouldCompare(new VerenigingDetail
        {
            Bron = Bron.Initiator,
            type = JsonLdType.FeitelijkeVereniging.Type,
            CorresponderendeVCodes = [],
            Doelgroep = new DoelgroepResponse
            {
                type = JsonLdType.Doelgroep.Type,
                id = JsonLdType.Doelgroep.CreateWithIdValues(_context.VCode),
                Minimumleeftijd = 1,
                Maximumleeftijd = 149,
            },
            VCode = _context.VCode,
            KorteBeschrijving = _context.Request.KorteBeschrijving,
            KorteNaam = _context.Request.KorteNaam,
            Verenigingstype = new VerenigingsType
            {
                Code = Verenigingstype.FeitelijkeVereniging.Code,
                Naam = Verenigingstype.FeitelijkeVereniging.Naam,
            },
            Naam = _context.Request.Naam,
            Startdatum = Instant.FromDateTimeOffset(DateTimeOffset.UtcNow).ConvertAndFormatToBelgianDate(),
            Einddatum = null,
            Status = VerenigingStatus.Actief,
            IsUitgeschrevenUitPubliekeDatastroom = _context.Request.IsUitgeschrevenUitPubliekeDatastroom,
            Contactgegevens = MapLocaties(_context.Request.Contactgegevens, _context.VCode),
            HoofdactiviteitenVerenigingsloket = MapHoofdactiviteitenVerenigingsloket(_context.Request.HoofdactiviteitenVerenigingsloket),
            Locaties = MapLocaties(_context.Request.Locaties, _context.VCode),
            Vertegenwoordigers = MapVertegenwoordigers(_context.Request.Vertegenwoordigers, _context.VCode),
            Relaties = MapRelaties([], _context.VCode),
            Sleutels = MapSleutels(_context.Request, _context.VCode),
        }, compareConfig: AdminDetailComparisonConfig.Instance);

    private static Sleutel[] MapSleutels(RegistreerFeitelijkeVerenigingRequest request, string vCode)
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

    private static Vertegenwoordiger[] MapVertegenwoordigers(ToeTeVoegenVertegenwoordiger[] vertegenwoordigers, string vCode)
    {
        return vertegenwoordigers.Select((x, i) => new Vertegenwoordiger
        {
            id = JsonLdType.Vertegenwoordiger.CreateWithIdValues(
                vCode, $"{i + 1}"),
            type = JsonLdType.Vertegenwoordiger.Type,
            VertegenwoordigerId = i + 1,
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

    private static Contactgegeven[] MapLocaties(ToeTeVoegenContactgegeven[] toeTeVoegenContactgegevens, string vCode)
    {
        return toeTeVoegenContactgegevens.Select((x, i) => new Contactgegeven
        {
            id = JsonLdType.Contactgegeven.CreateWithIdValues(
                vCode, $"{i + 1}"),
            type = JsonLdType.Contactgegeven.Type,
            ContactgegevenId = i + 1,
            Contactgegeventype = x.Contactgegeventype,
            Waarde = x.Waarde,
            Beschrijving = x.Beschrijving!,
            IsPrimair = x.IsPrimair,
            Bron = Bron.Initiator,
        }).ToArray();
    }

    private static Locatie[] MapLocaties(ToeTeVoegenLocatie[] toeTeVoegenLocaties, string vCode)
    {
        return toeTeVoegenLocaties.Select((x, i) => new Locatie
        {
            id = JsonLdType.Locatie.CreateWithIdValues(
                vCode, $"{i + 1}"),
            type = JsonLdType.Locatie.Type,
            LocatieId = i + 1,
            Locatietype = x.Locatietype,
            Naam = x.Naam,
            Bron = Bron.Initiator,
            IsPrimair = x.IsPrimair,
        }).ToArray();
    }

    private static Admin.Api.Verenigingen.Detail.ResponseModels.HoofdactiviteitVerenigingsloket[] MapHoofdactiviteitenVerenigingsloket(
        string[] hoofdactiviteitenVerenigingsloket)
    {
        return hoofdactiviteitenVerenigingsloket.Select(x =>
        {
            var hoofdactiviteitVerenigingsloket = HoofdactiviteitVerenigingsloket.Create(x);

            return new Admin.Api.Verenigingen.Detail.ResponseModels.HoofdactiviteitVerenigingsloket
            {
                Code = hoofdactiviteitVerenigingsloket.Code,
                Naam = hoofdactiviteitVerenigingsloket.Naam,
                id = JsonLdType.Hoofdactiviteit.CreateWithIdValues(hoofdactiviteitVerenigingsloket.Code),
                type = JsonLdType.Hoofdactiviteit.Type,
            };
        }).ToArray();
    }

    public async Task InitializeAsync()
    {
        Response = _context.ApiSetup.AdminApiHost.GetDetail(_context.VCode);
    }

    public DetailVerenigingResponse Response { get; set; }

    public async Task DisposeAsync()
    {
    }

}
