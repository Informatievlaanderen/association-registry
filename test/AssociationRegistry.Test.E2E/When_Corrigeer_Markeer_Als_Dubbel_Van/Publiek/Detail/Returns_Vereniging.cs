namespace AssociationRegistry.Test.E2E.When_Corrigeer_Markeer_Als_Dubbel_Van.Publiek.Detail;

using FluentAssertions;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.TestClasses;
using Public.Api.Verenigingen.Detail.ResponseModels;
using Scenarios.Requests;
using Xunit;

[Collection(FullBlownApiCollection.Name)]
public class Returns_Vereniging : End2EndTest<CorrigeerMarkeringAlsDubbelVanContext, NullRequest,
    PubliekVerenigingDetailResponse>
{
    private readonly CorrigeerMarkeringAlsDubbelVanContext _context;

    public Returns_Vereniging(CorrigeerMarkeringAlsDubbelVanContext context) : base(context)
    {
        _context = context;
    }

    [Fact]
    public void Response_Not_Null()
    {
        Response.Should().NotBeNull();
    }

    public override Func<IApiSetup, PubliekVerenigingDetailResponse> GetResponse
        => setup => setup.PublicApiHost.GetPubliekDetail(_context.Scenario.DubbeleVerenging.VCode);
}
