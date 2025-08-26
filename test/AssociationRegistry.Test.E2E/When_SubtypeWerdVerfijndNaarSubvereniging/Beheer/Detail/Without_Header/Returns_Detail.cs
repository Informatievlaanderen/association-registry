namespace AssociationRegistry.Test.E2E.When_SubtypeWerdVerfijndNaarSubvereniging.Beheer.Detail.Without_Header;

using Admin.Api.WebApi.Verenigingen.Detail.ResponseModels;
using FluentAssertions;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.TestClasses;
using KellermanSoftware.CompareNetObjects;
using Xunit;

[Collection(nameof(VerfijnSubtypeNaarSubverenigingCollection))]
public class Returns_Detail : End2EndTest<DetailVerenigingResponse>
{
    private readonly VerfijnSubtypeNaarSubverenigingContext _testContext;

    public Returns_Detail(VerfijnSubtypeNaarSubverenigingContext testContext) : base(testContext.ApiSetup)
    {
        _testContext = testContext;
    }

    public override async Task<DetailVerenigingResponse> GetResponse(FullBlownApiSetup setup)
        => await setup.AdminApiHost.GetBeheerDetail(setup.SuperAdminHttpClient, _testContext.CommandResult.VCode, headers: new RequestParameters().WithExpectedSequence(
                                                  _testContext.CommandResult.Sequence));

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
}
