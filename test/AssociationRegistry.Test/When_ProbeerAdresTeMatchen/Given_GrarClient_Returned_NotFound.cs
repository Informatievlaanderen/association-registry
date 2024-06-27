﻿namespace AssociationRegistry.Test.When_ProbeerAdresTeMatchen;

using AutoFixture;
using Events;
using FluentAssertions;
using Framework.Customizations;
using Grar;
using Grar.Exceptions;
using Moq;
using System.Net;
using Vereniging;
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

        grarClient.Setup(x => x.GetAddressMatches(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                                           It.IsAny<string>()))
                  .ThrowsAsync(new AdressenregisterReturnedNonSuccessStatusCode(HttpStatusCode.NotFound));

        vereniging.Hydrate(
            new VerenigingState()
               .Apply(feitelijkeVerenigingWerdGeregistreerd));

        await vereniging.ProbeerAdresTeMatchen(grarClient.Object, feitelijkeVerenigingWerdGeregistreerd.Locaties.First().LocatieId);

        var @event = vereniging.UncommittedEvents.OfType<AdresWerdNietGevondenInAdressenregister>().SingleOrDefault();

        @event.Should().NotBeNull();
    }
}
