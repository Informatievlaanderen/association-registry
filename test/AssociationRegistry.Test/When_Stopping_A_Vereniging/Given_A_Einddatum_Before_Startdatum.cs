namespace AssociationRegistry.Test.When_Stopping_A_Vereniging;

using AutoFixture;
using Common.AutoFixture;
using DecentraalBeheer.Vereniging;
using DecentraalBeheer.Vereniging.Exceptions;
using Events;
using FluentAssertions;
using Framework;
using Xunit;

public class Given_A_Einddatum_Before_Startdatum
{
    private static ClockStub _clock;
    private static Datum _einddatum;

    [Theory]
    [MemberData(nameof(Data))]
    public void Then_It_Throws(VerenigingState givenState)
    {
        var vereniging = new Vereniging();
        vereniging.Hydrate(givenState);

        var stopVereniging = () => vereniging.Stop(_einddatum, _clock);

        stopVereniging.Should().Throw<EinddatumLigtVoorStartdatum>();
    }

    public static IEnumerable<object[]> Data
    {
        get
        {
            var fixture = new Fixture().CustomizeDomain();
            var feitelijkeVerenigingWerdGeregistreerd = fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>();
            var verenigingZonderEigenRechtspersoonlijkheid = fixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>() with
            {
                Startdatum = feitelijkeVerenigingWerdGeregistreerd.Startdatum!.Value,
            };

            _clock = new ClockStub(feitelijkeVerenigingWerdGeregistreerd.Startdatum!.Value.AddDays(100));
            _einddatum = Datum.Create(feitelijkeVerenigingWerdGeregistreerd.Startdatum.Value.AddDays(-1));

            return new List<object[]>
            {
                new object[]
                {
                    new VerenigingState().Apply(feitelijkeVerenigingWerdGeregistreerd),
                },
                new object[]
                {
                    new VerenigingState().Apply(verenigingZonderEigenRechtspersoonlijkheid),
                },
            };
        }
    }
}
