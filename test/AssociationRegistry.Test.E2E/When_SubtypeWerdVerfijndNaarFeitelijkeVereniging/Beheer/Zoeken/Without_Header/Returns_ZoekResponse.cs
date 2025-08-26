namespace AssociationRegistry.Test.E2E.When_SubtypeWerdVerfijndNaarFeitelijkeVereniging.Beheer.Zoeken.Without_Header;

using Admin.Api.WebApi.Verenigingen.Search.ResponseModels;
using AssociationRegistry.Test.E2E.Framework.ApiSetup;
using AssociationRegistry.Test.E2E.Framework.TestClasses;
using FluentAssertions;
using Framework.AlbaHost;
using KellermanSoftware.CompareNetObjects;
using Marten;
using Xunit;
using ITestOutputHelper = Xunit.ITestOutputHelper;

[Collection(nameof(VerfijnSubtypeNaarFeitelijkeVerenigingCollection))]
public class Returns_Detail : End2EndTest<SearchVerenigingenResponse>
{
    private readonly VerfijnSubtypeNaarFeitelijkeVerenigingContext _testContext;
    private readonly ITestOutputHelper _testOutputHelper;

    public Returns_Detail(VerfijnSubtypeNaarFeitelijkeVerenigingContext testContext, ITestOutputHelper testOutputHelper) : base(testContext.ApiSetup)
    {
        _testContext = testContext;
        _testOutputHelper = testOutputHelper;
    }

    public override async Task<SearchVerenigingenResponse> GetResponse(FullBlownApiSetup setup)
        => await setup.AdminApiHost.GetBeheerZoeken(setup.AdminHttpClient, $"vCode:{_testContext.VCode}", setup.AdminApiHost.DocumentStore(), headers: new RequestParameters()
                                                 .WithExpectedSequence(_testContext.CommandResult.Sequence), testOutputHelper: _testOutputHelper);
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
