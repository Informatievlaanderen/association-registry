namespace AssociationRegistry.Test.E2E.When_Corrigeer_Markeer_Als_Dubbel_Van.Publiek.Detail_All;

using FluentAssertions;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.TestClasses;
using Public.Api.Verenigingen.Detail.ResponseModels;
using Xunit;

[Collection(nameof(CorrigeerMarkeringAlsDubbelVanCollection))]
public class Returns_Vereniging : End2EndTest<PubliekVerenigingDetailResponse>
{
    private readonly CorrigeerMarkeringAlsDubbelVanContext _testContext;

    public Returns_Vereniging(CorrigeerMarkeringAlsDubbelVanContext testContext) : base(testContext.ApiSetup)
    {
        _testContext = testContext;
    }

    public override PubliekVerenigingDetailResponse GetResponse(FullBlownApiSetup setup)
        => setup.PublicApiHost
                .GetPubliekDetailAll()
                .FindVereniging(_testContext.VCode);

    [Fact]
    public void WithVereniging()
        => Response.Vereniging.Should().NotBeNull();
}
