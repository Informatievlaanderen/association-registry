namespace AssociationRegistry.Test.E2E.When_Corrigeer_Markeer_Als_Dubbel_Van.Publiek.Detail;

using FluentAssertions;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.TestClasses;
using Public.Api.WebApi.Verenigingen.Detail.ResponseModels;
using Xunit;

[Collection(nameof(CorrigeerMarkeringAlsDubbelVanCollection))]
public class Returns_Vereniging : End2EndTest<PubliekVerenigingDetailResponse>
{
    private readonly CorrigeerMarkeringAlsDubbelVanContext _testContext;

    public Returns_Vereniging(CorrigeerMarkeringAlsDubbelVanContext testContext) : base(testContext.ApiSetup)
    {
        _testContext = testContext;
    }

    public override async Task<PubliekVerenigingDetailResponse> GetResponse(FullBlownApiSetup setup)
        => await setup.PublicApiHost.GetPubliekDetail(_testContext.Scenario.DubbeleVerenging.VCode);

    [Fact]
    public void Response_Not_Null()
    {
        Response.Should().NotBeNull();
    }
}
