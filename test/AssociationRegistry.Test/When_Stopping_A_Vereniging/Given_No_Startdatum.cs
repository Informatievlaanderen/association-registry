namespace AssociationRegistry.Test.When_Stopping_A_Vereniging;

using AutoFixture;
using Common.AutoFixture;
using Events;
using FluentAssertions;
using Framework;
using Framework.Customizations;
using Vereniging;
using Xunit;

public class Given_No_Startdatum
{
    [Fact]
    public void Then_It_Adds_A_VerenigingWerdGestopt_Event()
    {
        var fixture = new Fixture().CustomizeDomain();

        var vereniging = new Vereniging();

        vereniging.Hydrate(new VerenigingState().Apply(fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>()
                                                           with
                                                           {
                                                               Startdatum = null,
                                                           }));

        var clock = new ClockStub(fixture.Create<DateTime>());
        var einddatum = Datum.Create(clock.Today);

        vereniging.Stop(einddatum, clock);

        vereniging.UncommittedEvents.Should().BeEquivalentTo(new[]
        {
            new VerenigingWerdGestopt(einddatum.Value),
        });
    }
}
