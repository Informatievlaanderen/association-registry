namespace AssociationRegistry.Test.E2E.When_Corrigeer_Markeer_Als_Dubbel_Van.Beheer.Detail;

using Admin.Api.WebApi.Verenigingen.Detail.ResponseModels;
using Admin.Schema.Constants;
using FluentAssertions;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.TestClasses;
using Xunit;

[Collection(nameof(CorrigeerMarkeringAlsDubbelVanCollection))]
public class Returns_Detail_With_Dubbel_Van : End2EndTest<DetailVerenigingResponse>
{
    private readonly CorrigeerMarkeringAlsDubbelVanContext _testContext;

    public Returns_Detail_With_Dubbel_Van(CorrigeerMarkeringAlsDubbelVanContext testContext) : base(testContext.ApiSetup)
    {
        _testContext = testContext;
    }

    public override async Task<DetailVerenigingResponse> GetResponse(FullBlownApiSetup setup)
        => await setup.AdminApiHost.GetBeheerDetail(setup.AdminHttpClient ,_testContext.Scenario.MultipleWerdGeregistreerdScenario.FeitelijkeVerenigingWerdGeregistreerd.VCode, headers: new RequestParameters().WithExpectedSequence(_testContext.CommandResult.Sequence));

    [Fact]
    public void With_IsDubbelVan_VCode_Of_AndereFeitelijkeVerenigingWerdGeregistreerd()
    {
        Response.Vereniging.IsDubbelVan.Should().BeEmpty();
    }

    [Fact]
    public void With_Status_Is_Dubbel()
    {
        Response.Vereniging.Status.Should().Be(VerenigingStatus.Actief);
    }
}
