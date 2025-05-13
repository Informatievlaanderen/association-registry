namespace AssociationRegistry.Test.E2E.When_Markeer_Als_Dubbel_Van.Publiek.Detail;

using FluentAssertions;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.TestClasses;
using System.Net;
using Xunit;

[Collection(nameof(MarkeerAlsDubbelVanCollection))]
public class Returns_Vereniging : End2EndTest<HttpStatusCode>
{
    private readonly MarkeerAlsDubbelVanContext _testContext;

    public Returns_Vereniging(MarkeerAlsDubbelVanContext testContext) : base(testContext.ApiSetup)
    {
        _testContext = testContext;
    }

    public override HttpStatusCode GetResponse(FullBlownApiSetup setup)
        => setup.PublicApiHost.GetPubliekDetailStatusCode(_testContext.VCode);

    [Fact]
    public void Status_Code_Is_NotFound()
    {
        Response.Should().Be(HttpStatusCode.NotFound);
    }
}
