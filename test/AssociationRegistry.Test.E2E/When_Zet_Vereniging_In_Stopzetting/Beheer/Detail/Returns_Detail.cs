namespace AssociationRegistry.Test.E2E.When_Zet_Vereniging_In_Stopzetting.Beheer.Detail;

using Admin.Api.WebApi.Verenigingen.Detail.ResponseModels;
using DecentraalBeheer.Vereniging;
using FluentAssertions;
using Formats;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.TestClasses;
using Xunit;

[Collection(nameof(ZetVerenigingInStopzettingCollection))]
public class Returns_Detail : End2EndTest<DetailVerenigingResponse>
{
    private readonly ZetVerenigingInStopzettingContext _testInStopzettingContext;

    public Returns_Detail(ZetVerenigingInStopzettingContext testInStopzettingContext)
        : base(testInStopzettingContext.ApiSetup)
    {
        _testInStopzettingContext = testInStopzettingContext;
    }

    public override async Task<DetailVerenigingResponse> GetResponse(FullBlownApiSetup setup) =>
        await setup.AdminApiHost.GetBeheerDetail(
            setup.AdminHttpClient,
            _testInStopzettingContext.CommandResult.VCode,
            headers: new RequestParameters().WithExpectedSequence(_testInStopzettingContext.CommandResult.Sequence)
        );

    [Fact]
    public void JsonContentMatches()
    {
        Response.Vereniging.InStopzetting.Should().BeTrue();
    }
}
