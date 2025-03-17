namespace AssociationRegistry.Test.E2E.When_SubtypeWerdGewijzgid.Publiek.Detail.With_Header;

using AssociationRegistry.Admin.Api.Verenigingen.Subtype.RequestModels;
using AssociationRegistry.Public.Api;
using AssociationRegistry.Public.Api.Verenigingen.Detail.ResponseModels;
using AssociationRegistry.Test.E2E.Framework.AlbaHost;
using AssociationRegistry.Test.E2E.Framework.ApiSetup;
using AssociationRegistry.Test.E2E.Framework.TestClasses;
using FluentAssertions;
using KellermanSoftware.CompareNetObjects;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;
using Verenigingssubtype = Public.Api.Verenigingen.Detail.ResponseModels.Verenigingssubtype;

[Collection(FullBlownApiCollection.Name)]
public class Returns_Detail : End2EndTest<WhenSubtypeWerdGewijzigdContext, WijzigSubtypeRequest, PubliekVerenigingDetailResponse>
{
    private readonly WhenSubtypeWerdGewijzigdContext _context;

    public Returns_Detail(WhenSubtypeWerdGewijzigdContext context): base(context)
    {
        _context = context;
    }

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
            Code = AssociationRegistry.Vereniging.Verenigingssubtype.Subvereniging.Code,
            Naam = AssociationRegistry.Vereniging.Verenigingssubtype.Subvereniging.Naam
        };

        Response.Vereniging.Verenigingssubtype.Should().BeEquivalentTo(expected);

        Response.Vereniging.SubverenigingVan.Should().BeEquivalentTo(new SubverenigingVan()
        {
            Naam = _context.Scenario.BaseScenario.VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.Naam,
            AndereVereniging = _context.Request.AndereVereniging!,
            Beschrijving = _context.Request.Beschrijving!,
            Identificatie = _context.Request.Identificatie!,
        });
    }

    public override Func<IApiSetup, PubliekVerenigingDetailResponse> GetResponse
    {
        get { return setup =>
        {
            var logger = setup.PublicApiHost.Services.GetRequiredService<ILogger<Program>>();

            logger.LogInformation("EXECUTING GET REQUEST");

            return setup.PublicApiHost.GetPubliekDetailWithHeader(setup.SuperAdminHttpClient, TestContext.RequestResult.VCode,
                                                                  TestContext.RequestResult.Sequence).GetAwaiter().GetResult();
        }; }
    }
}
