namespace AssociationRegistry.Test.E2E.When_Bevestig_Relatieaanvraag.Beheer;

using Admin.Api.WebApi.Relatieaanvragen.Opvragen.ResponseModels;
using FluentAssertions;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.TestClasses;
using Xunit;

[Collection(nameof(BevestigRelatieaanvraagCollection))]
public class Returns_Bevestigde_Relatieaanvraag : End2EndTest<RelatieaanvraagResponse>
{
    private readonly BevestigRelatieaanvraagContext _testContext;

    public Returns_Bevestigde_Relatieaanvraag(BevestigRelatieaanvraagContext testContext)
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
    public void With_Status_Bevestigd() => Response.Status.Should().Be("Bevestigd");

    [Fact]
    public void With_The_Bevestiging_Of_Today() =>
        Response
            .Bevestiging.Should()
            .BeEquivalentTo(
                new RelatieaanvraagBevestigingResponse
                {
                    Datum = DateOnly.FromDateTime(DateTime.Today).ToString("yyyy-MM-dd"),
                }
            );
}
