namespace AssociationRegistry.Test.E2E.When_Corrigeer_Markeer_Als_Dubbel_Van.Publiek.Detail_All;

using FluentAssertions;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.TestClasses;
using Public.Api.Verenigingen.Detail.ResponseModels;
using Scenarios.Requests;
using Xunit;

[Collection(FullBlownApiCollection.Name)]
public class Returns_Vereniging : End2EndTest<CorrigeerMarkeringAlsDubbelVanContext, NullRequest, PubliekVerenigingDetailResponse>
{
    public override Func<IApiSetup, PubliekVerenigingDetailResponse> GetResponse =>
        setup => setup.PublicApiHost
                      .GetPubliekDetailAll()
                      .FindVereniging(TestContext.VCode);

    public Returns_Vereniging(CorrigeerMarkeringAlsDubbelVanContext context) : base(context)
    {
    }

    [Fact]
    public void WithVereniging()
        => Response.Vereniging.Should().NotBeNull();
}
