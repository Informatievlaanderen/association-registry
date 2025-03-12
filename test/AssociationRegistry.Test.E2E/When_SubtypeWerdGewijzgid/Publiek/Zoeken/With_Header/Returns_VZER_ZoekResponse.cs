namespace AssociationRegistry.Test.E2E.When_SubtypeWerdGewijzgid.Publiek.Zoeken.With_Header;

using AssociationRegistry.Admin.Api.Verenigingen.Subtype.RequestModels;
using AssociationRegistry.Public.Api.Verenigingen.Search.ResponseModels;
using AssociationRegistry.Test.E2E.Framework.AlbaHost;
using AssociationRegistry.Test.E2E.Framework.ApiSetup;
using AssociationRegistry.Test.E2E.Framework.TestClasses;
using FluentAssertions;
using KellermanSoftware.CompareNetObjects;
using Xunit;
using Verenigingssubtype = Admin.Api.Verenigingen.Search.ResponseModels.Verenigingssubtype;

[Collection(FullBlownApiCollection.Name)]
public class Returns_VZER_ZoekResponse : End2EndTest<WhenSubtypeWerdGewijzigdContext, WijzigSubtypeRequest, SearchVerenigingenResponse>
{
    private readonly WhenSubtypeWerdGewijzigdContext _testContext;

    public Returns_VZER_ZoekResponse(WhenSubtypeWerdGewijzigdContext testContext) : base(testContext)
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
        vereniging.Verenigingssubtype.Should().BeEquivalentTo(new Verenigingssubtype
        {
            Naam = AssociationRegistry.Vereniging.Verenigingssubtype.Subvereniging.Naam,
            Code = AssociationRegistry.Vereniging.Verenigingssubtype.Subvereniging.Code,
        });
    }

    public override Func<IApiSetup, SearchVerenigingenResponse> GetResponse
        => setup => setup.AdminApiHost.GetPubliekZoekenWithHeader(setup.SuperAdminHttpClient,$"vCode:{_testContext.VCode}").GetAwaiter().GetResult();
}
