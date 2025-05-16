namespace AssociationRegistry.Test.E2E.When_Voeg_Lidmaatschap_Toe.Beheer.Detail;

using AssociationRegistry.Admin.Api.Verenigingen.Detail.ResponseModels;
using AssociationRegistry.Formats;
using AssociationRegistry.JsonLdContext;
using AssociationRegistry.Test.E2E.Framework.AlbaHost;
using AssociationRegistry.Test.E2E.Framework.ApiSetup;
using AssociationRegistry.Test.E2E.Framework.TestClasses;
using KellermanSoftware.CompareNetObjects;
using Xunit;

[Collection(nameof(VoegLidmaatschapToeCollection))]
public class Returns_Detail_With_Toegevoegd_Lidmaatschap : End2EndTest<DetailVerenigingResponse>
{
    private readonly VoegLidmaatschapToeContext _testContext;

    public Returns_Detail_With_Toegevoegd_Lidmaatschap(VoegLidmaatschapToeContext testContext)
        : base(testContext.ApiSetup)
    {
        _testContext = testContext;
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
            id = JsonLdType.Lidmaatschap.CreateWithIdValues(_testContext.VCode, "1"),
            type = JsonLdType.Lidmaatschap.Type,
            LidmaatschapId = 1,
            AndereVereniging = _testContext.CommandRequest.AndereVereniging,
            Beschrijving = _testContext.CommandRequest.Beschrijving,
            Van = _testContext.CommandRequest.Van.FormatAsBelgianDate(),
            Tot = _testContext.CommandRequest.Tot.FormatAsBelgianDate(),
            Identificatie = _testContext.CommandRequest.Identificatie,
            Naam = _testContext.Scenario.AndereFeitelijkeVerenigingWerdGeregistreerd.Naam,
        };

        Response.Vereniging.Lidmaatschappen.Single(x => x.LidmaatschapId == 1)
                .ShouldCompare(expected, compareConfig: comparisonConfig);
    }
}
