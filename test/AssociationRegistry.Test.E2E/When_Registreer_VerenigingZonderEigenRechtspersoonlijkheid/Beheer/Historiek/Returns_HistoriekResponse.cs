namespace AssociationRegistry.Test.E2E.When_Registreer_VerenigingZonderEigenRechtspersoonlijkheid.Beheer.Historiek;

using Admin.Api.Verenigingen.Historiek.ResponseModels;
using Events;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.Comparison;
using Framework.Mappers;
using Framework.TestClasses;
using KellermanSoftware.CompareNetObjects;
using Xunit;

[Collection(nameof(RegistreerVerenigingZonderEigenRechtspersoonlijkheidCollection))]
public class Returns_HistoriekResponse : End2EndTest<HistoriekResponse>
{
    private readonly RegistreerVerenigingZonderEigenRechtspersoonlijkheidContext _testContext;

    public Returns_HistoriekResponse(RegistreerVerenigingZonderEigenRechtspersoonlijkheidContext testContext) : base(testContext.ApiSetup)
    {
        _testContext = testContext;
    }

    public override HistoriekResponse GetResponse(FullBlownApiSetup setup)
        => setup.AdminApiHost.GetBeheerHistoriek(setup.AdminHttpClient, _testContext.VCode, headers: new RequestParameters().WithExpectedSequence(_testContext.CommandResult.Sequence)).GetAwaiter().GetResult();

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
        var werdGeregistreerd =
            Response.Gebeurtenissen.SingleOrDefault(x => x.Gebeurtenis == nameof(VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd));

        werdGeregistreerd.ShouldCompare(HistoriekGebeurtenisMapper.VerenigingWerdGeregistreerd(_testContext.CommandRequest, _testContext.VCode),
                                        compareConfig: HistoriekComparisonConfig.Instance);
    }
}
