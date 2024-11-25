namespace AssociationRegistry.Test.When_Stopping_A_Vereniging.More_Than_Once;

using AutoFixture;
using Common.AutoFixture;
using Events;
using FluentAssertions;
using Framework;
using Framework.Customizations;
using Vereniging;
using Xunit;

public class Given_Einddatum_Is_The_Same
{
    [Fact]
    public void Then_It_Adds_A_EinddatumWerdGewijzigd_Event()
    {
        var fixture = new Fixture().CustomizeDomain();

        var vereniging = new Vereniging();

        var clock = new ClockStub(fixture.Create<DateTime>());
        var einddatum = Datum.Create(clock.Today);

        vereniging.Hydrate(
            new VerenigingState()
               .Apply(fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>()
                          with
                          {
                              Startdatum = null,
                          })
               .Apply(VerenigingWerdGestopt.With(einddatum)));

        vereniging.Stop(einddatum, clock);

        vereniging.UncommittedEvents.Should().BeEmpty();
    }
}
