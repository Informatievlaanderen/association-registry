namespace AssociationRegistry.Test.Locaties.Adressen.When_ProbeerAdresTeMatchen;

using Events;
using Grar;
using Grar.Models;
using AssociationRegistry.Test.Common.AutoFixture;
using Vereniging;
using AutoFixture;
using AutoFixture.Kernel;
using FluentAssertions;
using Grar.Clients;
using Moq;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_GrarClient_Returned_No_Matches
{
    [Theory]
    [InlineData(typeof(FeitelijkeVerenigingWerdGeregistreerd))]
    [InlineData(typeof(VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd))]
    public async Task Then_AdresWerdNietGevondenInAdressenregister(Type verenigingType)
    {
        var fixture = new Fixture().CustomizeDomain();
        var context = new SpecimenContext(fixture);


        var grarClient = new Mock<IGrarClient>();
        var vereniging = new VerenigingOfAnyKind();

        var verenigingWerdGeregistreerd = (IVerenigingWerdGeregistreerd)context.Resolve(verenigingType);

        grarClient.Setup(x => x.GetAddressMatches(
                             It.IsAny<string>(),
                             It.IsAny<string>(),
                             It.IsAny<string>(),
                             It.IsAny<string>(),
                             It.IsAny<string>(),
                             It.IsAny<CancellationToken>()))
                  .ReturnsAsync(new AdresMatchResponseCollection(new List<AddressMatchResponse>()));

        vereniging.Hydrate(
            new VerenigingState()
               .Apply((dynamic)verenigingWerdGeregistreerd));

        await vereniging.ProbeerAdresTeMatchen(grarClient.Object, verenigingWerdGeregistreerd.Locaties.First().LocatieId,
                                               CancellationToken.None);

        var @event = vereniging.UncommittedEvents.OfType<AdresWerdNietGevondenInAdressenregister>().SingleOrDefault();

        @event.Should().NotBeNull();
    }
}
