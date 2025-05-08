namespace AssociationRegistry.Test.E2E.When_Stop_Vereniging.Beheer.Zoeken;

using Admin.Api.Verenigingen.Search.ResponseModels;
using Admin.Api.Verenigingen.Stop.RequestModels;
using FluentAssertions;
using Formats;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.TestClasses;
using KellermanSoftware.CompareNetObjects;
using Xunit;
using VerenigingStatus = Admin.Schema.Constants.VerenigingStatus;

[Collection(FullBlownApiCollection.Name)]
public class Returns_SearchVerenigingenResponse : End2EndTest<StopVerenigingContext, StopVerenigingRequest, SearchVerenigingenResponse>
{
    private readonly StopVerenigingContext _testContext;

    public Returns_SearchVerenigingenResponse(StopVerenigingContext testContext)
    {
        TestContext = _testContext = testContext;
    }

    [Fact]
    public void With_Context()
    {
        Response.Context.ShouldCompare("http://127.0.0.1:11003/v1/contexten/beheer/zoek-verenigingen-context.json");
    }

    [Fact]
    public async ValueTask WithFeitelijkeVereniging()
    {
        Response.Verenigingen.Single().Einddatum.Should().Be(_testContext.Request.Einddatum.FormatAsBelgianDate());
        Response.Verenigingen.Single().Status.Should().Be(VerenigingStatus.Gestopt);
    }

    public override Func<IApiSetup, SearchVerenigingenResponse> GetResponse
        => setup => setup.AdminApiHost.GetBeheerZoeken($"vCode:{_testContext.VCode}");
}
