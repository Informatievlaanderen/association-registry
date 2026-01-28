namespace AssociationRegistry.Test.E2E.When_Registreer_VerenigingMetRechtspersoonlijkheid.Beheer.Zoeken.With_Header;

using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Search.ResponseModels;
using AssociationRegistry.Test.E2E.Framework.AlbaHost;
using AssociationRegistry.Test.E2E.Framework.ApiSetup;
using AssociationRegistry.Test.E2E.Framework.TestClasses;
using FluentAssertions;
using KellermanSoftware.CompareNetObjects;
using Marten;
using Xunit;

[Collection(nameof(RegistreerVerenigingMetRechtsperoonlijkheidCollection))]
public class Returns_Vereniging : End2EndTest<SearchVerenigingenResponse>
{
    private readonly RegistreerVerenigingMetRechtsperoonlijkheidContext _testContext;
    private readonly ITestOutputHelper _helper;

    public Returns_Vereniging(RegistreerVerenigingMetRechtsperoonlijkheidContext testContext, ITestOutputHelper helper) : base(testContext.ApiSetup)
    {
        _testContext = testContext;
        _helper = helper;
    }

    public override async Task<SearchVerenigingenResponse> GetResponse(FullBlownApiSetup setup)
    {
        return await setup.AdminApiHost.GetBeheerZoeken(setup.SuperAdminHttpClient, $"vCode:{_testContext.CommandResult.VCode}",
                                                  setup.AdminApiHost.DocumentStore(),
                                                  headers: new RequestParameters().V2()
                                                                                  .WithExpectedSequence(
                                                                                       _testContext.CommandResult.Sequence));
    }

    [Fact]
    public void With_Context()
    {
        Response.Context.ShouldCompare("http://127.0.0.1:11003/v1/contexten/beheer/zoek-verenigingen-context.json");
    }

    [Fact]
    public async ValueTask WithFeitelijkeVereniging()
        => Response.Verenigingen.Single().Verenigingssubtype.Should().BeNull();
}
