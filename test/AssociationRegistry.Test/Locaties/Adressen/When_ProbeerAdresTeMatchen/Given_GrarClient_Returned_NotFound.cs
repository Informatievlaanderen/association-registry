namespace AssociationRegistry.Test.Locaties.Adressen.When_ProbeerAdresTeMatchen;

using AssociationRegistry.Events;
using AssociationRegistry.Grar;
using AssociationRegistry.Grar.Exceptions;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Vereniging;
using AutoFixture;
using FluentAssertions;
using Moq;
using System.Net;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_GrarClient_Returned_NotFound
{
    [Fact]
    public async Task Then_AdresWerdNietGevondenInAdressenregister()
    {
        var fixture = new Fixture().CustomizeDomain();

        var grarClient = new Mock<IGrarClient>();
        var vereniging = new VerenigingOfAnyKind();

        var feitelijkeVerenigingWerdGeregistreerd = fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>();

        grarClient.Setup(x => x.GetAddressMatches(
                             It.IsAny<string>(),
                             It.IsAny<string>(),
                             It.IsAny<string>(),
                             It.IsAny<string>(),
                             It.IsAny<string>(),
                             It.IsAny<CancellationToken>()))
                  .ThrowsAsync(new AdressenregisterReturnedNonSuccessStatusCode(HttpStatusCode.NotFound));

        vereniging.Hydrate(
            new VerenigingState()
               .Apply(feitelijkeVerenigingWerdGeregistreerd));

        await vereniging.ProbeerAdresTeMatchen(grarClient.Object, feitelijkeVerenigingWerdGeregistreerd.Locaties.First().LocatieId,
                                               CancellationToken.None);

        var @event = vereniging.UncommittedEvents.OfType<AdresWerdNietGevondenInAdressenregister>().SingleOrDefault();

        @event.Should().NotBeNull();
    }
}
