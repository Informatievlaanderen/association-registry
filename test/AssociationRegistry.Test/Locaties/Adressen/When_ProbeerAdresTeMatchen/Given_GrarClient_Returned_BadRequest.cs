namespace AssociationRegistry.Test.Locaties.Adressen.When_ProbeerAdresTeMatchen;

using Events;
using Grar;
using Grar.Exceptions;
using Resources;
using AssociationRegistry.Test.Common.AutoFixture;
using Vereniging;
using AutoFixture;
using AutoFixture.Kernel;
using FluentAssertions;
using Grar.Clients;
using Moq;
using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_GrarClient_Returned_BadRequest
{
    [Theory]
    [InlineData(typeof(FeitelijkeVerenigingWerdGeregistreerd))]
    [InlineData(typeof(VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd))]
    public async Task Then_AdresKonNietOvergenomenWordenUitAdressenregister(Type verenigingType)
    {
        var fixture = new Fixture().CustomizeDomain();
        var context = new SpecimenContext(fixture);

        var grarClient = new Mock<IGrarClient>();
        var vereniging = new VerenigingOfAnyKind();

        var verenigingWerdGeregistreerd = (IVerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd)context.Resolve(verenigingType);

        grarClient.Setup(x => x.GetAddressMatches(
                             It.IsAny<string>(),
                             It.IsAny<string>(),
                             It.IsAny<string>(),
                             It.IsAny<string>(),
                             It.IsAny<string>(),
                             It.IsAny<CancellationToken>()))
                  .ThrowsAsync(new AdressenregisterReturnedNonSuccessStatusCode(HttpStatusCode.BadRequest));

        vereniging.Hydrate(
            new VerenigingState()
               .Apply((dynamic)verenigingWerdGeregistreerd));

        var locatie = verenigingWerdGeregistreerd.Locaties.First();

        await vereniging.ProbeerAdresTeMatchen(grarClient.Object, locatie.LocatieId,
                                               CancellationToken.None);

        var @event = vereniging.UncommittedEvents.OfType<AdresKonNietOvergenomenWordenUitAdressenregister>().SingleOrDefault();

        @event.Should().NotBeNull();
        @event.Adres.Should().BeEquivalentTo($"{locatie.Adres.Straatnaam} {locatie.Adres.Huisnummer}" +
                                             (!string.IsNullOrWhiteSpace(locatie.Adres.Busnummer)
                                                 ? $" bus {locatie.Adres.Busnummer}"
                                                 : string.Empty) +
                                             $", {locatie.Adres.Postcode} {locatie.Adres.Gemeente}, {locatie.Adres.Land}");
        @event!.Reden.Should().Be(ExceptionMessages.AdresKonNietOvergenomenWordenBadRequest);
    }
}
