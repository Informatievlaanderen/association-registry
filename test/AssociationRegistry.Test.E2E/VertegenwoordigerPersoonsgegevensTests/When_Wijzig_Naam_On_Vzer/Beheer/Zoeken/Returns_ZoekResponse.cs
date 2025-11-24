namespace AssociationRegistry.Test.E2E.VertegenwoordigerPersoonsgegevensTests.When_Wijzig_Naam_On_Vzer.Beheer.Zoeken;

using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Search.ResponseModels;
using AssociationRegistry.Test.E2E.Framework.AlbaHost;
using AssociationRegistry.Test.E2E.Framework.ApiSetup;
using AssociationRegistry.Test.E2E.Framework.TestClasses;
using FluentAssertions;
using KellermanSoftware.CompareNetObjects;
using Marten;
using When_Wijzig_Naam_On_FV;
using Xunit;

[Collection(nameof(WijzigNaamOnVZERTestCollection))]
public class Returns_SearchVerenigingenResponse

    : End2EndTest<SearchVerenigingenResponse>
{
    private readonly WijzigNaamOnVZERTestContext _testContext;

    private readonly ITestOutputHelper _testOutputHelper;

    public Returns_SearchVerenigingenResponse(WijzigNaamOnVZERTestContext testContext, ITestOutputHelper testOutputHelper) : base(testContext.ApiSetup)
    {
        _testContext = testContext;
        _testOutputHelper = testOutputHelper;
    }

    public override async Task<SearchVerenigingenResponse> GetResponse(FullBlownApiSetup setup)
        => await setup.AdminApiHost.GetBeheerZoeken(setup.AdminHttpClient, $"vCode:{_testContext.VCode}", setup.AdminApiHost.DocumentStore(), headers: new RequestParameters().WithExpectedSequence(_testContext.CommandResult.Sequence), testOutputHelper: _testOutputHelper);


    [Fact]
    public void With_Context()
    {
        Response.Context.ShouldCompare("http://127.0.0.1:11003/v1/contexten/beheer/zoek-verenigingen-context.json");
    }

    [Fact]
    public async ValueTask WithFeitelijkeVereniging()
        => Response.Verenigingen.Single().Naam.Should().Be(_testContext.CommandRequest.Naam);

}
