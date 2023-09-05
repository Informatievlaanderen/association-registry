namespace AssociationRegistry.Test.When_Stopping_A_Vereniging;

using AutoFixture;
using Events;
using Framework;
using Framework.Customizations;
using Vereniging;
using Vereniging.Einddatum;
using Vereniging.Einddatum.Exceptions;
using Xunit;

public class Given_A_Einddatum_In_The_Future
{
    [Fact]
    public void Then_It_Throws()
    {
        var fixture = new Fixture().CustomizeDomain();

        var vereniging = new Vereniging();
        vereniging.Hydrate(new VerenigingState().Apply(fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>()));

        var clock = new ClockStub(fixture.Create<DateTime>());
        var einddatum = Einddatum.Create(clock.Today.AddDays(1));

        var stopVereniging = () => vereniging.Stop(einddatum, clock);

        stopVereniging.Should().Throw<EinddatumIsInFuture>();
    }
}
