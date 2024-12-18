﻿namespace AssociationRegistry.Test.When_ProbeerAdresTeMatchen;

using AutoFixture;
using Common.AutoFixture;
using Events;
using FluentAssertions;
using Framework.Customizations;
using Grar;
using Grar.Exceptions;
using Moq;
using Resources;
using System.Net;
using Vereniging;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_GrarClient_Returned_InternalServerError
{
    [Fact]
    public async Task Then_AdresKonNietOvergenomenWordenUitAdressenregister()
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
                  .ThrowsAsync(new AdressenregisterReturnedNonSuccessStatusCode(HttpStatusCode.InternalServerError));

        vereniging.Hydrate(
            new VerenigingState()
               .Apply(feitelijkeVerenigingWerdGeregistreerd));

        await vereniging.ProbeerAdresTeMatchen(grarClient.Object, feitelijkeVerenigingWerdGeregistreerd.Locaties.First().LocatieId,
                                               CancellationToken.None);

        var @event = vereniging.UncommittedEvents.OfType<AdresKonNietOvergenomenWordenUitAdressenregister>().SingleOrDefault();

        @event.Should().NotBeNull();
        @event!.Reden.Should().Be(ExceptionMessages.AdresKonNietOvergenomenWorden);
    }
}
