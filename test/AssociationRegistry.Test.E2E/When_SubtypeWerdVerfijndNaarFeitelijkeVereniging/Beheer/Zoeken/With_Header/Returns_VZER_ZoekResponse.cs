namespace AssociationRegistry.Test.E2E.When_SubtypeWerdVerfijndNaarFeitelijkeVereniging.Beheer.Zoeken.With_Header;

using Admin.Api.WebApi.Verenigingen.Search.ResponseModels;
using DecentraalBeheer.Vereniging;
using DecentraalBeheer.Vereniging.Mappers;
using FluentAssertions;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.TestClasses;
using KellermanSoftware.CompareNetObjects;
using Marten;
using Vereniging;
using Xunit;
using Verenigingssubtype = Admin.Api.WebApi.Verenigingen.Search.ResponseModels.Verenigingssubtype;

[Collection(nameof(VerfijnSubtypeNaarFeitelijkeVerenigingCollection))]
public class Returns_Detail : End2EndTest<SearchVerenigingenResponse>
{
    private readonly VerfijnSubtypeNaarFeitelijkeVerenigingContext _testContext;

    public Returns_Detail(VerfijnSubtypeNaarFeitelijkeVerenigingContext testContext) : base(testContext.ApiSetup)
    {
        _testContext = testContext;
    }

    public override SearchVerenigingenResponse GetResponse(FullBlownApiSetup setup)
        => setup.AdminApiHost.GetBeheerZoeken(setup.AdminHttpClient, $"vCode:{_testContext.VCode}", setup.AdminApiHost.DocumentStore(), headers: new RequestParameters()
                                                 .V2().WithExpectedSequence(_testContext.CommandResult.Sequence)).GetAwaiter().GetResult();

    [Fact]
    public void With_Context()
    {
        Response.Context.ShouldCompare("http://127.0.0.1:11003/v1/contexten/beheer/zoek-verenigingen-context.json");
    }

    [Fact]
    public async ValueTask WithFeitelijkeVereniging()
    {
        var vereniging = Response.Verenigingen.Single();
        vereniging.VCode.Should().BeEquivalentTo(_testContext.VCode);
        vereniging.Verenigingssubtype.Should().BeEquivalentTo(VerenigingssubtypeCode.FeitelijkeVereniging.Map<Verenigingssubtype>());
    }
}
