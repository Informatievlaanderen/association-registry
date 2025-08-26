namespace AssociationRegistry.Test.E2E.When_Stop_Vereniging.Beheer.Zoeken;

using Admin.Api.WebApi.Verenigingen.Search.ResponseModels;
using FluentAssertions;
using Formats;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.TestClasses;
using KellermanSoftware.CompareNetObjects;
using Marten;
using Xunit;
using ITestOutputHelper = Xunit.ITestOutputHelper;
using VerenigingStatus = Admin.Schema.Constants.VerenigingStatus;

[Collection(nameof(StopVerenigingCollection))]
public class Returns_ZoekResponse : End2EndTest<SearchVerenigingenResponse>
{
    private readonly StopVerenigingContext _testContext;
    private readonly ITestOutputHelper _testOutputHelper;

    public Returns_ZoekResponse(StopVerenigingContext testContext, ITestOutputHelper testOutputHelper) : base(testContext.ApiSetup)
    {
        _testContext = testContext;
        _testOutputHelper = testOutputHelper;
    }

    public override async Task<SearchVerenigingenResponse> GetResponse(FullBlownApiSetup setup)
        => await setup.AdminApiHost.GetBeheerZoeken(setup.AdminHttpClient ,$"vCode:{_testContext.VCode}",
                                              setup.AdminApiHost.DocumentStore(),
                                              headers: new RequestParameters().WithExpectedSequence(_testContext.CommandResult.Sequence),
                                              testOutputHelper: _testOutputHelper);

    [Fact]
    public void With_Context()
    {
        Response.Context.ShouldCompare("http://127.0.0.1:11003/v1/contexten/beheer/zoek-verenigingen-context.json");
    }

    [Fact]
    public async ValueTask WithFeitelijkeVereniging()
    {
        Response.Verenigingen.Single().Einddatum.Should().Be(_testContext.CommandRequest.Einddatum.FormatAsBelgianDate());
        Response.Verenigingen.Single().Status.Should().Be(VerenigingStatus.Gestopt);
    }
}
