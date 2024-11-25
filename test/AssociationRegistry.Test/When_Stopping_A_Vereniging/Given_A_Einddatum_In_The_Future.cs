namespace AssociationRegistry.Test.When_Stopping_A_Vereniging;

using AutoFixture;
using Common.AutoFixture;
using Events;
using FluentAssertions;
using Framework;
using Framework.Customizations;
using Vereniging;
using Vereniging.Exceptions;
using Xunit;

public class Given_A_Einddatum_In_The_Future
{
    [Fact]
    public void Then_It_Throws()
    {
        var fixture = new Fixture().CustomizeDomain();
        var clock = new ClockStub(fixture.Create<DateTime>());

        var vereniging = new Vereniging();

        vereniging.Hydrate(new VerenigingState().Apply(fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>() with
        {
            Startdatum = clock.Today.AddDays(-1),
        }));

        var einddatum = Datum.Create(clock.Today.AddDays(1));

        var stopVereniging = () => vereniging.Stop(einddatum, clock);

        stopVereniging.Should().Throw<EinddatumMagNietInToekomstZijn>();
    }
}
