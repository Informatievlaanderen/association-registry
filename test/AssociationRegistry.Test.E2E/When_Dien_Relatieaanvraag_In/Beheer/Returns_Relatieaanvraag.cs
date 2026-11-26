namespace AssociationRegistry.Test.E2E.When_Dien_Relatieaanvraag_In.Beheer;

using Admin.Api.WebApi.Relatieaanvragen.Opvragen.ResponseModels;
using FluentAssertions;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.TestClasses;
using Xunit;

[Collection(nameof(DienRelatieaanvraagInCollection))]
public class Returns_Relatieaanvraag : End2EndTest<RelatieaanvraagResponse>
{
    private readonly DienRelatieaanvraagInContext _testContext;

    public Returns_Relatieaanvraag(DienRelatieaanvraagInContext testContext)
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
    public void With_The_Relatieaanvraag()
    {
        var vandaag = DateOnly.FromDateTime(DateTime.Today);

        Response
            .Should()
            .BeEquivalentTo(
                new RelatieaanvraagResponse
                {
                    AanvraagId = _testContext.AanvraagId,
                    KoepelVCode = _testContext.Scenario.KoepelVCode,
                    VerenigingVCode = _testContext.Scenario.VerenigingVCode,
                    AangevraagdDoor = _testContext.Scenario.KoepelVCode,
                    RelatieType = "Lidmaatschap",
                    Status = "Aangevraagd",
                    AanvraagDatum = vandaag.ToString("yyyy-MM-dd"),
                    VervalDatum = vandaag.AddDays(90).ToString("yyyy-MM-dd"),
                    Beschrijving = _testContext.CommandRequest.Beschrijving!,
                    AfhandelDatum = string.Empty,
                }
            );
    }
}
