namespace AssociationRegistry.Test.E2E.When_Voeg_Contactgegeven_Toe.Beheer.Historiek;

using Admin.Api.Verenigingen.Contactgegevens.FeitelijkeVereniging.VoegContactGegevenToe.RequestsModels;
using Admin.Api.Verenigingen.Historiek.ResponseModels;
using Admin.Api.Verenigingen.Lidmaatschap.VoegLidmaatschapToe.RequestModels;
using Events;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.Comparison;
using Framework.Mappers;
using Framework.TestClasses;
using KellermanSoftware.CompareNetObjects;
using Xunit;

[Collection(FullBlownApiCollection.Name)]
public class Returns_Historiek : End2EndTest<VoegContactgegevenToeContext, VoegContactgegevenToeRequest, HistoriekResponse>
{
    private readonly VoegContactgegevenToeContext _context;

    public override Func<IApiSetup, HistoriekResponse> GetResponse
        => setup => setup.AdminApiHost.GetBeheerHistoriek(TestContext.VCode);

    public Returns_Historiek(VoegContactgegevenToeContext testContext)
    {
        _context = testContext;
    }

    [Fact]
    public void With_VCode()
    {
        Response.VCode.ShouldCompare(TestContext.VCode);
    }

    [Fact]
    public void With_Context()
    {
        Response.Context.ShouldCompare("http://127.0.0.1:11003/v1/contexten/beheer/historiek-vereniging-context.json");
    }

    [Fact]
    public void With_LidmaatschapWerdToegevoegd_Gebeurtenissen()
    {
        var nextContactgegevenId = _context.Scenario.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd
                                           .Contactgegevens.Max(x => x.ContactgegevenId) + 1;
        var gebeurtenisResponse = Response.Gebeurtenissen.SingleOrDefault(x => x.Gebeurtenis == nameof(ContactgegevenWerdToegevoegd));
        gebeurtenisResponse.ShouldCompare(HistoriekGebeurtenisMapper.ContactgegevenWerdToegevoegd(nextContactgegevenId,TestContext.Request),
                                        compareConfig: HistoriekComparisonConfig.Instance);
    }
}
