namespace AssociationRegistry.Test.E2E.When_Trek_Relatieaanvraag_In.Beheer;

using Admin.Api.WebApi.Relatieaanvragen.Opvragen.ResponseModels;
using DecentraalBeheer.Relatieaanvragen;
using FluentAssertions;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.TestClasses;
using Scenarios.Requests.Relatieaanvragen;
using Xunit;

[Collection(nameof(TrekRelatieaanvraagInCollection))]
public class Returns_Ingetrokken_Relatieaanvraag : End2EndTest<RelatieaanvraagResponse>
{
    private readonly TrekRelatieaanvraagInContext _testContext;

    public Returns_Ingetrokken_Relatieaanvraag(TrekRelatieaanvraagInContext testContext)
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
    public void With_Status_Ingetrokken() => Response.Status.Should().Be(RelatieaanvraagStatus.Labels.Ingetrokken);

    [Fact]
    public void With_AfhandelDatum_Today() =>
        Response.AfhandelDatum.Should().Be(DateOnly.FromDateTime(DateTime.Today).ToString("yyyy-MM-dd"));
}
