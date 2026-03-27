namespace AssociationRegistry.Test.E2E.When_Verwijder_Vertegenwoordiger.Beheer.Bewaartermijn;

using Admin.Api.WebApi.Administratie.Bewaartermijnen;
using Admin.Schema.Bewaartermijn;
using CommandHandling.Bewaartermijnen.Acties.Start;
using DecentraalBeheer.Vereniging.Bewaartermijnen;
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

    public override async Task<BewaartermijnResponse> GetResponse(FullBlownApiSetup setup) =>
        await setup.AdminApiHost.GetBewaartermijn(
            setup.SuperAdminHttpClient,
            _testContext.VCode,
            _testContext.Scenario.VertegenwoordigerWerdToegevoegd.VertegenwoordigerId,
            new RequestParameters().WithExpectedSequence(_testContext.CommandResult.Sequence)
        );

    [Fact]
    public void JsonContentMatches()
    {
        var expectedId =
            $"{BewaartermijnId.BewaartermijnAggregateName}-{_testContext.VCode}-{PersoonsgegevensType.Vertegenwoordigers.Value}-{_testContext.Scenario.VertegenwoordigerWerdToegevoegd.VertegenwoordigerId}";
        var options = new BewaartermijnOptions();
        var tolerance = Duration.FromSeconds(5);

        var now = SystemClock.Instance.GetCurrentInstant();
        var expectedVervaldag = now.PlusTicks(options.Duration.Ticks);

        Response
            .Should()
            .BeEquivalentTo(
                new
                {
                    BewaartermijnId = expectedId,
                    VCode = _testContext.VCode.ToString(),
                    BewaartermijnType = PersoonsgegevensType.Vertegenwoordigers.Value,
                    EntityId = _testContext.Scenario.VertegenwoordigerWerdToegevoegd.VertegenwoordigerId,
                    Reden = BewaartermijnReden.VertegenwoordigerWerdVerwijderd,
                    Status = BewaartermijnStatus.StatusGepland.Naam,
                    Gebeurtenissen = new object[] { new { Status = BewaartermijnStatus.StatusGepland.Naam } },
                },
                cfg => cfg.ExcludingMissingMembers()
            );

        Response.Vervaldag.Should().Match<Instant>(actual => (actual - expectedVervaldag) <= tolerance);
        Response.Gebeurtenissen[0].Tijdstip.Should().Match<Instant>(actual => (actual - now) <= tolerance);
    }
}
