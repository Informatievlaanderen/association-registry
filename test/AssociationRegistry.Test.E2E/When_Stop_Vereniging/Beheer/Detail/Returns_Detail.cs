namespace AssociationRegistry.Test.E2E.When_Stop_Vereniging.Beheer.Detail;

using Admin.Api.WebApi.Verenigingen.Detail.ResponseModels;
using DecentraalBeheer.Vereniging;
using FluentAssertions;
using Formats;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.TestClasses;
using Vereniging;
using Xunit;

[Collection(nameof(StopVerenigingCollection))]
public class Returns_Detail : End2EndTest<DetailVerenigingResponse>
{
    private readonly StopVerenigingContext _testContext;

    public Returns_Detail(StopVerenigingContext testContext) : base(testContext.ApiSetup)
    {
        _testContext = testContext;
    }

    public override async Task<DetailVerenigingResponse> GetResponse(FullBlownApiSetup setup)
        => await setup.AdminApiHost.GetBeheerDetail(setup.AdminHttpClient ,_testContext.CommandResult.VCode, headers: new RequestParameters().WithExpectedSequence(_testContext.CommandResult.Sequence));

    [Fact]
    public void JsonContentMatches()
    {
       Response.Vereniging.Einddatum.Should().BeEquivalentTo(_testContext.CommandRequest.Einddatum.FormatAsBelgianDate());
       Response.Vereniging.Status.Should().BeEquivalentTo(VerenigingStatus.Gestopt.StatusNaam);
    }
}
