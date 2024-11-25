namespace AssociationRegistry.Test.When_Stopping_A_Vereniging;

using AutoFixture;
using Common.AutoFixture;
using Events;
using FluentAssertions;
using Framework;
using Framework.Customizations;
using Vereniging;
using Xunit;

public class Given_A_Einddatum_Is_Today
{
    [Fact]
    public void Then_It_Adds_A_VerenigingWerdGestopt_Event()
    {
        var fixture = new Fixture().CustomizeDomain();
        var clock = new ClockStub(fixture.Create<DateTime>());

        var vereniging = new Vereniging();

        vereniging.Hydrate(new VerenigingState().Apply(fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>()with
        {
            Startdatum = clock.Today.AddDays(-1),
        }));

        var einddatum = Datum.Create(clock.Today);

        vereniging.Stop(einddatum, clock);

        vereniging.UncommittedEvents.Should().BeEquivalentTo(new[]
        {
            new VerenigingWerdGestopt(einddatum.Value),
        });
    }
}
