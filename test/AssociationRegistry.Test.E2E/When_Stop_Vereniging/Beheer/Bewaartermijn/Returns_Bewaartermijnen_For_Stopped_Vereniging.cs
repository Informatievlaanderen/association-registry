namespace AssociationRegistry.Test.E2E.When_Stop_Vereniging.Beheer.Bewaartermijn;

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

[Collection(nameof(StopVerenigingCollection))]
public class Returns_Bewaartermijnen_For_Stopped_Vereniging : End2EndTest<BewaartermijnResponse[]>
{
    private readonly StopVerenigingContext _testContext;

    public Returns_Bewaartermijnen_For_Stopped_Vereniging(StopVerenigingContext testContext)
        : base(testContext.ApiSetup)
    {
        _testContext = testContext;
    }

    public override async Task<BewaartermijnResponse[]> GetResponse(FullBlownApiSetup setup)
    {
        var vCode = _testContext.VCode;

        var vertegenwoordigers = _testContext.Scenario.FeitelijkeVerenigingWerdGeregistreerd.Vertegenwoordigers;

        var headers = new RequestParameters().WithExpectedSequence(_testContext.CommandResult.Sequence);

        var responses = new List<BewaartermijnResponse>();

        foreach (var v in vertegenwoordigers)
        {
            var response = await setup.AdminApiHost.GetBewaartermijn(
                setup.SuperAdminHttpClient,
                vCode,
                v.VertegenwoordigerId,
                headers
            );

            responses.Add(response);
        }

        return responses.ToArray();
    }

    [Fact]
    public void JsonContentMatches()
    {
        Response.Should().NotBeEmpty();
        var options = new BewaartermijnOptions();
        var tolerance = Duration.FromSeconds(5);
        var now = SystemClock.Instance.GetCurrentInstant();
        var expectedVervaldag = now.PlusTicks(options.Duration.Ticks);

        foreach (var bewaartermijn in Response)
        {
            var expectedId =
                $"{BewaartermijnId.BewaartermijnAggregateName}-{_testContext.VCode}-{PersoonsgegevensType.Vertegenwoordigers.Value}-{bewaartermijn.EntityId}";

            bewaartermijn.BewaartermijnId.Should().Be(expectedId);
            bewaartermijn.VCode.Should().Be(_testContext.VCode.ToString());
            bewaartermijn.PersoonsgegevensType.Should().Be(PersoonsgegevensType.Vertegenwoordigers.Value);
            bewaartermijn.Reden.Should().Be(BewaartermijnReden.VerenigingWerdGestopt);
            bewaartermijn.Status.Should().Be(BewaartermijnStatus.StatusGepland.Naam);
            bewaartermijn.Vervaldag.Should().Match<Instant>(actual => (actual - expectedVervaldag) <= tolerance);

            bewaartermijn
                .Gebeurtenissen.Should()
                .ContainSingle()
                .Which.Status.Should()
                .Be(BewaartermijnStatus.StatusGepland.Naam);
        }
    }
}
