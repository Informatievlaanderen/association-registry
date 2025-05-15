namespace AssociationRegistry.Test.E2E.When_Verwijder_Vereniging.Zoeken;

using Admin.Api.Verenigingen.Search.ResponseModels;
using FluentAssertions;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.TestClasses;
using KellermanSoftware.CompareNetObjects;
using Marten;
using Xunit;

[Collection(nameof(VerwijderVerenigingCollection))]
public class Returns_SearchVerenigingenResponse : End2EndTest<SearchVerenigingenResponse>
{
    private readonly VerwijderVerenigingContext _testContext;

    public Returns_SearchVerenigingenResponse(VerwijderVerenigingContext testContext) : base(testContext.ApiSetup)
    {
        _testContext = testContext;
    }

    [Fact]
    public void With_Context()
    {
        Response.Context.ShouldCompare("http://127.0.0.1:11003/v1/contexten/beheer/zoek-verenigingen-context.json");
    }

    [Fact]
    public async ValueTask With_No_Vereniging()
        => Response.Verenigingen.SingleOrDefault(x => x.VCode == _testContext.VCode).Should().BeNull();

    public override SearchVerenigingenResponse GetResponse(FullBlownApiSetup setup)
        => setup.AdminApiHost.GetBeheerZoeken(setup.AdminHttpClient, $"vCode:{_testContext.VCode}", setup.AdminApiHost.DocumentStore(), headers: new RequestParameters().WithExpectedSequence(_testContext.CommandResult.Sequence)).GetAwaiter().GetResult();

}
