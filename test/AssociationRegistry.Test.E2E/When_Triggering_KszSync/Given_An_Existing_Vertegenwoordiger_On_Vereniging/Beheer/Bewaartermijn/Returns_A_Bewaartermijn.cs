namespace AssociationRegistry.Test.E2E.When_Triggering_KszSync.Given_An_Existing_Vertegenwoordiger_On_Vereniging.Beheer.Bewaartermijn;

using Admin.Api.WebApi.Administratie.Bewaartermijnen;
using Admin.Schema.Bewaartermijn;
using DecentraalBeheer.Vereniging.Bewaartermijnen;
using FluentAssertions;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.TestClasses;
using Integrations.Grar.Bewaartermijnen;
using NodaTime;
using Xunit;

[Collection(nameof(ForExistingVertegenwoordigerCollection))]
public class Returns_A_Bewaartermijn : End2EndTest<BewaartermijnResponse>
{
    private readonly TriggerKszSyncForExistingVertegenwoordigerContext _context;

    public Returns_A_Bewaartermijn(TriggerKszSyncForExistingVertegenwoordigerContext context)
        : base(context.ApiSetup)
    {
        _context = context;
    }

    public override async Task<BewaartermijnResponse> GetResponse(FullBlownApiSetup setup) =>
        await setup.AdminApiHost.GetBewaartermijn(
            setup.SuperAdminHttpClient,
            _context.VCode,
            _context.Scenario.Vertegenwoordiger.VertegenwoordigerId
        );
// Deze test zou moeten groendraaien
// Daarna nog een test toevoegen zodat alle events afgechecked zijn
// Admin api listenTo, admin host publish msg
// allemaal includen, geen idee of handler moet included zijn op host, indien wel reference leggen en mssges verhuizen zou ik zeggen


[Fact]
    public void JsonContentMatches()
    {
        var expectedId =
            $"{BewaartermijnId.BewaartermijnAggregateName}-{_context.VCode}-{PersoonsgegevensType.Vertegenwoordigers.Value}-{_context.Scenario.Vertegenwoordiger.VertegenwoordigerId}";
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
                    VCode = _context.VCode.ToString(),
                    BewaartermijnType = PersoonsgegevensType.Vertegenwoordigers.Value,
                    EntityId = _context.Scenario.Vertegenwoordiger.VertegenwoordigerId,
                    Reden = BewaartermijnReden.KszSyncHeeftVertegenwoordigerAangeduidAlsOverleden,
                    Status = BewaartermijnStatus.StatusGepland.Naam,
                    Gebeurtenissen = new object[] { new { Status = BewaartermijnStatus.StatusGepland.Naam } },
                },
                cfg => cfg.ExcludingMissingMembers()
            );

        Response.Vervaldag.Should().Match<Instant>(actual => (actual - expectedVervaldag) <= tolerance);
        Response.Gebeurtenissen[0].Tijdstip.Should().Match<Instant>(actual => (actual - now) <= tolerance);
    }
}
