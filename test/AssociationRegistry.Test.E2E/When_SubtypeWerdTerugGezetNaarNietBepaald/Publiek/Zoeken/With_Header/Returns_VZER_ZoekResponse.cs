namespace AssociationRegistry.Test.E2E.When_SubtypeWerdTerugGezetNaarNietBepaald.Publiek.Zoeken.With_Header;

using AssociationRegistry.Admin.Api.Verenigingen.Subtype.RequestModels;
using AssociationRegistry.Public.Api.Verenigingen.Search.ResponseModels;
using AssociationRegistry.Test.E2E.Framework.AlbaHost;
using AssociationRegistry.Test.E2E.Framework.ApiSetup;
using AssociationRegistry.Test.E2E.Framework.TestClasses;
using FluentAssertions;
using KellermanSoftware.CompareNetObjects;
using Vereniging;
using Vereniging.Mappers;
using When_SubtypeWerdVerfijndNaarFeitelijkeVereniging;
using Xunit;
using Verenigingssubtype = Admin.Api.Verenigingen.Search.ResponseModels.Verenigingssubtype;

[Collection(FullBlownApiCollection.Name)]
public class Returns_VZER_ZoekResponse : End2EndTest<ZetSubtypeNaarNietBepaaldContext, WijzigSubtypeRequest, SearchVerenigingenResponse>
{
    private readonly ZetSubtypeNaarNietBepaaldContext _testContext;

    public Returns_VZER_ZoekResponse(ZetSubtypeNaarNietBepaaldContext testContext) : base(testContext)
    {
        _testContext = testContext;
    }

    [Fact]
    public void With_Context()
    {
        Response.Context.ShouldCompare("http://127.0.0.1:11003/v1/contexten/publiek/zoek-verenigingen-context.json");
    }

    [Fact]
    public async Task WithFeitelijkeVereniging()
    {
        var vereniging = Response.Verenigingen.Single();
        vereniging.VCode.Should().BeEquivalentTo(_testContext.VCode);
        vereniging.Verenigingssubtype.Should().BeEquivalentTo(VerenigingssubtypeCodering.NietBepaald.Map<Verenigingssubtype>());
    }

    public override Func<IApiSetup, SearchVerenigingenResponse> GetResponse
        => setup => setup.PublicApiHost.GetPubliekZoekenWithHeader(setup.SuperAdminHttpClient,$"vCode:{_testContext.VCode}").GetAwaiter().GetResult();
}
