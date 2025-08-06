namespace AssociationRegistry.Test.E2E.When_Wijzig_Locatie.Beheer.Detail;

using Admin.Api.WebApi.Verenigingen.Detail.ResponseModels;
using DecentraalBeheer.Vereniging;
using DecentraalBeheer.Vereniging.Adressen;
using JsonLdContext;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.TestClasses;
using Vereniging;
using Vereniging.Bronnen;
using KellermanSoftware.CompareNetObjects;
using Xunit;
using Adres = Admin.Api.WebApi.Verenigingen.Detail.ResponseModels.Adres;
using AdresId = Admin.Api.WebApi.Verenigingen.Detail.ResponseModels.AdresId;
using Locatie = Admin.Api.WebApi.Verenigingen.Detail.ResponseModels.Locatie;

[Collection(nameof(WijzigLocatieCollection))]
public class Returns_Detail_With_Gewijzigde_Locatie : End2EndTest<DetailVerenigingResponse>
{
    private readonly WijzigLocatieContext _testContext;

    public Returns_Detail_With_Gewijzigde_Locatie(WijzigLocatieContext testContext) : base(testContext.ApiSetup)
    {
        _testContext = testContext;
    }

    [Fact]
    public void JsonContentMatches()
    {
        var comparisonConfig = new ComparisonConfig();
        comparisonConfig.MaxDifferences = 10;
        comparisonConfig.MaxMillisecondsDateDifference = (int)TimeSpan.FromSeconds(10).TotalMilliseconds;

        var expected = new Locatie
        {
            id = JsonLdType.Locatie.CreateWithIdValues(_testContext.VCode, "1"),
            type = JsonLdType.Locatie.Type,
            LocatieId = 1,
            Naam = "Kantoor",
            Adres = new Adres
            {
                id = JsonLdType.Adres.CreateWithIdValues(_testContext.VCode, "1"),
                type = JsonLdType.Adres.Type,
                Straatnaam = _testContext.CommandRequest.Locatie.Adres.Straatnaam,
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
        };

        Response.Vereniging.Locaties.Single(x => x.LocatieId == 1)
                .ShouldCompare(expected, compareConfig: comparisonConfig);
    }

    public override DetailVerenigingResponse GetResponse(FullBlownApiSetup setup)
        => _testContext.ApiSetup.AdminApiHost.GetBeheerDetail(
                            setup.AdminHttpClient,
                            _testContext.VCode,
                            headers: new RequestParameters().WithExpectedSequence(_testContext.CommandResult.Sequence))
                       .GetAwaiter().GetResult();
}
