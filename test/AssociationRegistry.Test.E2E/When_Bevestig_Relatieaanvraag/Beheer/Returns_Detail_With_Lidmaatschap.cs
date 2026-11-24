namespace AssociationRegistry.Test.E2E.When_Bevestig_Relatieaanvraag.Beheer;

using Admin.Api.WebApi.Verenigingen.Detail.ResponseModels;
using FluentAssertions;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.TestClasses;
using Xunit;

/// <summary>The relatie is made in the same transaction as the confirmation: the same sequence shows both.</summary>
[Collection(nameof(BevestigRelatieaanvraagCollection))]
public class Returns_Detail_With_Lidmaatschap : End2EndTest<DetailVerenigingResponse>
{
    private readonly BevestigRelatieaanvraagContext _testContext;

    public Returns_Detail_With_Lidmaatschap(BevestigRelatieaanvraagContext testContext)
        : base(testContext.ApiSetup)
    {
        _testContext = testContext;
    }

    // The subtype is only in the response with VR-Api-Version v2.
    public override async Task<DetailVerenigingResponse> GetResponse(FullBlownApiSetup setup) =>
        await setup.AdminApiHost.GetBeheerDetail(
            setup.AdminHttpClient,
            _testContext.Scenario.VerenigingVCode,
            headers: new RequestParameters().V2().WithExpectedSequence(_testContext.CommandResult.Sequence)
        );

    [Fact]
    public void With_A_Lidmaatschap_Of_The_Koepel_From_Today() =>
        Response
            .Vereniging.Lidmaatschappen.Should()
            .ContainSingle()
            .Which.Should()
            .BeEquivalentTo(
                new
                {
                    AndereVereniging = _testContext.Scenario.KoepelVCode,
                    Van = DateOnly.FromDateTime(DateTime.Today).ToString("yyyy-MM-dd"),
                    Tot = string.Empty,
                    Identificatie = _testContext.CommandRequest.Identificatie,
                    Beschrijving = _testContext.CommandRequest.Beschrijving,
                },
                options => options.ExcludingMissingMembers()
            );

    [Fact]
    public void With_Subtype_FV() => Response.Vereniging.Verenigingssubtype!.Code.Should().Be("FV");
}
