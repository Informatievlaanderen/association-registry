namespace AssociationRegistry.Test.When_Stopping_A_Vereniging.More_Than_Once;

using AutoFixture;
using Events;
using FluentAssertions;
using Framework;
using Framework.Customizations;
using Vereniging;
using Xunit;

public class Given_Startdatum_Is_Before_Einddatum
{
    [Fact]
    public void Then_It_Adds_A_EinddatumWerdGewijzigd_Event()
    {
        var fixture = new Fixture().CustomizeDomain();

        var vereniging = new Vereniging();

        vereniging.Hydrate(
            new VerenigingState()
               .Apply(fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>()
                          with
                          {
                              Startdatum = null,
                          })
               .Apply(fixture.Create<VerenigingWerdGestopt>()));

        var clock = new ClockStub(fixture.Create<DateTime>());
        var einddatum = Datum.Create(clock.Today);

        vereniging.Stop(einddatum, clock);

        vereniging.UncommittedEvents.Should().BeEquivalentTo(new[]
        {
            new EinddatumWerdGewijzigd(einddatum.Value),
        });
    }
}
