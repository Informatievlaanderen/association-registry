namespace AssociationRegistry.Test.E2E.When_SubtypeWerdGewijzgid.Beheer.Zoeken.Without_Header;

using Admin.Api.Verenigingen.Search.ResponseModels;
using FluentAssertions;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.TestClasses;
using KellermanSoftware.CompareNetObjects;
using Xunit;

[Collection(nameof(WhenSubtypeWerdGewijzigdCollection))]
public class Returns_ZoekResponse : End2EndTest<SearchVerenigingenResponse>
{
    private readonly WhenSubtypeWerdGewijzigdContext _testContext;

    public Returns_ZoekResponse(WhenSubtypeWerdGewijzigdContext testContext) : base(testContext.ApiSetup)
    {
        _testContext = testContext;
    }

    public override SearchVerenigingenResponse GetResponse(FullBlownApiSetup setup)
        => setup.AdminApiHost.GetBeheerZoeken(setup.AdminHttpClient, $"vCode:{_testContext.VCode}", new RequestHeadersBuilder().WithExpectedSequence(_testContext.CommandResult.Sequence)).GetAwaiter().GetResult();


    [Fact]
    public void With_Context()
    {
        Response.Context.ShouldCompare("http://127.0.0.1:11003/v1/contexten/beheer/zoek-verenigingen-context.json");
    }

    [Fact]
    public async ValueTask WithFeitelijkeVereniging()
    {
        var vereniging = Response.Verenigingen.Single();
        vereniging.VCode.Should().BeEquivalentTo(_testContext.VCode);
        vereniging.Verenigingssubtype.Should().BeNull();
    }
}
