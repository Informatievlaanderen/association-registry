namespace AssociationRegistry.Test.E2E.When_Weiger_Relatieaanvraag.Beheer;

using Admin.Api.WebApi.Relatieaanvragen.Opvragen.ResponseModels;
using DecentraalBeheer.Relatieaanvragen;
using FluentAssertions;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.TestClasses;
using Scenarios.Requests.Relatieaanvragen;
using Xunit;

[Collection(nameof(WeigerRelatieaanvraagCollection))]
public class Returns_Geweigerde_Relatieaanvraag : End2EndTest<RelatieaanvraagResponse>
{
    private readonly WeigerRelatieaanvraagContext _testContext;

    public Returns_Geweigerde_Relatieaanvraag(WeigerRelatieaanvraagContext testContext)
        : base(testContext.ApiSetup)
    {
        _testContext = testContext;
    }

    public override async Task<RelatieaanvraagResponse> GetResponse(FullBlownApiSetup setup) =>
        await setup.AdminApiHost.GetRelatieaanvraag(
            setup.AdminHttpClient,
            _testContext.AanvraagId,
            headers: new RequestParameters().WithExpectedSequence(_testContext.CommandResult.Sequence)
        );

    [Fact]
    public void With_Status_Geweigerd() => Response.Status.Should().Be(RelatieaanvraagStatus.Labels.Geweigerd);

    [Fact]
    public void With_AfhandelDatum_Today() =>
        Response.AfhandelDatum.Should().Be(DateOnly.FromDateTime(DateTime.Today).ToString("yyyy-MM-dd"));
}
