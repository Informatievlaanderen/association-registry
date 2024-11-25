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

public class Given_A_Einddatum_Before_Startdatum
{
    [Fact]
    public void Then_It_Throws()
    {
        var fixture = new Fixture().CustomizeDomain();

        var vereniging = new Vereniging();
        var feitelijkeVerenigingWerdGeregistreerd = fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>();
        vereniging.Hydrate(new VerenigingState().Apply(feitelijkeVerenigingWerdGeregistreerd));

        var clock = new ClockStub(feitelijkeVerenigingWerdGeregistreerd.Startdatum!.Value.AddDays(100));
        var einddatum = Datum.Create(feitelijkeVerenigingWerdGeregistreerd.Startdatum.Value.AddDays(-1));

        var stopVereniging = () => vereniging.Stop(einddatum, clock);

        stopVereniging.Should().Throw<EinddatumLigtVoorStartdatum>();
    }
}
