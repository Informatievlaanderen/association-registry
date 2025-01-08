namespace AssociationRegistry.Test.Basisgegevens.When_Wijzig_Startdatum;

using AssociationRegistry.Events;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Framework;
using AssociationRegistry.Vereniging;
using AssociationRegistry.Vereniging.Exceptions;
using AutoFixture;
using EventFactories;
using FluentAssertions;
using Xunit;

public class Given_Vereniging_Is_Gestopt
{
    [Fact]
    public void Then_It_Throws_If_Startdatum_Is_After_Einddatum()
    {
        var fixture = new Fixture().CustomizeDomain();

        var vereniging = new Vereniging();

        var einddatum = fixture.Create<Datum>();
        var feitelijkeVerenigingWerdGeregistreerd = fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>();
        var verenigingWerdGestopt = EventFactory.VerenigingWerdGestopt(einddatum);

        vereniging.Hydrate(
            new VerenigingState()
               .Apply(feitelijkeVerenigingWerdGeregistreerd)
               .Apply(verenigingWerdGestopt));

        var clock = new ClockStub(einddatum.Value.AddDays(100));

        var startdatum = Datum.Hydrate(einddatum.Value.AddDays(1));
        var wijzigStartdatum = () => vereniging.WijzigStartdatum(startdatum, clock);

        wijzigStartdatum.Should().Throw<StartdatumLigtNaEinddatum>();
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
