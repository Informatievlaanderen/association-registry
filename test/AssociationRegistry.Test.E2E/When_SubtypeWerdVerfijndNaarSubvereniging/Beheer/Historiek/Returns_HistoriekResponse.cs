namespace AssociationRegistry.Test.E2E.When_SubtypeWerdVerfijndNaarSubvereniging.Beheer.Historiek;

using Admin.Api.WebApi.Verenigingen.Historiek.ResponseModels;
using Events;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.Comparison;
using Framework.Mappers;
using Framework.TestClasses;
using KellermanSoftware.CompareNetObjects;
using Xunit;

[Collection(nameof(VerfijnSubtypeNaarSubverenigingCollection))]
public class Returns_Detail : End2EndTest<HistoriekResponse>
{
    private readonly VerfijnSubtypeNaarSubverenigingContext _testContext;

    public Returns_Detail(VerfijnSubtypeNaarSubverenigingContext testContext) : base(testContext.ApiSetup)
    {
        _testContext = testContext;
    }

    public override async Task<HistoriekResponse> GetResponse(FullBlownApiSetup setup)
        => await setup.AdminApiHost.GetBeheerHistoriek(setup.SuperAdminHttpClient, _testContext.CommandResult.VCode, headers: new RequestParameters().WithExpectedSequence(
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
    public void With_All_Gebeurtenissen()
    {
        var werdVerfijnd =
            Response.Gebeurtenissen.SingleOrDefault(x => x.Gebeurtenis == nameof(VerenigingssubtypeWerdVerfijndNaarSubvereniging));

        werdVerfijnd.ShouldCompare(HistoriekGebeurtenisMapper.SubTypeWerdVerfijndNaarSubvereniging(new VerenigingssubtypeWerdVerfijndNaarSubvereniging(
                                                                                                       VCode: _testContext.VCode,
                                                                                                       new Registratiedata.SubverenigingVan(
                                                                                                           _testContext.Scenario.VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.VCode,
                                                                                                           _testContext.Scenario.VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.Naam,
                                                                                                           _testContext.CommandRequest.Identificatie ?? "",
                                                                                                           _testContext.CommandRequest.Beschrijving ?? ""
                                                                                                       ))),
                                        compareConfig: HistoriekComparisonConfig.Instance);
    }
}
