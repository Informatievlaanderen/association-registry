namespace AssociationRegistry.Test.E2E.VertegenwoordigerPersoonsgegevensTests.When_VertegenwoordigerPersoonsgegevens_Are_Removed_On_VZER.Beheer.Historiek;

using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Historiek.ResponseModels;
using AssociationRegistry.Events;
using AssociationRegistry.Test.E2E.Framework.AlbaHost;
using AssociationRegistry.Test.E2E.Framework.ApiSetup;
using AssociationRegistry.Test.E2E.Framework.Comparison;
using AssociationRegistry.Test.E2E.Framework.Mappers;
using AssociationRegistry.Test.E2E.Framework.TestClasses;
using KellermanSoftware.CompareNetObjects;
using Xunit;

[Collection(nameof(VertegenwoordigerZonderPersoonsgegevensTestCollection))]
public class Returns_Historiek : End2EndTest<HistoriekResponse>
{
    private readonly VertegenwoordigerZonderPersoonsgegevensOnVZERScenarioTestContext _testContext;

    public Returns_Historiek(VertegenwoordigerZonderPersoonsgegevensOnVZERScenarioTestContext testContext) : base(testContext.ApiSetup)
    {
        _testContext = testContext;
    }

    public override async Task<HistoriekResponse> GetResponse(FullBlownApiSetup setup)
        => await setup.AdminApiHost.GetBeheerHistoriek(setup.AdminHttpClient, _testContext.VCode, headers: new RequestParameters().WithExpectedSequence(_testContext.CommandResult.Sequence));

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
        var naamWerdGewijzigd = Response.Gebeurtenissen.SingleOrDefault(x => x.Gebeurtenis == nameof(NaamWerdGewijzigd));

        naamWerdGewijzigd.ShouldCompare(HistoriekGebeurtenisMapper.NaamWerdGewijzigd(_testContext.VCode, _testContext.CommandRequest.Naam),
                                        compareConfig: HistoriekComparisonConfig.Instance);
    }

    [Fact]
    public void Then_Gebeurtenissen_Persoonsgegevens_Are_Anonymised_For_VZERWerdGerigstreerd()
    {
        var werdGeregistreerd =
            Response.Gebeurtenissen.SingleOrDefault(x => x.Gebeurtenis == nameof(VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd));

        werdGeregistreerd.ShouldCompare(HistoriekGebeurtenisMapper.VerenigingWerdGeregistreerdZonderPersoonsGegevens(_testContext.Scenario.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdZonderPersoonsgegevens, _testContext.VCode),
                                        compareConfig: HistoriekComparisonConfig.Instance);
    }

    [Fact]
    public void Then_Gebeurtenissen_Persoonsgegevens_Are_Anonymised_For_VertegenwoordigerWerdToegevoegd()
    {
        // Only need to check the first added Vertegenwoordiger
       var vertegenwoordigerWerdToegevoegd =
            Response.Gebeurtenissen.FirstOrDefault(x => x.Gebeurtenis == nameof(VertegenwoordigerWerdToegevoegd));

        vertegenwoordigerWerdToegevoegd.ShouldCompare(HistoriekGebeurtenisMapper.VertegenwoordigerWerdToegevoegdZonderPersoonsgegevens(_testContext.Scenario.VertegenwoordigerWerdToegevoegdZonderPersoonsgegevens),
                                                      compareConfig: HistoriekComparisonConfig.Instance);
    }

    [Fact]
    public void Then_Gebeurtenissen_Persoonsgegevens_Are_Anonymised_For_VertegenwoordigerWerdVerwijderd()
    {
       var vertegenwoordigerWerdVerwijderd =
            Response.Gebeurtenissen.FirstOrDefault(x => x.Gebeurtenis == nameof(VertegenwoordigerWerdVerwijderd));

       vertegenwoordigerWerdVerwijderd.ShouldCompare(HistoriekGebeurtenisMapper.VertegenwoordigerWerdVerwijderdZonderPersoonsgegevens(_testContext.Scenario.VertegenwoordigerWerdVerwijderdZonderPersoonsgegevens),
                                                     compareConfig: HistoriekComparisonConfig.Instance);
    }

    [Fact]
    public void Then_Gebeurtenissen_Persoonsgegevens_Are_Anonymised_For_VertegenwoordigerWerdGewijzigd()
    {
        var vertegenwoordigerWerdGewijzigd =
            Response.Gebeurtenissen.FirstOrDefault(x => x.Gebeurtenis == nameof(VertegenwoordigerWerdGewijzigd));

        vertegenwoordigerWerdGewijzigd.ShouldCompare(HistoriekGebeurtenisMapper.VertegenwoordigerWerdGewijzigdZonderPersoonsgegevens(_testContext.Scenario.VertegenwoordigerWerdGewijzigdZonderPersoonsgegevens),
                                                     compareConfig: HistoriekComparisonConfig.Instance);
    }

    [Fact]
    public void Then_Gebeurtenissen_Persoonsgegevens_Are_Anonymised_For_KszSyncHeeftVertegenwoordigerAangeduidAlsOverleden()
    {
        var overledenEvent =
            Response.Gebeurtenissen.FirstOrDefault(x => x.Gebeurtenis == nameof(KszSyncHeeftVertegenwoordigerAangeduidAlsOverleden));

        overledenEvent.ShouldCompare(HistoriekGebeurtenisMapper.KszSyncHeeftVertegenwoordigerAangeduidAlsOverledenZonderPersoonsgegevens(_testContext.Scenario.KszSyncHeeftVertegenwoordigerAangeduidAlsOverledenZonderPersoonsgegevens),
                                     compareConfig: HistoriekComparisonConfig.Instance);
    }

    [Fact]
    public void Then_Gebeurtenissen_Persoonsgegevens_Are_Anonymised_For_KszSyncHeeftVertegenwoordigerAangeduidAlsNietGekend()
    {
        var nietGekendEvent =
            Response.Gebeurtenissen.FirstOrDefault(x => x.Gebeurtenis == nameof(KszSyncHeeftVertegenwoordigerAangeduidAlsNietGekend));

        nietGekendEvent.ShouldCompare(HistoriekGebeurtenisMapper.KszSyncHeeftVertegenwoordigerAangeduidAlsNietGekendZonderPersoonsgegevens(_testContext.Scenario.KszSyncHeeftVertegenwoordigerAangeduidAlsNietGekendZonderPersoonsgegevens),
                                      compareConfig: HistoriekComparisonConfig.Instance);
    }
}
