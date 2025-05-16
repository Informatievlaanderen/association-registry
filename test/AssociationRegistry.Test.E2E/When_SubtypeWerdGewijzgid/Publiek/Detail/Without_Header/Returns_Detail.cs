namespace AssociationRegistry.Test.E2E.When_SubtypeWerdGewijzgid.Publiek.Detail.Without_Header;

using AssociationRegistry.Admin.Api.Verenigingen.Subtype.RequestModels;
using AssociationRegistry.Public.Api.Verenigingen.Detail.ResponseModels;
using AssociationRegistry.Test.E2E.Framework.AlbaHost;
using AssociationRegistry.Test.E2E.Framework.ApiSetup;
using AssociationRegistry.Test.E2E.Framework.TestClasses;
using FluentAssertions;
using KellermanSoftware.CompareNetObjects;
using Xunit;

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
        Response.Vereniging.Verenigingssubtype.Should().BeNull();
    }

    public override Func<IApiSetup, PubliekVerenigingDetailResponse> GetResponse
        => setup => setup.PublicApiHost.GetPubliekDetail(TestContext.RequestResult.VCode);
}
