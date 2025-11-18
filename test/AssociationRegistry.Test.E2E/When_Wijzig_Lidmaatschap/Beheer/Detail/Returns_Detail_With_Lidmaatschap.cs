namespace AssociationRegistry.Test.E2E.When_Wijzig_Lidmaatschap.Beheer.Detail;

using Admin.Api.WebApi.Verenigingen.Detail.ResponseModels;
using Contracts.JsonLdContext;
using Formats;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.TestClasses;
using KellermanSoftware.CompareNetObjects;
using Xunit;
using Lidmaatschap = Admin.Api.WebApi.Verenigingen.Detail.ResponseModels.Lidmaatschap;

[Collection(nameof(WijzigLidmaatschapCollection))]
public class Returns_Detail_With_Lidmaatschap : End2EndTest<DetailVerenigingResponse>
{
    private readonly WijzigLidmaatschapContext _testContext;

    public Returns_Detail_With_Lidmaatschap(WijzigLidmaatschapContext testContext) : base(testContext.ApiSetup)
    {
        _testContext = testContext;
    }

    public override async Task<DetailVerenigingResponse> GetResponse(FullBlownApiSetup setup)
        => await setup.AdminApiHost.GetBeheerDetail(setup.AdminHttpClient, _testContext.VCode,new RequestParameters().WithExpectedSequence(_testContext.CommandResult.Sequence));

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
            LidmaatschapId = _testContext.Scenario.LidmaatschapWerdToegevoegd.Lidmaatschap.LidmaatschapId,
            AndereVereniging = _testContext.Scenario.LidmaatschapWerdToegevoegd.Lidmaatschap.AndereVereniging,
            Beschrijving = _testContext.CommandRequest.Beschrijving,
            Van = _testContext.CommandRequest.Van.Value.FormatAsBelgianDate(),
            Tot = _testContext.CommandRequest.Tot.Value.FormatAsBelgianDate(),
            Identificatie = _testContext.CommandRequest.Identificatie,
            Naam = _testContext.Scenario.BaseScenario.AndereFeitelijkeVerenigingWerdGeregistreerd.Naam,
        };

        Response.Vereniging.Lidmaatschappen.Single(x => x.LidmaatschapId == _testContext.Scenario.LidmaatschapWerdToegevoegd.Lidmaatschap.LidmaatschapId)
                .ShouldCompare(expected, compareConfig: comparisonConfig);
    }
}
