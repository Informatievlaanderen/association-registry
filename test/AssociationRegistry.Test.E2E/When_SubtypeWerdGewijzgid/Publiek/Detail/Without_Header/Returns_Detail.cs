namespace AssociationRegistry.Test.E2E.When_SubtypeWerdGewijzgid.Publiek.Detail.Without_Header;

using FluentAssertions;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.TestClasses;
using KellermanSoftware.CompareNetObjects;
using Public.Api.Verenigingen.Detail.ResponseModels;
using Xunit;

[Collection(nameof(WhenSubtypeWerdGewijzigdCollection))]
public class Returns_Detail : End2EndTest<PubliekVerenigingDetailResponse>
{
    private readonly WhenSubtypeWerdGewijzigdContext _testContext;

    public Returns_Detail(WhenSubtypeWerdGewijzigdContext testContext) : base(testContext.ApiSetup)
    {
        _testContext = testContext;
    }

    public override PubliekVerenigingDetailResponse GetResponse(FullBlownApiSetup setup)
        =>  setup.PublicApiHost.GetPubliekDetail(_testContext.CommandResult.VCode);

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
}
