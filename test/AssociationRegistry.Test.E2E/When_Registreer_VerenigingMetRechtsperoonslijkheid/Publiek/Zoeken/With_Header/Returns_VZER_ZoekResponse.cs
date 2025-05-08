namespace AssociationRegistry.Test.E2E.When_Registreer_VerenigingMetRechtsperoonslijkheid.Publiek.Zoeken.With_Header;

using Admin.Api.Verenigingen.Registreer.MetRechtspersoonlijkheid.RequestModels;
using FluentAssertions;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.TestClasses;
using KellermanSoftware.CompareNetObjects;
using Public.Api.Verenigingen.Search.ResponseModels;
using Xunit;

[Collection(FullBlownApiCollection.Name)]
public class Returns_VZER_ZoekResponse : End2EndTest<RegistreerVerenigingMetRechtsperoonlijkheidTestContext, RegistreerVerenigingUitKboRequest, SearchVerenigingenResponse>
{
    private readonly RegistreerVerenigingMetRechtsperoonlijkheidTestContext _testContext;

    public Returns_VZER_ZoekResponse(RegistreerVerenigingMetRechtsperoonlijkheidTestContext testContext) : base(testContext)
    {
        _testContext = testContext;
    }

    [Fact]
    public void With_Context()
    {
        Response.Context.ShouldCompare("http://127.0.0.1:11003/v1/contexten/publiek/zoek-verenigingen-context.json");
    }

    [Fact]
    public async ValueTask WithFeitelijkeVereniging()
        => Response.Verenigingen.Single().Verenigingssubtype.Should().BeNull();

    public override Func<IApiSetup, SearchVerenigingenResponse> GetResponse
        => setup => setup.PublicApiHost.GetPubliekZoekenWithHeader(setup.SuperAdminHttpClient,$"vCode:{_testContext.RequestResult.VCode}").GetAwaiter().GetResult();
}
