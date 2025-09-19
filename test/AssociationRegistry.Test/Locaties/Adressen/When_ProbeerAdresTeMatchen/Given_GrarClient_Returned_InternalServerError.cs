namespace AssociationRegistry.Test.Locaties.Adressen.When_ProbeerAdresTeMatchen;

using AssociationRegistry.Grar.AdresMatch;
using AssociationRegistry.Grar.Exceptions;
using Events;
using Resources;
using AssociationRegistry.Test.Common.AutoFixture;
using AutoFixture;
using AutoFixture.Kernel;
using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Locaties.ProbeerAdresTeMatchen;
using Common.StubsMocksFakes.Faktories;
using Common.StubsMocksFakes.VerenigingsRepositories;
using DecentraalBeheer.Vereniging;
using FluentAssertions;
using AssociationRegistry.Integrations.Grar.AdresMatch;
using AssociationRegistry.Integrations.Grar.Clients;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using System.Net;
using Xunit;

public class Given_GrarClient_Returned_InternalServerError
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
                  .ThrowsAsync(new AdressenregisterReturnedNonSuccessStatusCode(HttpStatusCode.InternalServerError));

        var state = new VerenigingState()
           .Apply((dynamic)verenigingWerdGeregistreerd);

        vereniging.Hydrate(
            state);

        var faktory = Faktory.New();

        VerenigingRepositoryMock repository = faktory.VerenigingsRepository.Mock(state, expectedLoadingDubbel: true);
        var handler = new ProbeerAdresTeMatchenCommandHandler(repository, new AdresMatchService(
                                                                  grarClient.Object, new PerfectScoreMatchStrategy(),
                                                                  new GrarAddressVerrijkingsService(grarClient.Object)),
                                                              NullLogger<ProbeerAdresTeMatchenCommandHandler>.Instance);

        await handler.Handle(new ProbeerAdresTeMatchenCommand(verenigingWerdGeregistreerd.VCode,
                                                              verenigingWerdGeregistreerd.Locaties.First().LocatieId));

        var @event = repository.ShouldHaveSavedEventType<AdresKonNietOvergenomenWordenUitAdressenregister>(1).First();
        @event.Should().NotBeNull();
        @event!.Reden.Should().Be(ExceptionMessages.AdresKonNietOvergenomenWorden);
    }
}
