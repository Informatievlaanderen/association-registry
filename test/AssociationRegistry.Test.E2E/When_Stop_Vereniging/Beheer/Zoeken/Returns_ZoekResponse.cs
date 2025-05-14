namespace AssociationRegistry.Test.E2E.When_Stop_Vereniging.Beheer.Zoeken;

using Admin.Api.Verenigingen.Search.ResponseModels;
using FluentAssertions;
using Formats;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.TestClasses;
using KellermanSoftware.CompareNetObjects;
using Xunit;
using VerenigingStatus = Admin.Schema.Constants.VerenigingStatus;

[Collection(nameof(StopVerenigingCollection))]
public class Returns_ZoekResponse : End2EndTest<SearchVerenigingenResponse>
{
    private readonly StopVerenigingContext _testContext;

    public Returns_ZoekResponse(StopVerenigingContext testContext) : base(testContext.ApiSetup)
    {
        _testContext = testContext;
    }

    public override SearchVerenigingenResponse GetResponse(FullBlownApiSetup setup)
        => setup.AdminApiHost.GetBeheerZoeken(setup.AdminHttpClient ,$"vCode:{_testContext.VCode}", new RequestParameters().WithExpectedSequence(_testContext.CommandResult.Sequence)).GetAwaiter().GetResult();

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
