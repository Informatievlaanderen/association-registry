namespace AssociationRegistry.Test.E2E.When_Verwijder_Vertegenwoordiger.Beheer.Bewaartermijn;

using Admin.Api.WebApi.Administratie.Bewaartermijnen;
using FluentAssertions;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.TestClasses;
using ImTools;
using Integrations.Grar.Bewaartermijnen;
using NodaTime;
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
        => await setup.AdminApiHost.GetBewaartermijn(setup.SuperAdminHttpClient, _testContext.VCode, _testContext.Scenario.VertegenwoordigerWerdToegevoegd.VertegenwoordigerId, new RequestParameters().WithExpectedSequence(_testContext.CommandResult.Sequence));

    [Fact]
    public void JsonContentMatches()
    {
        var expectedId = $"{_testContext.VCode}-{_testContext.Scenario.VertegenwoordigerWerdToegevoegd.VertegenwoordigerId}";
        var options = new BewaartermijnOptions();
        var tolerance = Duration.FromSeconds(5);

        var now = SystemClock.Instance.GetCurrentInstant();
        var expectedVervaldag = now.PlusTicks(options.Duration.Ticks);

        Response.Should().BeEquivalentTo(new
        {
            Id = expectedId,
            VCode = _testContext.VCode.ToString(),
            VertegenwoordigerId = _testContext.Scenario.VertegenwoordigerWerdToegevoegd.VertegenwoordigerId
        }, cfg => cfg.ExcludingMissingMembers());

        Response.Vervaldag.Should().Match<Instant>(actual => (actual - expectedVervaldag) <= tolerance);
    }
}
