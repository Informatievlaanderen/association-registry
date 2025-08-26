namespace AssociationRegistry.Test.E2E.When_SubtypeWerdTerugGezetNaarNietBepaald.Publiek.Zoeken.With_Header;

using DecentraalBeheer.Vereniging;
using DecentraalBeheer.Vereniging.Mappers;
using FluentAssertions;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.TestClasses;
using KellermanSoftware.CompareNetObjects;
using Public.Api.WebApi.Verenigingen.Search.ResponseModels;
using Vereniging;
using Xunit;
using Verenigingssubtype = Admin.Api.WebApi.Verenigingen.Search.ResponseModels.Verenigingssubtype;

[Collection(nameof(ZetSubtypeNaarNietBepaaldCollection))]
public class Returns_Detail : End2EndTest<SearchVerenigingenResponse>
{
    private readonly ZetSubtypeNaarNietBepaaldContext _testContext;

    public Returns_Detail(ZetSubtypeNaarNietBepaaldContext testContext) : base(testContext.ApiSetup)
    {
        _testContext = testContext;
    }

    public override async Task<SearchVerenigingenResponse> GetResponse(FullBlownApiSetup setup)
        => await setup.PublicApiHost.GetPubliekZoekenWithHeader(setup.AdminHttpClient, $"vCode:{_testContext.VCode}", _testContext.CommandResult.Sequence);

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
        vereniging.Verenigingssubtype.Should().BeEquivalentTo(VerenigingssubtypeCode.NietBepaald.Map<Verenigingssubtype>());
    }
}
