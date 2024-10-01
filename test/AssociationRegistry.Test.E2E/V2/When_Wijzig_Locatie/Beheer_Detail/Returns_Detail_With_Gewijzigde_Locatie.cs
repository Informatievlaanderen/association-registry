namespace AssociationRegistry.Test.E2E.V2.When_Wijzig_Locatie.Beheer_Detail;

using AssociationRegistry.Admin.Api.Verenigingen.Detail.ResponseModels;
using AssociationRegistry.Test.E2E.Data;
using AssociationRegistry.Test.E2E.Framework.AlbaHost;
using AssociationRegistry.Test.E2E.V2.When_Wijzig_Locatie;
using Azure.Core;
using FluentAssertions;
using JsonLdContext;
using KellermanSoftware.CompareNetObjects;
using Scenarios;
using Vereniging;
using Vereniging.Bronnen;
using Xunit;
using Adres = Admin.Api.Verenigingen.Detail.ResponseModels.Adres;
using AdresId = Admin.Api.Verenigingen.Detail.ResponseModels.AdresId;
using Locatie = Admin.Api.Verenigingen.Detail.ResponseModels.Locatie;

[Collection(RegistreerVerenigingAdminCollection2.Name)]
public class Returns_Detail_With_Gewijzigde_Locatie : IClassFixture<WijzigLocatieContext>, IAsyncLifetime
{
    private readonly WijzigLocatieContext _context;

    public Returns_Detail_With_Gewijzigde_Locatie(WijzigLocatieContext context)
    {
        _context = context;
    }

    [Fact]
    public void JsonContentMatches()
    {
        var comparisonConfig = new ComparisonConfig();
        comparisonConfig.MaxDifferences = 10;
        comparisonConfig.MaxMillisecondsDateDifference = (int)TimeSpan.FromSeconds(10).TotalMilliseconds;

        Response.Vereniging.Locaties.Single(x => x.LocatieId == 1)
                .ShouldCompare(new Locatie
                 {
                     id = JsonLdType.Locatie.CreateWithIdValues(_context.VCode, "1"),
                     type = JsonLdType.Locatie.Type,
                     LocatieId = 1,
                     Naam = "Kantoor",
                     Adres = new Adres
                     {
                         id = JsonLdType.Adres.CreateWithIdValues(_context.VCode, "1"),
                         type = JsonLdType.Adres.Type,
                         Straatnaam = _context.Request.Locatie.Adres.Straatnaam,
                         Huisnummer = "99",
                         Busnummer = "",
                         Postcode = "9200",
                         Gemeente = "Dendermonde",
                         Land = "België",
                     },
                     Adresvoorstelling = "Leopold II-laan 99, 9200 Dendermonde, België",
                     AdresId = new AdresId
                     {
                         Broncode = Adresbron.AR,
                         Bronwaarde = "https://data.vlaanderen.be/id/adres/3213019",
                     },
                     VerwijstNaar = new AdresVerwijzing
                     {
                         id = JsonLdType.AdresVerwijzing.CreateWithIdValues("3213019"),
                         type = JsonLdType.AdresVerwijzing.Type,
                     },
                     Bron = Bron.Initiator,
                     IsPrimair = true,
                     Locatietype = Locatietype.Correspondentie,
                 }, compareConfig: comparisonConfig);
    }

    public DetailVerenigingResponse Response { get; set; }

    public async Task InitializeAsync()
    {
        Response = _context.ApiSetup.AdminApiHost.GetDetail(_context.VCode);
    }

    public async Task DisposeAsync()
    {
    }
}
