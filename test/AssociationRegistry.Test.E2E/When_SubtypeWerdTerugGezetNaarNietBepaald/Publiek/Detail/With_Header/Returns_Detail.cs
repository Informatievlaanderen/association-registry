namespace AssociationRegistry.Test.E2E.When_SubtypeWerdTerugGezetNaarNietBepaald.Publiek.Detail.With_Header;

using FluentAssertions;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.TestClasses;
using KellermanSoftware.CompareNetObjects;
using Public.Api.Verenigingen.Detail.ResponseModels;
using Vereniging;
using Xunit;
using Verenigingssubtype = Public.Api.Verenigingen.Detail.ResponseModels.Verenigingssubtype;

[Collection(nameof(ZetSubtypeNaarNietBepaaldCollection))]
public class Returns_Detail : End2EndTest<PubliekVerenigingDetailResponse>
{
    private readonly ZetSubtypeNaarNietBepaaldContext _testContext;

    public Returns_Detail(ZetSubtypeNaarNietBepaaldContext testContext) : base(testContext.ApiSetup)
    {
        _testContext = testContext;
    }

    public override PubliekVerenigingDetailResponse GetResponse(FullBlownApiSetup setup)
        => setup.PublicApiHost.GetPubliekDetailWithHeader(setup.AdminHttpClient, _testContext.VCode).GetAwaiter().GetResult();

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
            Code = VerenigingssubtypeCode.NietBepaald.Code,
            Naam = VerenigingssubtypeCode.NietBepaald.Naam
        };

        Response.Vereniging.Verenigingssubtype.Should().BeEquivalentTo(expected);
    }
}
