namespace AssociationRegistry.Test.E2E.VertegenwoordigerPersoonsgegevensTests.When_VertegenwoordigerPersoonsgegevens_Are_Removed_On_KBO.Beheer.Historiek;

using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Historiek.ResponseModels;
using AssociationRegistry.Events;
using AssociationRegistry.Test.E2E.Framework.AlbaHost;
using AssociationRegistry.Test.E2E.Framework.ApiSetup;
using AssociationRegistry.Test.E2E.Framework.Comparison;
using AssociationRegistry.Test.E2E.Framework.Mappers;
using AssociationRegistry.Test.E2E.Framework.TestClasses;
using KellermanSoftware.CompareNetObjects;
using Xunit;

[Collection(nameof(VertegenwoordigerZonderPersoonsgegevensOnKBOTestCollection))]
public class Returns_Historiek : End2EndTest<HistoriekResponse>
{
    private readonly VertegenwoordigerZonderPersoonsgegevensOnKBOTestContext _testContext;

    public Returns_Historiek(VertegenwoordigerZonderPersoonsgegevensOnKBOTestContext testContext) : base(testContext.ApiSetup)
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
        var roepnaamWerdGewijzigd =
            Response.Gebeurtenissen.SingleOrDefault(x => x.Gebeurtenis == nameof(RoepnaamWerdGewijzigd));

        roepnaamWerdGewijzigd.ShouldCompare(
            HistoriekGebeurtenisMapper.RoepnaamWerdGewijzigd(_testContext.CommandRequest.Roepnaam),
            compareConfig: HistoriekComparisonConfig.Instance);
    }

    [Fact]
    public void Then_Gebeurtenissen_Persoonsgegevens_Are_Anonymised_For_VertegenwoordigerWerdToegevoegdVanuitKBO()
    {
        var vertegenwoordigerWerdToegevoegd =
            Response.Gebeurtenissen.FirstOrDefault(x => x.Gebeurtenis == nameof(VertegenwoordigerWerdToegevoegdVanuitKBO));

        vertegenwoordigerWerdToegevoegd.ShouldCompare(HistoriekGebeurtenisMapper.VertegenwoordigerWerdToegevoegdZonderPersoonsgegevens(_testContext.Scenario.VertegenwoordigerWerdToegevoegdVanuitKBOZonderPersoonsgegevensToKeep),
                                                      compareConfig: HistoriekComparisonConfig.Instance);
    }

    [Fact]
    public void Then_Gebeurtenissen_Persoonsgegevens_Are_Anonymised_For_VertegenwoordigerWerdOvergenomenUitKBO()
    {
        var vertegenwoordigerWerdOvergenomen =
            Response.Gebeurtenissen.FirstOrDefault(x => x.Gebeurtenis == nameof(VertegenwoordigerWerdOvergenomenUitKBO));

        vertegenwoordigerWerdOvergenomen.ShouldCompare(HistoriekGebeurtenisMapper.VertegenwoordigerWerdOvergenomenUitKBOZonderPersoonsgegevens(_testContext.Scenario.VertegenwoordigerWerdOvergenomenUitKBOZonderPersoonsgegevensToChangeAndDelete),
                                                       compareConfig: HistoriekComparisonConfig.Instance);
    }

    [Fact]
    public void Then_Gebeurtenissen_Persoonsgegevens_Are_Anonymised_For_VertegenwoordigerWerdGewijzigdInKBO()
    {
        var vertegenwoordigerWerdGewijzigd =
            Response.Gebeurtenissen.FirstOrDefault(x => x.Gebeurtenis == nameof(VertegenwoordigerWerdGewijzigdInKBO));

        vertegenwoordigerWerdGewijzigd.ShouldCompare(HistoriekGebeurtenisMapper.VertegenwoordigerWerdGewijzigdInKBOZonderPersoonsgegevens(_testContext.Scenario.VertegenwoordigerWerdGewijzigdInKBOZonderPersoonsgegevens),
                                                       compareConfig: HistoriekComparisonConfig.Instance);
    }

    [Fact]
    public void Then_Gebeurtenissen_Persoonsgegevens_Are_Anonymised_For_VertegenwoordigerWerdVerwijderdUitKBO()
    {
        var vertegenwoordigerWerdVerwijderd =
            Response.Gebeurtenissen.FirstOrDefault(x => x.Gebeurtenis == nameof(VertegenwoordigerWerdVerwijderdUitKBO));

        vertegenwoordigerWerdVerwijderd.ShouldCompare(HistoriekGebeurtenisMapper.VertegenwoordigerWerdVerwijderdUitKBOZonderPersoonsgegevens(_testContext.Scenario.VertegenwoordigerWerdVerwijderdUitKBOZonderPersoonsgegevens),
                                                      compareConfig: HistoriekComparisonConfig.Instance);
    }
}
