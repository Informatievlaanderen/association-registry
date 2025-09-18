namespace AssociationRegistry.Test.Basisgegevens.When_Wijzig_Startdatum;

using AutoFixture;
using AutoFixture.Kernel;
using Common.AutoFixture;
using DecentraalBeheer.Vereniging;
using DecentraalBeheer.Vereniging.Exceptions;
using Events;
using Events.Factories;
using FluentAssertions;
using Framework;
using Xunit;

public class Given_Vereniging_Is_Gestopt
{
    [Theory]
    [InlineData(typeof(FeitelijkeVerenigingWerdGeregistreerd))]
    [InlineData(typeof(VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd))]
    public void Then_It_Throws_If_Startdatum_Is_After_Einddatum2(Type verenigingType)
    {
        var fixture = new Fixture().CustomizeDomain();
        var context = new SpecimenContext(fixture);

        var vereniging = new Vereniging();

        var verenigingWerdGeregistreerd = (IVerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd)context.Resolve(verenigingType);

        var einddatum = verenigingWerdGeregistreerd.Startdatum.Value.AddDays(10);
        var verenigingWerdGestopt = EventFactory.VerenigingWerdGestopt(Datum.Hydrate(einddatum));

        vereniging.Hydrate(
            new VerenigingState()
               .Apply((dynamic)verenigingWerdGeregistreerd)
               .Apply(verenigingWerdGestopt));

        var clockStub = new ClockStub(verenigingWerdGestopt.Einddatum.AddDays(20));
        var nieuweStartDatum = verenigingWerdGestopt.Einddatum.AddDays(10);

        var wijzigStartdatum = () => vereniging.WijzigStartdatum(Datum.Hydrate(nieuweStartDatum), clockStub);

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
