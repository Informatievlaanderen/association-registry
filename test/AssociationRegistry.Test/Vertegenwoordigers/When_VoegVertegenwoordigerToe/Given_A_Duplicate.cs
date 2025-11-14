namespace AssociationRegistry.Test.Vertegenwoordigers.When_VoegVertegenwoordigerToe;

using AssociationRegistry.Events;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Vereniging;
using AutoFixture;
using AutoFixture.Kernel;
using Common.StubsMocksFakes.VertegenwoordigerPersoonsgegevensRepositories;
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
        var eersteGeregistreerdeVertegenwoordiger = verenigingWerdGeregistreerd.Vertegenwoordigers[0];
        var vertegenwoordigerPersoonsgegevensMock = new VertegenwoordigerPersoonsgegevensRepositoryMock();

        var vertegenwoordigerPersoonsgegevens = fixture.Create<VertegenwoordigerPersoonsgegevens>() with
        {
            RefId = eersteGeregistreerdeVertegenwoordiger.RefId,
        };
        vertegenwoordigerPersoonsgegevensMock.Save(vertegenwoordigerPersoonsgegevens);

        var vereniging = new Vereniging();

        vereniging.Hydrate(new VerenigingState(){VertegenwoordigerPersoonsgegevensRepository = vertegenwoordigerPersoonsgegevensMock}
                              .Apply((dynamic)verenigingWerdGeregistreerd));

        var toeTeVoegenVertegenwoordiger = fixture.Create<Vertegenwoordiger>() with { Insz = Insz.Create(vertegenwoordigerPersoonsgegevens.Insz) };
        Assert.ThrowsAsync<InszMoetUniekZijn>(() => vereniging.VoegVertegenwoordigerToe(toeTeVoegenVertegenwoordiger, Mock.Of<IVertegenwoordigerPersoonsgegevensRepository>()));
    }
}
