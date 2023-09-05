namespace AssociationRegistry.Test.When_Wijzig_Startdatum;

using AutoFixture;
using Events;
using FluentAssertions;
using Framework;
using Framework.Customizations;
using Vereniging;
using Vereniging.Exceptions;
using Xunit;

public class Given_Vereniging_Is_Gestopt
{
    [Fact]
    public void Then_It_Throws_If_Startdatum_Is_After_Einddatum()
    {
        var fixture = new Fixture().CustomizeDomain();

        var einddatum = fixture.Create<Datum>();

        var vereniging = new Vereniging();
        vereniging.Hydrate(
            new VerenigingState()
               .Apply(fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>())
               .Apply(VerenigingWerdGestopt.With(einddatum)));

        var clock = new ClockStub(einddatum.ValueOrThrow.AddDays(100));

        var startdatum = Datum.Hydrate(einddatum.ValueOrThrow.AddDays(1));
        var wijzigStartdatum = () => vereniging.WijzigStartdatum(startdatum, clock);

        wijzigStartdatum.Should().Throw<EinddatumIsBeforeStartdatum>();
    }

    [Fact]
    public void Then_It_Has_Events_If_Startdatum_Is_Before_Einddatum()
    {
    }

    [Fact]
    public void Then_It_Has_Events_If_Startdatum_Is_Equal_To_Einddatum()
    {
    }
}
