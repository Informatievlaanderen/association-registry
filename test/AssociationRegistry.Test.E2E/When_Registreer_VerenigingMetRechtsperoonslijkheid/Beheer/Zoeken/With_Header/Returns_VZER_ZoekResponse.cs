namespace AssociationRegistry.Test.E2E.When_Registreer_VerenigingMetRechtsperoonslijkheid.Beheer.Zoeken.With_Header;

using Admin.Api.Verenigingen.Search.ResponseModels;
using Admin.Schema.Search;
using FluentAssertions;
using FluentAssertions.Extensions;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.TestClasses;
using KellermanSoftware.CompareNetObjects;
using Marten;
using Marten.Events.Daemon;
using Elastic.Clients.Elasticsearch;
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

    public override SearchVerenigingenResponse GetResponse(FullBlownApiSetup setup)
    {
        return setup.AdminApiHost.GetBeheerZoeken(setup.SuperAdminHttpClient, $"vCode:{_testContext.CommandResult.VCode}",
                                                  setup.AdminApiHost.DocumentStore(),headers: new RequestParameters().V2().WithExpectedSequence(_testContext.CommandResult.Sequence)).GetAwaiter()
                    .GetResult();
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
