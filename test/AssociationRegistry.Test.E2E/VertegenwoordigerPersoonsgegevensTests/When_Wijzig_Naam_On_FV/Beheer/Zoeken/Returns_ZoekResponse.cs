namespace AssociationRegistry.Test.E2E.VertegenwoordigerPersoonsgegevensTests.When_Wijzig_Naam_On_FV.Beheer.Zoeken;

using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Search.ResponseModels;
using AssociationRegistry.Contracts.JsonLdContext;
using AssociationRegistry.Formats;
using AssociationRegistry.Test.E2E.Framework.AlbaHost;
using AssociationRegistry.Test.E2E.Framework.ApiSetup;
using AssociationRegistry.Test.E2E.Framework.Comparison;
using AssociationRegistry.Test.E2E.Framework.Mappers;
using AssociationRegistry.Test.E2E.Framework.TestClasses;
using FluentAssertions;
using KellermanSoftware.CompareNetObjects;
using Marten;
using NodaTime;
using Xunit;
using Vereniging = Admin.Api.WebApi.Verenigingen.Search.ResponseModels.Vereniging;
using VerenigingStatus = Admin.Schema.Constants.VerenigingStatus;
using Verenigingstype = Admin.Api.WebApi.Verenigingen.Search.ResponseModels.Verenigingstype;

[Collection(nameof(WijzigNaamOnFVTestCollection))]
public class Returns_SearchVerenigingenResponse

    : End2EndTest<SearchVerenigingenResponse>
{
    private readonly WijzigNaamOnFVTestContext _testContext;

    private readonly ITestOutputHelper _testOutputHelper;

    public Returns_SearchVerenigingenResponse(WijzigNaamOnFVTestContext testContext, ITestOutputHelper testOutputHelper) : base(testContext.ApiSetup)
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
