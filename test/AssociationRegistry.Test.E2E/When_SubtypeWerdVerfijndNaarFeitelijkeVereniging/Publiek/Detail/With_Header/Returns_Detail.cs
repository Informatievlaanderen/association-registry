namespace AssociationRegistry.Test.E2E.When_SubtypeWerdVerfijndNaarFeitelijkeVereniging.Publiek.Detail.With_Header;

using FluentAssertions;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.TestClasses;
using KellermanSoftware.CompareNetObjects;
using Public.Api.Verenigingen.Detail.ResponseModels;
using Vereniging;
using Xunit;
using Verenigingssubtype = Public.Api.Verenigingen.Detail.ResponseModels.Verenigingssubtype;

[Collection(nameof(VerfijnSubtypeNaarFeitelijkeVerenigingCollection))]
public class Returns_Detail : End2EndTest<PubliekVerenigingDetailResponse>
{
    private readonly VerfijnSubtypeNaarFeitelijkeVerenigingContext _testContext;

    public Returns_Detail(VerfijnSubtypeNaarFeitelijkeVerenigingContext testContext) : base(testContext.ApiSetup)
    {
        _testContext = testContext;
    }

    public override PubliekVerenigingDetailResponse GetResponse(FullBlownApiSetup setup)
        => setup.PublicApiHost.GetPubliekDetailWithHeader(setup.SuperAdminHttpClient, _testContext.VCode).GetAwaiter().GetResult();

    [Fact]
    public void With_Context()
    {
        Response.Context.ShouldCompare("http://127.0.0.1:11003/v1/contexten/publiek/detail-vereniging-context.json");
    }

    [Fact]
    public void JsonContentMatches()
    {
        var expected = new Verenigingssubtype()
        {
            Code = VerenigingssubtypeCode.FeitelijkeVereniging.Code,
            Naam = VerenigingssubtypeCode.FeitelijkeVereniging.Naam
        };

        Response.Vereniging.Verenigingssubtype.Should().BeEquivalentTo(expected);
    }
}
