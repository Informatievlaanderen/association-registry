namespace AssociationRegistry.Test.When_Stopping_A_Vereniging;

using AutoFixture;
using Common.AutoFixture;
using DecentraalBeheer.Vereniging;
using Events;
using FluentAssertions;
using Framework;
using Xunit;

public class Given_No_Startdatum
{
    private static ClockStub _clock;
    private static Datum _einddatum;

    [Theory]
    [MemberData(nameof(Data))]
    public void Then_It_Adds_A_VerenigingWerdGestopt_Event(VerenigingState state)
    {
        var vereniging = new Vereniging();

        vereniging.Hydrate(state);

        vereniging.Stop(_einddatum, _clock);

        vereniging.UncommittedEvents.Should().BeEquivalentTo(new[]
        {
            new VerenigingWerdGestopt(_einddatum.Value),
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
                                                    }),
                },
                new object[]
                {
                    new VerenigingState().Apply(fixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>()
                                                    with
                                                    {
                                                        Startdatum = null,
                                                    }),
                },
            };
        }
    }
}
