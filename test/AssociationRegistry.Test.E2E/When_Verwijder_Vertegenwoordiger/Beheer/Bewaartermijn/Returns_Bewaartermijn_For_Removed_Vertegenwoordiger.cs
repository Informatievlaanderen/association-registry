namespace AssociationRegistry.Test.E2E.When_Verwijder_Vertegenwoordiger.Beheer.Bewaartermijn;

using Admin.Api.WebApi.Administratie.Bewaartermijnen;
using FluentAssertions;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.TestClasses;
using Xunit;

[Collection(nameof(VerwijderVertegenwoordigerCollection))]
public class Returns_Bewaartermijn_For_Removed_Vertegenwoordiger : End2EndTest<BewaartermijnResponse>
{
    private readonly VerwijderVertegenwoordigerContext _testContext;

    public Returns_Bewaartermijn_For_Removed_Vertegenwoordiger(VerwijderVertegenwoordigerContext testContext)
        : base(testContext.ApiSetup)
    {
        _testContext = testContext;
    }

    public override async Task<BewaartermijnResponse> GetResponse(FullBlownApiSetup setup)
        => await setup.AdminApiHost.GetBewaartermijn(setup.AdminHttpClient, _testContext.VCode, _testContext.Scenario.VertegenwoordigerWerdToegevoegd.VertegenwoordigerId, new RequestParameters().WithExpectedSequence(_testContext.CommandResult.Sequence));

    [Fact]
    public void JsonContentMatches()
    {
        Response.Should().Be(new BewaartermijnResponse());
    }
}
