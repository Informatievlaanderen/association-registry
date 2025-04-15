namespace AssociationRegistry.Test.E2E.When_Verwijder_Vereniging.Beheer.Zoeken;

using Admin.Api.Verenigingen.Search.ResponseModels;
using Admin.Api.Verenigingen.Verwijder.RequestModels;
using FluentAssertions;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.TestClasses;
using KellermanSoftware.CompareNetObjects;
using Xunit;

[Collection(FullBlownApiCollection.Name)]
public class Returns_SearchVerenigingenResponse : End2EndTest<VerwijderVerenigingContext, VerwijderVerenigingRequest, SearchVerenigingenResponse>
{
    private readonly VerwijderVerenigingContext _testContext;

    public Returns_SearchVerenigingenResponse(VerwijderVerenigingContext testContext) : base(testContext)
    {
        _testContext = testContext;
    }

    [Fact]
    public void With_Context()
    {
        Response.Context.ShouldCompare("http://127.0.0.1:11003/v1/contexten/beheer/zoek-verenigingen-context.json");
    }

    [Fact]
    public async Task With_No_Vereniging()
        => Response.Verenigingen.SingleOrDefault(x => x.VCode == _testContext.VCode).Should().BeNull();

    public override Func<IApiSetup, SearchVerenigingenResponse> GetResponse
        => setup => setup.AdminApiHost.GetBeheerZoeken($"vCode:{_testContext.VCode}");
}
