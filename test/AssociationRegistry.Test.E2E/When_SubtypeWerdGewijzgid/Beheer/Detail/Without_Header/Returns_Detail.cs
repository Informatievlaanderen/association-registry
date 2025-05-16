namespace AssociationRegistry.Test.E2E.When_SubtypeWerdGewijzgid.Beheer.Detail.Without_Header;

using AssociationRegistry.Admin.Api.Verenigingen.Detail.ResponseModels;
using AssociationRegistry.Admin.Api.Verenigingen.Subtype.RequestModels;
using AssociationRegistry.Test.E2E.Framework.AlbaHost;
using AssociationRegistry.Test.E2E.Framework.ApiSetup;
using AssociationRegistry.Test.E2E.Framework.TestClasses;
using FluentAssertions;
using KellermanSoftware.CompareNetObjects;
using Xunit;

[Collection(FullBlownApiCollection.Name)]
public class Returns_Detail : End2EndTest<WhenSubtypeWerdGewijzigdContext, WijzigSubtypeRequest, DetailVerenigingResponse>
{
    private readonly WhenSubtypeWerdGewijzigdContext _context;

    public Returns_Detail(WhenSubtypeWerdGewijzigdContext context)
    {
        _context = context;
    }

    [Fact]
    public void With_Context()
    {
        Response.Context.ShouldCompare("http://127.0.0.1:11003/v1/contexten/beheer/detail-vereniging-context.json");
    }

    [Fact]
    public void JsonContentMatches()
    {
        Response.Vereniging.Verenigingssubtype.Should().BeNull();
    }

    public override Func<IApiSetup, DetailVerenigingResponse> GetResponse
        => setup => setup.AdminApiHost.GetBeheerDetail(TestContext.VCode);
}
