namespace AssociationRegistry.Test.E2E.When_Markeer_Als_Dubbel_Van.Beheer.Detail;

using Admin.Api.Verenigingen.Detail.ResponseModels;
using Admin.Schema.Constants;
using FluentAssertions;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.TestClasses;
using Xunit;

[Collection(nameof(MarkeerAlsDubbelVanCollection))]
public class Returns_Vereniging : End2EndTest<DetailVerenigingResponse>
{
    private readonly MarkeerAlsDubbelVanContext _testContext;

    public Returns_Vereniging(MarkeerAlsDubbelVanContext testContext) : base(testContext.ApiSetup)
    {
        _testContext = testContext;
    }

    public override DetailVerenigingResponse GetResponse(FullBlownApiSetup setup)
        => setup.AdminApiHost.GetBeheerDetail(setup.AdminHttpClient, _testContext.VCode,new RequestParameters().WithExpectedSequence(_testContext.CommandResult.Sequence)).GetAwaiter().GetResult();

    [Fact]
    public void With_IsDubbelVan_VCode_Of_AndereFeitelijkeVerenigingWerdGeregistreerd()
    {
        Response.Vereniging.IsDubbelVan.Should().Be(_testContext.Scenario.AndereFeitelijkeVerenigingWerdGeregistreerd.VCode);
    }

    [Fact]
    public void With_Status_Is_Dubbel()
    {
        Response.Vereniging.Status.Should().Be(VerenigingStatus.Dubbel);
    }
}
