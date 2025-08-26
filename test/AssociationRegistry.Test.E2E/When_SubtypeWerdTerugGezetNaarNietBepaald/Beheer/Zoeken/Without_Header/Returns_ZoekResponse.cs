namespace AssociationRegistry.Test.E2E.When_SubtypeWerdTerugGezetNaarNietBepaald.Beheer.Zoeken.Without_Header;

using Admin.Api.WebApi.Verenigingen.Search.ResponseModels;
using FluentAssertions;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.TestClasses;
using KellermanSoftware.CompareNetObjects;
using Marten;
using Xunit;


[Collection(nameof(ZetSubtypeNaarNietBepaaldCollection))]
public class Returns_Detail : End2EndTest<SearchVerenigingenResponse>
{
    private readonly ZetSubtypeNaarNietBepaaldContext _testContext;
    private readonly ITestOutputHelper _testOutputHelper;

    public Returns_Detail(ZetSubtypeNaarNietBepaaldContext testContext, ITestOutputHelper testOutputHelper) : base(testContext.ApiSetup)
    {
        _testContext = testContext;
        _testOutputHelper = testOutputHelper;
    }

    public override async Task<SearchVerenigingenResponse> GetResponse(FullBlownApiSetup setup)
        => await setup.AdminApiHost.GetBeheerZoeken(setup.AdminHttpClient, $"vCode:{_testContext.VCode}", setup.AdminApiHost.DocumentStore(), testOutputHelper: _testOutputHelper, headers: new RequestParameters().WithExpectedSequence(_testContext.CommandResult.Sequence));

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
