namespace AssociationRegistry.Test.E2E.When_SubtypeWerdGewijzgid.Beheer.Detail.With_Header;

using Admin.Api.Verenigingen.Detail.ResponseModels;
using FluentAssertions;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.TestClasses;
using KellermanSoftware.CompareNetObjects;
using Vereniging;
using Xunit;
using SubverenigingVan = Admin.Api.Verenigingen.Detail.ResponseModels.SubverenigingVan;
using Verenigingssubtype = Admin.Api.Verenigingen.Detail.ResponseModels.Verenigingssubtype;

[Collection(nameof(WhenSubtypeWerdGewijzigdCollection))]
public class Returns_Detail : End2EndTest<DetailVerenigingResponse>
{
    private readonly WhenSubtypeWerdGewijzigdContext _testContext;

    public Returns_Detail(WhenSubtypeWerdGewijzigdContext testContext) : base(testContext.ApiSetup)
    {
        _testContext = testContext;
    }

    public override DetailVerenigingResponse GetResponse(FullBlownApiSetup setup)
        => setup.AdminApiHost.GetBeheerDetailWithHeader(setup.SuperAdminHttpClient, _testContext.CommandResult.VCode,
                                                  _testContext.CommandResult.Sequence)
                       .GetAwaiter().GetResult();
    [Fact]
    public void With_testContext()
    {
        Response.Context.ShouldCompare("http://127.0.0.1:11003/v1/contexten/beheer/detail-vereniging-context.json");
    }

    [Fact]
    public void Subtype_Is_Subvereniging()
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
