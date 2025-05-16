namespace AssociationRegistry.Test.E2E.When_SubtypeWerdVerfijndNaarFeitelijkeVereniging.Publiek.Zoeken.Without_Header;

using Admin.Api.Verenigingen.Subtype.RequestModels;
using AssociationRegistry.Test.E2E.Framework.AlbaHost;
using AssociationRegistry.Test.E2E.Framework.ApiSetup;
using AssociationRegistry.Test.E2E.Framework.TestClasses;
using FluentAssertions;
using KellermanSoftware.CompareNetObjects;
using Public.Api.Verenigingen.Search.ResponseModels;
using Xunit;

[Collection(FullBlownApiCollection.Name)]
public class Returns_SearchVerenigingenResponse : End2EndTest<VerfijnSubtypeNaarFeitelijkeVerenigingContext, WijzigSubtypeRequest, SearchVerenigingenResponse>
{
    private readonly VerfijnSubtypeNaarFeitelijkeVerenigingContext _testContext;

    public Returns_SearchVerenigingenResponse(VerfijnSubtypeNaarFeitelijkeVerenigingContext testContext)
    {
        TestContext = _testContext = testContext;
    }

    [Fact]
    public void With_Context()
    {
        Response.Context.ShouldCompare("http://127.0.0.1:11003/v1/contexten/publiek/zoek-verenigingen-context.json");
    }

    [Fact]
    public async ValueTask WithFeitelijkeVereniging()
    {
        var vereniging = Response.Verenigingen.Single();
        vereniging.VCode.Should().BeEquivalentTo(_testContext.VCode);
        vereniging.Verenigingssubtype.Should().BeNull();
    }

    public override Func<IApiSetup, SearchVerenigingenResponse> GetResponse
        => setup => setup.PublicApiHost.GetPubliekZoeken($"vCode:{_testContext.VCode}");
}
