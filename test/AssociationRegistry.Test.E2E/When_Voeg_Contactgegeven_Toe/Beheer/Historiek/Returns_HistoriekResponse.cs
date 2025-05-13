namespace AssociationRegistry.Test.E2E.When_Voeg_Contactgegeven_Toe.Beheer.Historiek;

using Admin.Api.Verenigingen.Historiek.ResponseModels;
using Events;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.Comparison;
using Framework.Mappers;
using Framework.TestClasses;
using KellermanSoftware.CompareNetObjects;
using Xunit;

[Collection(nameof(VoegContactgegevenToeCollection))]
public class Returns_Detail_With_Lidmaatschap : End2EndTest<HistoriekResponse>
{
    private readonly VoegContactgegevenToeContext _testContext;

    public Returns_Detail_With_Lidmaatschap(VoegContactgegevenToeContext testContext) : base(testContext.ApiSetup)
    {
        _testContext = testContext;
    }

    public override HistoriekResponse GetResponse(FullBlownApiSetup setup)
        => setup.AdminApiHost.GetBeheerHistoriek(_testContext.VCode);

    [Fact]
    public void With_Context()
    {
        Response.Context.ShouldCompare("http://127.0.0.1:11003/v1/contexten/beheer/historiek-vereniging-context.json");
    }

    [Fact]
    public void With_LidmaatschapWerdToegevoegd_Gebeurtenissen()
    {
        var nextContactgegevenId = _testContext.Scenario.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd
                                           .Contactgegevens.Max(x => x.ContactgegevenId) + 1;
        var gebeurtenisResponse = Response.Gebeurtenissen.SingleOrDefault(x => x.Gebeurtenis == nameof(ContactgegevenWerdToegevoegd));
        gebeurtenisResponse.ShouldCompare(HistoriekGebeurtenisMapper.ContactgegevenWerdToegevoegd(nextContactgegevenId,_testContext.CommandRequest),
                                        compareConfig: HistoriekComparisonConfig.Instance);
    }
}
