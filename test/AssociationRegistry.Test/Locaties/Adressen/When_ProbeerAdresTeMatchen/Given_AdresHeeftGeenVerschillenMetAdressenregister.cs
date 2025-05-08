namespace AssociationRegistry.Test.Locaties.Adressen.When_ProbeerAdresTeMatchen;

using AssociationRegistry.Grar;
using AssociationRegistry.Grar.Clients;
using AssociationRegistry.Grar.Models;
using Events;
using Formats;
using AssociationRegistry.Test.Common.AutoFixture;
using Vereniging;
using AutoFixture;
using AutoFixture.Kernel;
using FluentAssertions;
using Moq;
using Xunit;

public class Given_AdresHeeftGeenVerschillenMetAdressenregister
{
    private static Mock<IGrarClient> SetupGrarClientMock(Fixture fixture, Registratiedata.Locatie locatie)
    {
        var grarClient = new Mock<IGrarClient>();

        grarClient.Setup(x => x.GetAddressMatches(
                             It.IsAny<string>(),
                             It.IsAny<string>(),
                             It.IsAny<string>(),
                             It.IsAny<string>(),
                             It.IsAny<string>(),
                             CancellationToken.None))
                  .ReturnsAsync(new AdresMatchResponseCollection(new[]
                   {
                       fixture.Create<AddressMatchResponse>() with
                       {
                           Score = 100,
                           AdresId = locatie.AdresId,
                           Straatnaam = locatie.Adres.Straatnaam,
                           Adresvoorstelling = locatie.Adres.ToAdresString(),
                           Busnummer = locatie.Adres.Busnummer,
                           Gemeente = locatie.Adres.Gemeente,
                           Huisnummer = locatie.Adres.Huisnummer,
                           Postcode = locatie.Adres.Postcode,
                       },
                   }));

        return grarClient;
    }

    [Theory]
    [InlineData(typeof(FeitelijkeVerenigingWerdGeregistreerd))]
    [InlineData(typeof(VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd))]
    public void Then_AdresHeeftGeenVerschillenMetAdressenregister_IsApplied(Type verenigingType)
    {
        var fixture = new Fixture().CustomizeDomain();
        var context = new SpecimenContext(fixture);

        var verenigingWerdGeregistreerd = (IVerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd)context.Resolve(verenigingType);
        var locatie = verenigingWerdGeregistreerd.Locaties.First();

        var grarClient = SetupGrarClientMock(fixture, locatie);

        var vereniging = new VerenigingOfAnyKind();

        vereniging.Hydrate(
            new VerenigingState()
               .Apply((dynamic)verenigingWerdGeregistreerd));

        vereniging.ProbeerAdresTeMatchen(grarClient.Object, locatie.LocatieId,
                                          CancellationToken.None)
                   .GetAwaiter().GetResult();

        var overgenomenEvent = vereniging.UncommittedEvents.OfType<AdresHeeftGeenVerschillenMetAdressenregister>().SingleOrDefault();

        overgenomenEvent.Should().NotBeNull();
    }
}
