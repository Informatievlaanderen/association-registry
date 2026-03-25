namespace AssociationRegistry.Test.E2E.VertegenwoordigerPersoonsgegevensTests.
    When_VertegenwoordigerPersoonsgegevens_Are_Removed_On_FV.Beheer.Historiek;

using Admin.Api.WebApi.Verenigingen.Historiek.ResponseModels;
using Events;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.Comparison;
using Framework.Mappers;
using Framework.TestClasses;
using KellermanSoftware.CompareNetObjects;
using Xunit;

[Collection(nameof(VertegenwoordigerZonderPersoonsgegevensOnFVTestCollection))]
public class Returns_Historiek : End2EndTest<HistoriekResponse>
{
    private readonly VertegenwoordigerZonderPersoonsgegevensOnFVScenarioTestContext _testContext;

    public Returns_Historiek(VertegenwoordigerZonderPersoonsgegevensOnFVScenarioTestContext testContext) : base(
        testContext.ApiSetup)
    {
        _testContext = testContext;
    }

    public override async Task<HistoriekResponse> GetResponse(FullBlownApiSetup setup)
        => await setup.AdminApiHost.GetBeheerHistoriek(setup.AdminHttpClient,
                                                       _testContext.VCode,
                                                       headers: new RequestParameters().WithExpectedSequence(
                                                           _testContext.CommandResult.Sequence));

    [Fact]
    public void With_VCode()
    {
        Response.VCode.ShouldCompare(_testContext.VCode);
    }

    [Fact]
    public void With_Context()
    {
        Response.Context.ShouldCompare("http://127.0.0.1:11003/v1/contexten/beheer/historiek-vereniging-context.json");
    }

    [Fact]
    public void With_Naam_Werd_Gewijzigd_Gebeurtenis()
    {
        var naamWerdGewijzigd =
            Response.Gebeurtenissen.SingleOrDefault(x => x.Gebeurtenis == nameof(NaamWerdGewijzigd));

        naamWerdGewijzigd.ShouldCompare(
            HistoriekGebeurtenisMapper.NaamWerdGewijzigd(_testContext.VCode, _testContext.CommandRequest.Naam),
            compareConfig: HistoriekComparisonConfig.Instance);
    }

    [Fact]
    public void Then_Gebeurtenissen_Persoonsgegevens_Are_Anonymised_For_FeitelijkeVerenigingWerdGeregistreerd()
    {
        var werdGeregistreerd =
            Response.Gebeurtenissen.SingleOrDefault(x => x.Gebeurtenis ==
                                                         nameof(FeitelijkeVerenigingWerdGeregistreerd));

        werdGeregistreerd.ShouldCompare(
            HistoriekGebeurtenisMapper.FeitelijkeVerenigingWerdGeregistreerdZonderPersoonsgegevens(
                _testContext.Scenario.FeitelijkeVerenigingWerdGeregistreerdZonderPersoonsgegevens),
            compareConfig: HistoriekComparisonConfig.Instance);
    }

    [Fact]
    public void Then_Gebeurtenissen_Persoonsgegevens_Are_Anonymised_For_VertegenwoordigerWerdToegevoegd()
    {
        var werdGeregistreerd =
            Response.Gebeurtenissen.SingleOrDefault(x => x.Gebeurtenis == nameof(VertegenwoordigerWerdToegevoegd));

        werdGeregistreerd.ShouldCompare(
            HistoriekGebeurtenisMapper.VertegenwoordigerWerdToegevoegdZonderPersoonsgegevens(
                _testContext.Scenario.VertegenwoordigerWerdToegevoegdZonderPersoonsgegevens),
            compareConfig: HistoriekComparisonConfig.Instance);
    }

    [Fact]
    public void Then_Gebeurtenissen_Persoonsgegevens_Are_Anonymised_For_VertegenwoordigerWerdGewijzigd()
    {
        var werdGeregistreerd =
            Response.Gebeurtenissen.SingleOrDefault(x => x.Gebeurtenis == nameof(VertegenwoordigerWerdGewijzigd));

        werdGeregistreerd.ShouldCompare(
            HistoriekGebeurtenisMapper.VertegenwoordigerWerdGewijzigdZonderPersoonsgegevens(
                _testContext.Scenario.VertegenwoordigerWerdGewijzigdZonderPersoonsgegevens),
            compareConfig: HistoriekComparisonConfig.Instance);
    }

    [Fact]
    public void Then_Gebeurtenissen_Persoonsgegevens_Are_Anonymised_For_VertegenwoordigerWerdVerwijderd()
    {
        var werdGeregistreerd =
            Response.Gebeurtenissen.SingleOrDefault(x => x.Gebeurtenis == nameof(VertegenwoordigerWerdVerwijderd));

        werdGeregistreerd.ShouldCompare(
            HistoriekGebeurtenisMapper.VertegenwoordigerWerdVerwijderdZonderPersoonsgegevens(
                _testContext.Scenario.VertegenwoordigerWerdVerwijderdZonderPersoonsgegevens),
            compareConfig: HistoriekComparisonConfig.Instance);
    }
}
