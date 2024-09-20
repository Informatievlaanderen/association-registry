namespace AssociationRegistry.Test.E2E.When_Wijzig_Locatie.Beheer_Detail;

using Admin.Api.Verenigingen.Detail.ResponseModels;
using Admin.Api.Verenigingen.Locaties.FeitelijkeVereniging.WijzigLocatie.RequestModels;
using Alba;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.TestClasses;
using JsonLdContext;
using KellermanSoftware.CompareNetObjects;
using Vereniging;
using Vereniging.Bronnen;
using Xunit;
using Adres = Admin.Api.Verenigingen.Detail.ResponseModels.Adres;
using AdresId = Admin.Api.Verenigingen.Detail.ResponseModels.AdresId;
using Locatie = Admin.Api.Verenigingen.Detail.ResponseModels.Locatie;

[Collection(WijzigLocatieAdminCollection.Name)]
public class Returns_Detail_With_Gewijzigde_Locatie(WijzigLocatieContext<AdminApiSetup> context)
    : End2EndTest<WijzigLocatieContext<AdminApiSetup>, WijzigLocatieRequest, DetailVerenigingResponse>(context)
{
    protected override Func<IAlbaHost, DetailVerenigingResponse> GetResponse =>
        host => host.GetDetail(context.VCode);

    [Fact]
    public void JsonContentMatches()
    {
        var comparisonConfig = new ComparisonConfig();
        comparisonConfig.MaxDifferences = 10;
        comparisonConfig.MaxMillisecondsDateDifference = (int)TimeSpan.FromSeconds(10).TotalMilliseconds;

        Response.Vereniging.Locaties.Single(x => x.LocatieId == 1)
                .ShouldCompare(new Locatie
                 {
                     id = JsonLdType.Locatie.CreateWithIdValues(context.Scenario.VCode, "1"),
                     type = JsonLdType.Locatie.Type,
                     LocatieId = 1,
                     Naam = "Kantoor",
                     Adres = new Adres
                     {
                         id = JsonLdType.Adres.CreateWithIdValues(context.Scenario.VCode, "1"),
                         type = JsonLdType.Adres.Type,
                         Straatnaam = Request.Locatie.Adres.Straatnaam,
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
}
