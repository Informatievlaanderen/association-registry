namespace AssociationRegistry.Test.E2E.When_SubtypeWerdVerfijndNaarSubvereniging.Publiek.Zoeken.With_Header;

using FluentAssertions;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.TestClasses;
using KellermanSoftware.CompareNetObjects;
using Public.Api.Verenigingen.Search.ResponseModels;
using Vereniging;
using Vereniging.Mappers;
using Xunit;
using SubverenigingVan = Public.Api.Verenigingen.Search.ResponseModels.SubverenigingVan;
using Verenigingssubtype = Admin.Api.Verenigingen.Search.ResponseModels.Verenigingssubtype;

[Collection(nameof(VerfijnSubtypeNaarSubverenigingCollection))]
public class Returns_Detail : End2EndTest<SearchVerenigingenResponse>
{
    private readonly VerfijnSubtypeNaarSubverenigingContext _testContext;

    public Returns_Detail(VerfijnSubtypeNaarSubverenigingContext testContext) : base(testContext.ApiSetup)
    {
        _testContext = testContext;
    }

    public override SearchVerenigingenResponse GetResponse(FullBlownApiSetup setup)
        => setup.PublicApiHost.GetPubliekZoekenWithHeader(setup.SuperAdminHttpClient,$"vCode:{_testContext.VCode}", _testContext.CommandResult.Sequence).GetAwaiter().GetResult();

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
        vereniging.Verenigingssubtype.Should().BeEquivalentTo(VerenigingssubtypeCode.Subvereniging.Map<Verenigingssubtype>());

        vereniging.SubverenigingVan.Should().BeEquivalentTo(new SubverenigingVan
        {
            AndereVereniging = _testContext.CommandRequest.AndereVereniging!,
            Beschrijving = _testContext.CommandRequest.Beschrijving!,
            Identificatie = _testContext.CommandRequest.Identificatie!,
        });
    }
}
