namespace AssociationRegistry.Test.E2E.When_Wijzig_Lidmaatschap.Beheer.Detail;

using Admin.Api.Verenigingen.Detail.ResponseModels;
using Admin.Api.Verenigingen.Historiek.ResponseModels;
using Formats;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.TestClasses;
using JsonLdContext;
using KellermanSoftware.CompareNetObjects;
using Public.Api.Verenigingen.Detail.ResponseModels;
using Scenarios.Givens.FeitelijkeVereniging;
using Xunit;
using Lidmaatschap = Admin.Api.Verenigingen.Detail.ResponseModels.Lidmaatschap;

[Collection(nameof(WijzigLidmaatschapCollection))]
public class Returns_Detail_With_Lidmaatschap : End2EndTest<DetailVerenigingResponse>
{
    private readonly WijzigLidmaatschapContext _testContext;
    private readonly LidmaatschapWerdToegevoegdScenario _castedScenario;

    public Returns_Detail_With_Lidmaatschap(WijzigLidmaatschapContext testContext) : base(testContext.ApiSetup)
    {
        _testContext = testContext;
        _castedScenario = (LidmaatschapWerdToegevoegdScenario)testContext.Scenario;
    }

    public override DetailVerenigingResponse GetResponse(FullBlownApiSetup setup)
        => setup.AdminApiHost.GetBeheerDetail(_testContext.VCode);

    [Fact]
    public void JsonContentMatches()
    {
        var comparisonConfig = new ComparisonConfig();
        comparisonConfig.MaxDifferences = 10;
        comparisonConfig.MaxMillisecondsDateDifference = (int)TimeSpan.FromSeconds(10).TotalMilliseconds;

        var expected = new Lidmaatschap
        {
            id = JsonLdType.Lidmaatschap.CreateWithIdValues(_testContext.VCode, _castedScenario.LidmaatschapWerdToegevoegd.Lidmaatschap.LidmaatschapId.ToString()),
            type = JsonLdType.Lidmaatschap.Type,
            LidmaatschapId = _castedScenario.LidmaatschapWerdToegevoegd.Lidmaatschap.LidmaatschapId,
            AndereVereniging = _castedScenario.LidmaatschapWerdToegevoegd.Lidmaatschap.AndereVereniging,
            Beschrijving = _testContext.CommandRequest.Beschrijving,
            Van = _testContext.CommandRequest.Van.Value.FormatAsBelgianDate(),
            Tot = _testContext.CommandRequest.Tot.Value.FormatAsBelgianDate(),
            Identificatie = _testContext.CommandRequest.Identificatie,
            Naam = _castedScenario.BaseScenario.AndereFeitelijkeVerenigingWerdGeregistreerd.Naam,
        };

        Response.Vereniging.Lidmaatschappen.Single(x => x.LidmaatschapId == _castedScenario.LidmaatschapWerdToegevoegd.Lidmaatschap.LidmaatschapId)
                .ShouldCompare(expected, compareConfig: comparisonConfig);
    }
}
