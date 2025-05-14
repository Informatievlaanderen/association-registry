namespace AssociationRegistry.Test.E2E.When_Stop_Vereniging.Beheer.Detail;

using Admin.Api.Verenigingen.Detail.ResponseModels;
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

    public override DetailVerenigingResponse GetResponse(FullBlownApiSetup setup)
        => setup.AdminApiHost.GetBeheerDetail(setup.AdminHttpClient ,_testContext.CommandResult.VCode, new RequestParameters().WithExpectedSequence(_testContext.CommandResult.Sequence)).GetAwaiter().GetResult();

    [Fact]
    public void JsonContentMatches()
    {
       Response.Vereniging.Einddatum.Should().BeEquivalentTo(_testContext.CommandRequest.Einddatum.FormatAsBelgianDate());
       Response.Vereniging.Status.Should().BeEquivalentTo(VerenigingStatus.Gestopt.StatusNaam);
    }
}
