namespace AssociationRegistry.Test.E2E.When_SubtypeWerdGewijzgid.Publiek.Detail.With_Header;

using FluentAssertions;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.TestClasses;
using KellermanSoftware.CompareNetObjects;
using Public.Api.Verenigingen.Detail.ResponseModels;
using Vereniging;
using Xunit;
using SubverenigingVan = Public.Api.Verenigingen.Detail.ResponseModels.SubverenigingVan;
using Verenigingssubtype = Public.Api.Verenigingen.Detail.ResponseModels.Verenigingssubtype;

[Collection(nameof(WhenSubtypeWerdGewijzigdCollection))]
public class Returns_Detail : End2EndTest<PubliekVerenigingDetailResponse>
{
    private readonly WhenSubtypeWerdGewijzigdContext _testContext;

    public Returns_Detail(WhenSubtypeWerdGewijzigdContext testContext) : base(testContext.ApiSetup)
    {
        _testContext = testContext;
    }

    public override PubliekVerenigingDetailResponse GetResponse(FullBlownApiSetup setup)
        => setup.PublicApiHost.GetPubliekDetailWithHeader(setup.SuperAdminHttpClient, _testContext.CommandResult.VCode,
                                                          _testContext.CommandResult.Sequence).GetAwaiter().GetResult();

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
            Code = VerenigingssubtypeCode.Subvereniging.Code,
            Naam = VerenigingssubtypeCode.Subvereniging.Naam
        };

        Response.Vereniging.Verenigingssubtype.Should().BeEquivalentTo(expected);

        Response.Vereniging.SubverenigingVan.Should().BeEquivalentTo(new SubverenigingVan()
        {
            Naam = _testContext.Scenario.BaseScenario.VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.Naam,
            AndereVereniging = _testContext.CommandRequest.AndereVereniging!,
            Beschrijving = _testContext.CommandRequest.Beschrijving!,
            Identificatie = _testContext.CommandRequest.Identificatie!,
        });
    }
}
