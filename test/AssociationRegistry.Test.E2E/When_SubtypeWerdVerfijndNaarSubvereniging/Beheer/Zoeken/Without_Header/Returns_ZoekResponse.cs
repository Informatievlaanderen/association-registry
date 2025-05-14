namespace AssociationRegistry.Test.E2E.When_SubtypeWerdVerfijndNaarSubvereniging.Beheer.Zoeken.Without_Header;

using AssociationRegistry.Admin.Api.Verenigingen.Search.ResponseModels;
using AssociationRegistry.Test.E2E.Framework.AlbaHost;
using AssociationRegistry.Test.E2E.Framework.ApiSetup;
using AssociationRegistry.Test.E2E.Framework.TestClasses;
using FluentAssertions;
using KellermanSoftware.CompareNetObjects;
using Xunit;

[Collection(nameof(VerfijnSubtypeNaarSubverenigingCollection))]
public class Returns_Detail : End2EndTest<SearchVerenigingenResponse>
{
    private readonly VerfijnSubtypeNaarSubverenigingContext _testContext;

    public Returns_Detail(VerfijnSubtypeNaarSubverenigingContext testContext) : base(testContext.ApiSetup)
    {
        _testContext = testContext;
    }

    public override SearchVerenigingenResponse GetResponse(FullBlownApiSetup setup)
        => setup.AdminApiHost.GetBeheerZoeken(setup.SuperAdminHttpClient, $"vCode:{_testContext.VCode}", headers: new RequestParameters().WithExpectedSequence(
                                                  _testContext.CommandResult.Sequence))
                .GetAwaiter().GetResult();

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
        vereniging.Verenigingssubtype.Should().BeNull();
        vereniging.SubverenigingVan.Should().BeNull();
    }
}
