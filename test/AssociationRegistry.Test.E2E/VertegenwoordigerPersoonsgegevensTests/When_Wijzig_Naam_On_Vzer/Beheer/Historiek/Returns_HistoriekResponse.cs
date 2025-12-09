namespace AssociationRegistry.Test.E2E.VertegenwoordigerPersoonsgegevensTests.When_Wijzig_Naam_On_Vzer.Beheer.Historiek;

using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Historiek.ResponseModels;
using Events;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.Comparison;
using Framework.Mappers;
using Framework.TestClasses;
using KellermanSoftware.CompareNetObjects;
using Xunit;

[Collection(nameof(WijzigNaamOnVZERTestCollection))]
public class Returns_Historiek : End2EndTest<HistoriekResponse>
{
    private readonly WijzigNaamOnVZERTestContext _testContext;

    public Returns_Historiek(WijzigNaamOnVZERTestContext testContext) : base(testContext.ApiSetup)
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

        var kszSyncHeeftVertegenwoordigerAangeduidAlsOverleden = Response.Gebeurtenissen.SingleOrDefault(x => x.Gebeurtenis == nameof(KszSyncHeeftVertegenwoordigerAangeduidAlsOverleden));

        kszSyncHeeftVertegenwoordigerAangeduidAlsOverleden.ShouldCompare(HistoriekGebeurtenisMapper.KszSyncHeeftVertegenwoordigerAangeduidAlsOverleden(_testContext.Scenario.KszSyncHeeftVertegenwoordigerAangeduidAlsOverleden),
                                                                         compareConfig: HistoriekComparisonConfig.Instance);

        var kszSyncHeeftVertegenwoordigerAangeduidAlsNietGekend = Response.Gebeurtenissen.SingleOrDefault(x => x.Gebeurtenis == nameof(KszSyncHeeftVertegenwoordigerAangeduidAlsNietGekend));

        kszSyncHeeftVertegenwoordigerAangeduidAlsNietGekend.ShouldCompare(HistoriekGebeurtenisMapper.KszSyncHeeftVertegenwoordigerAangeduidAlsNietGekend(_testContext.Scenario.KszSyncHeeftVertegenwoordigerAangeduidAlsNietGekend),
                                                                          compareConfig: HistoriekComparisonConfig.Instance);
    }
}
