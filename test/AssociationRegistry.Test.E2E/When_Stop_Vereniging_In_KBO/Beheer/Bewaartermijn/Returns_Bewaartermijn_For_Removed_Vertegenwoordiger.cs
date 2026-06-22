namespace AssociationRegistry.Test.E2E.When_Stop_Vereniging_In_KBO.Beheer.Bewaartermijn;

using AssociationRegistry.Admin.Api.WebApi.Administratie.Bewaartermijnen;
using AssociationRegistry.Admin.Schema.Bewaartermijn;
using AssociationRegistry.DecentraalBeheer.Vereniging.Bewaartermijnen;
using AssociationRegistry.Integrations.Grar.Bewaartermijnen;
using AssociationRegistry.Test.E2E.Framework.AlbaHost;
using AssociationRegistry.Test.E2E.Framework.ApiSetup;
using AssociationRegistry.Test.E2E.Framework.TestClasses;
using FluentAssertions;
using NodaTime;
using Xunit;

[Collection(nameof(StopVerenigingInKBOCollection))]
public class Returns_Bewaartermijn_For_Removed_Vertegenwoordiger : End2EndTest<BewaartermijnResponse[]>
{
    private readonly StopVerenigingInKBOContext _testContext;

    public Returns_Bewaartermijn_For_Removed_Vertegenwoordiger(StopVerenigingInKBOContext testContext)
        : base(testContext.ApiSetup)
    {
        _testContext = testContext;
    }

    public override async Task<BewaartermijnResponse[]> GetResponse(FullBlownApiSetup setup)
    {
        var vCode = _testContext.VCode;

        int[] vertegenwoordigers =
        [
            _testContext.Scenario.VertegenwoordigerWerdToegevoegdVanuitKBO1.VertegenwoordigerId,
            _testContext.Scenario.VertegenwoordigerWerdToegevoegdVanuitKBO2.VertegenwoordigerId,
        ];

        var headers = new RequestParameters().WithExpectedSequence(_testContext.CommandResult.Sequence);

        var responses = new List<BewaartermijnResponse>();

        foreach (var vertegenwoordigerId in vertegenwoordigers)
        {
            var response = await setup.AdminApiHost.GetBewaartermijn(
                setup.SuperAdminHttpClient,
                vCode,
                vertegenwoordigerId,
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
        Response.Length.Should().Be(2);

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
            bewaartermijn.Reden.Should().Be(BewaartermijnReden.VerenigingWerdGestoptInKbo);
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
