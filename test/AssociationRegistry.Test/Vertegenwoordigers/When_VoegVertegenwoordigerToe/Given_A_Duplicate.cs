namespace AssociationRegistry.Test.Vertegenwoordigers.When_VoegVertegenwoordigerToe;

using AssociationRegistry.Events;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Vereniging;
using AutoFixture;
using AutoFixture.Kernel;
using DecentraalBeheer.Vereniging;
using DecentraalBeheer.Vereniging.Exceptions;
using Moq;
using Persoonsgegevens;
using Xunit;

public class Given_A_Duplicate
{
    [Theory]
    [InlineData(typeof(FeitelijkeVerenigingWerdGeregistreerd))]
    [InlineData(typeof(VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd))]
    public void Then_it_throws(Type verenigingType)
    {
        var fixture = new Fixture().CustomizeDomain();
        var context = new SpecimenContext(fixture);
        var verenigingWerdGeregistreerd = (IVerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd)context.Resolve(verenigingType);

        var vereniging = new Vereniging();

        vereniging.Hydrate(new VerenigingState()
                              .Apply((dynamic)verenigingWerdGeregistreerd));

        var toeTeVoegenVertegenwoordiger = fixture.Create<Vertegenwoordiger>() with { Insz = Insz.Create(verenigingWerdGeregistreerd.Vertegenwoordigers.First().Insz) };
        Assert.ThrowsAsync<InszMoetUniekZijn>(() => vereniging.VoegVertegenwoordigerToe(toeTeVoegenVertegenwoordiger, Mock.Of<IVertegenwoordigerPersoonsgegevensRepository>()));
    }
}
