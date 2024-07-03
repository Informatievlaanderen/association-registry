namespace AssociationRegistry.Test.Admin.Api.New.WhenWijzigLocatie.BeheerDetail;

using Alba;
using AssociationRegistry.Admin.Api.Verenigingen.Detail.ResponseModels;
using AssociationRegistry.Admin.Api.Verenigingen.Historiek.ResponseModels;
using AssociationRegistry.Admin.Schema;
using AssociationRegistry.Admin.Schema.Historiek.EventData;
using AssociationRegistry.Events;
using Historiek;
using JsonLdContext;
using KellermanSoftware.CompareNetObjects;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Vereniging;
using Vereniging.Bronnen;
using Xunit;
using Adres = AssociationRegistry.Admin.Api.Verenigingen.Detail.ResponseModels.Adres;
using AdresId = AssociationRegistry.Admin.Api.Verenigingen.Detail.ResponseModels.AdresId;
using Locatie = AssociationRegistry.Admin.Api.Verenigingen.Detail.ResponseModels.Locatie;

[Collection(nameof(WijzigLocatieContext))]
public class Returns_Detail_With_All_Fields : WijzigLocatieContext
{
    private readonly AppFixture _fixture;
    private readonly IAlbaHost theHost;

    public Returns_Detail_With_All_Fields(AppFixture fixture) : base(fixture)
    {
        _fixture = fixture;
        theHost = fixture.Host;
    }

    [Fact]
    public async Task JsonContentMatches()
    {
        var result = await theHost.GetAsJson<DetailVerenigingResponse>(url: $"/v1/verenigingen/{Scenario.VCode}");

        var comparisonConfig = new ComparisonConfig();
        comparisonConfig.MaxDifferences = 10;
        comparisonConfig.MaxMillisecondsDateDifference = (int)TimeSpan.FromSeconds(10).TotalMilliseconds;

        result.Vereniging.Locaties.Single(x => x.LocatieId == 1)
              .ShouldCompare(new Locatie()
               {
                   id = JsonLdType.Locatie.CreateWithIdValues(Scenario.VCode, "1"),
                   type = JsonLdType.Locatie.Type,
                   LocatieId = 1,
                   Naam = "Kantoor",
                   Adres = new Adres
                   {
                       id = JsonLdType.Adres.CreateWithIdValues(Scenario.VCode, "1"),
                       type = JsonLdType.Adres.Type,
                       Straatnaam = Request.Locatie.Adres.Straatnaam,
                       Huisnummer = "99",
                       Busnummer = "",
                       Postcode = "9200",
                       Gemeente = "Dendermonde",
                       Land = "België",
                   },
                   Adresvoorstelling = "Leopold II-laan 99, 9200 Dendermonde, België",
                   AdresId = new AdresId()
                   {
                       Broncode = Adresbron.AR,
                       Bronwaarde = "https://data.vlaanderen.be/id/adres/3213019",
                   },
                   VerwijstNaar = new AdresVerwijzing()
                   {
                       id = JsonLdType.AdresVerwijzing.CreateWithIdValues("3213019"),
                       type = JsonLdType.AdresVerwijzing.Type,
                   },
                   Bron = Bron.Initiator,
                   IsPrimair = true,
                   Locatietype = Locatietype.Correspondentie
               }, compareConfig: comparisonConfig);
    }
}

