namespace AssociationRegistry.Test.When_Stopping_A_Vereniging;

using AutoFixture;
using Common.AutoFixture;
using DecentraalBeheer.Vereniging;
using DecentraalBeheer.Vereniging.Exceptions;
using Events;
using FluentAssertions;
using Framework;
using Xunit;

public class Given_A_Einddatum_In_The_Future
{
    private static ClockStub _clock;
    private static Datum _einddatum;

    [Theory]
    [MemberData(nameof(Data))]
    public void Then_It_Throws(VerenigingState state)
    {
        var vereniging = new Vereniging();

        vereniging.Hydrate(state);

        var stopVereniging = () => vereniging.Stop(_einddatum, _clock);

        stopVereniging.Should().Throw<EinddatumMagNietInToekomstZijn>();
    }

    public static IEnumerable<object[]> Data
    {
        get
        {
            var fixture = new Fixture().CustomizeDomain();
            _clock = new ClockStub(fixture.Create<DateTime>());
            _einddatum = Datum.Create(_clock.Today.AddDays(1));
            return new List<object[]>
            {
                new object[]
                {
                    new VerenigingState().Apply(fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>() with
                    {
                        Startdatum = _clock.Today.AddDays(-1),
                    }),
                },
                new object[]
                {
                    new VerenigingState().Apply(fixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>() with
                    {
                        Startdatum = _clock.Today.AddDays(-1),
                    }),
                },
            };
        }
    }
}
