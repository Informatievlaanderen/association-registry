namespace AssociationRegistry.Test.Basisgegevens.When_Wijzig_Startdatum;

using AssociationRegistry.Events;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Framework;
using AssociationRegistry.Vereniging;
using AssociationRegistry.Vereniging.Exceptions;
using AutoFixture;
using AutoFixture.Kernel;
using EventFactories;
using FluentAssertions;
using Xunit;

public class Given_Vereniging_Is_Gestopt
{
    [Theory]
    [InlineData(typeof(FeitelijkeVerenigingWerdGeregistreerd))]
    [InlineData(typeof(VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd))]
    public void Then_It_Throws_If_Startdatum_Is_After_Einddatum(Type verenigingType)
    {
        var fixture = new Fixture().CustomizeDomain();
        var context = new SpecimenContext(fixture);

        var vereniging = new Vereniging();

        var einddatum = fixture.Create<Datum>();
        var verenigingWerdGeregistreerd = (IVerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd)context.Resolve(verenigingType);
        var verenigingWerdGestopt = EventFactory.VerenigingWerdGestopt(einddatum);

        vereniging.Hydrate(
            new VerenigingState()
               .Apply((dynamic)verenigingWerdGeregistreerd)
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
