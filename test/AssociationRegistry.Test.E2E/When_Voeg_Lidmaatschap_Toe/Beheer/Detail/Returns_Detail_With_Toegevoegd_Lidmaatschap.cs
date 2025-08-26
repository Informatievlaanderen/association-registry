namespace AssociationRegistry.Test.E2E.When_Voeg_Lidmaatschap_Toe.Beheer.Detail;

using Admin.Api.WebApi.Verenigingen.Detail.ResponseModels;
using AssociationRegistry.Formats;
using AssociationRegistry.Test.E2E.Framework.AlbaHost;
using AssociationRegistry.Test.E2E.Framework.ApiSetup;
using AssociationRegistry.Test.E2E.Framework.TestClasses;
using Contracts.JsonLdContext;
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

    public override async Task<DetailVerenigingResponse> GetResponse(FullBlownApiSetup setup)
        => await setup.AdminApiHost.GetBeheerDetail(setup.AdminHttpClient, _testContext.VCode,new RequestParameters().WithExpectedSequence(_testContext.CommandResult.Sequence));

    //await host.GetBeheerDetail(_testContext.AuthClient, _testContext.VCode, Headers.V2().WithExpectedSequence(42));

   // await host.GetBeheerZoeken(_testContext.AuthClient, "Foo", Headers.V2());

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
