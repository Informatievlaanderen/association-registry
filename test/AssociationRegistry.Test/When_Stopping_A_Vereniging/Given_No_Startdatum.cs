namespace AssociationRegistry.Test.When_Stopping_A_Vereniging;

using AutoFixture;
using Events;
using FluentAssertions;
using Framework;
using Framework.Customizations;
using Vereniging;
using Vereniging.Exceptions;
using Xunit;

public class Given_No_Startdatum
{
    [Fact]
    public void Then_It_DoesNotThrows()
    {
        var fixture = new Fixture().CustomizeDomain();

        var vereniging = new Vereniging();
        var feitelijkeVerenigingWerdGeregistreerd = fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>() with
        {
            Startdatum = null,
        };
        vereniging.Hydrate(new VerenigingState().Apply(feitelijkeVerenigingWerdGeregistreerd));

        var clock = new ClockStub(fixture.Create<DateTime>());
        var einddatum = Datum.Create(clock.Today);

        var stopVereniging = () => vereniging.Stop(einddatum, clock);

        stopVereniging.Should().NotThrow<EinddatumIsBeforeStartdatum>();
    }
}
