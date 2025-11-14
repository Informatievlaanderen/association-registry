namespace AssociationRegistry.Test.E2E.When_Wijzig_Lidmaatschap.Publiek.Detail;

using Contracts.JsonLdContext;
using Framework.AlbaHost;
using Formats;
using Framework.ApiSetup;
using Framework.TestClasses;
using KellermanSoftware.CompareNetObjects;
using Microsoft.Extensions.Logging;
using Public.Api.WebApi.Verenigingen.Detail.ResponseModels;
using Xunit;
using Lidmaatschap = Public.Api.WebApi.Verenigingen.Detail.ResponseModels.Lidmaatschap;

[Collection(nameof(WijzigLidmaatschapCollection))]
public class Returns_Detail_With_Lidmaatschap : End2EndTest<PubliekVerenigingDetailResponse>
{
    private readonly WijzigLidmaatschapContext _testContext;

    public Returns_Detail_With_Lidmaatschap(WijzigLidmaatschapContext testContext) : base(testContext.ApiSetup)
    {
        _testContext = testContext;
        testContext.ApiSetup.Logger.LogInformation("STARTING TEST");
    }

    public override async Task<PubliekVerenigingDetailResponse> GetResponse(FullBlownApiSetup setup)
        => await setup.PublicApiHost.GetPubliekDetail(_testContext.VCode);

    [Fact]
    public void JsonContentMatches()
    {
        var comparisonConfig = new ComparisonConfig();
        comparisonConfig.MaxDifferences = 10;
        comparisonConfig.MaxMillisecondsDateDifference = (int)TimeSpan.FromSeconds(10).TotalMilliseconds;

        var expected = new Lidmaatschap
        {
            id = JsonLdType.Lidmaatschap.CreateWithIdValues(_testContext.VCode, _testContext.Scenario.LidmaatschapWerdToegevoegd.Lidmaatschap.LidmaatschapId.ToString()),
            type = JsonLdType.Lidmaatschap.Type,
            AndereVereniging = _testContext.Scenario.LidmaatschapWerdToegevoegd.Lidmaatschap.AndereVereniging,
            Beschrijving = _testContext.CommandRequest.Beschrijving,
            Van = _testContext.CommandRequest.Van.Value.FormatAsBelgianDate(),
            Tot = _testContext.CommandRequest.Tot.Value.FormatAsBelgianDate(),
            Identificatie = _testContext.CommandRequest.Identificatie,
            Naam = _testContext.Scenario.BaseScenario.AndereFeitelijkeVerenigingWerdGeristreerdMetPersoonsgegevens.Naam,
        };

        Response.Vereniging.Lidmaatschappen.Single(x => x.id == JsonLdType.Lidmaatschap.CreateWithIdValues(_testContext.VCode, _testContext.Scenario.LidmaatschapWerdToegevoegd.Lidmaatschap.LidmaatschapId.ToString()))
                .ShouldCompare(expected, compareConfig: comparisonConfig);
    }
}
