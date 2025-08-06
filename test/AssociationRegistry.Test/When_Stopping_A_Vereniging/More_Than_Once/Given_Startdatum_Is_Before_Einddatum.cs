namespace AssociationRegistry.Test.When_Stopping_A_Vereniging.More_Than_Once;

using AutoFixture;
using Common.AutoFixture;
using DecentraalBeheer.Vereniging;
using Events;
using FluentAssertions;
using Framework;
using Vereniging;
using Xunit;

public class Given_Startdatum_Is_Before_Einddatum
{
    private static ClockStub _clock;
    private static Datum _einddatum;

    [Theory]
    [MemberData(nameof(Data))]
    public void Then_It_Adds_A_EinddatumWerdGewijzigd_Event(VerenigingState state)
    {
        var fixture = new Fixture().CustomizeDomain();

        var vereniging = new Vereniging();

        vereniging.Hydrate(state);

        vereniging.Stop(_einddatum, _clock);

        vereniging.UncommittedEvents.Should().BeEquivalentTo(new[]
        {
            new EinddatumWerdGewijzigd(_einddatum.Value),
        });
    }

    public static IEnumerable<object[]> Data
    {
        get
        {
            var fixture = new Fixture().CustomizeDomain();
            _clock = new ClockStub(fixture.Create<DateTime>());
            _einddatum = Datum.Create(_clock.Today);
            return new List<object[]>
            {
                new object[]
                {
                    new VerenigingState().Apply(fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>()
                                                    with
                                                    {
                                                        Startdatum = null,
                                                    })
                                         .Apply(fixture.Create<VerenigingWerdGestopt>()),
                },
                new object[]
                {
                    new VerenigingState().Apply(fixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>()
                                                    with
                                                    {
                                                        Startdatum = null,
                                                    })
                                         .Apply(fixture.Create<VerenigingWerdGestopt>()),
                },
            };
        }
    }
}
