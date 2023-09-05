namespace AssociationRegistry.Test.When_Stopping_A_Vereniging;

using AutoFixture;
using Events;
using FluentAssertions;
using Framework;
using Framework.Customizations;
using Vereniging;
using Xunit;

public class Given_An_Empty_Einddatum
{
    [Fact]
    public void Then_It_Throws()
    {
        var fixture = new Fixture().CustomizeDomain();

        var vereniging = new Vereniging();
        vereniging.Hydrate(new VerenigingState().Apply(fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>()));

        var clock = new ClockStub(fixture.Create<DateTime>());
        var einddatum = Datum.Leeg;

        var stopVereniging = () => vereniging.Stop(einddatum, clock);

        stopVereniging.Should().Throw<ArgumentNullException>();
    }
}
