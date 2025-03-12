namespace AssociationRegistry.Test.E2E.When_SubtypeWerdVerfijndNaarSubvereniging.Publiek.Zoeken.Without_Header;

using AssociationRegistry.Admin.Api.Verenigingen.Subtype.RequestModels;
using AssociationRegistry.Public.Api.Verenigingen.Search.ResponseModels;
using AssociationRegistry.Test.E2E.Framework.AlbaHost;
using AssociationRegistry.Test.E2E.Framework.ApiSetup;
using AssociationRegistry.Test.E2E.Framework.TestClasses;
using FluentAssertions;
using KellermanSoftware.CompareNetObjects;
using Xunit;

[Collection(FullBlownApiCollection.Name)]
public class Returns_SearchVerenigingenResponse : End2EndTest<VerfijnSubtypeNaarFeitelijkeVerenigingContext, WijzigSubtypeRequest, SearchVerenigingenResponse>
{
    private readonly VerfijnSubtypeNaarFeitelijkeVerenigingContext _testContext;

    public Returns_SearchVerenigingenResponse(VerfijnSubtypeNaarFeitelijkeVerenigingContext testContext) : base(testContext)
    {
        _testContext = testContext;
    }

    [Fact]
    public void With_Context()
    {
        Response.Context.ShouldCompare("http://127.0.0.1:11003/v1/contexten/beheer/zoek-verenigingen-context.json");
    }

    [Fact]
    public async Task WithFeitelijkeVereniging()
    {
        var vereniging = Response.Verenigingen.Single();
        vereniging.VCode.Should().BeEquivalentTo(_testContext.VCode);
        vereniging.Verenigingssubtype.Should().BeNull();
    }

    public override Func<IApiSetup, SearchVerenigingenResponse> GetResponse
        => setup => setup.AdminApiHost.GetPubliekZoeken($"vCode:{_testContext.VCode}");
}
