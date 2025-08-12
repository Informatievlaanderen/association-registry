namespace AssociationRegistry.Test.Locaties.Adressen.When_ProbeerAdresTeMatchen;

using AssociationRegistry.Grar;
using AssociationRegistry.Grar.AdresMatch;
using AssociationRegistry.Grar.Exceptions;
using Events;
using Resources;
using AssociationRegistry.Test.Common.AutoFixture;
using Vereniging;
using AutoFixture;
using AutoFixture.Kernel;
using CommandHandling.DecentraalBeheer.Acties.Locaties.ProbeerAdresTeMatchen;
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

        var state = new VerenigingState()
           .Apply((dynamic)verenigingWerdGeregistreerd);

        vereniging.Hydrate(
            state);

        var firstRegisteredLocatie = verenigingWerdGeregistreerd.Locaties.First();

        var faktory = Faktory.New();

        VerenigingRepositoryMock repository = faktory.VerenigingsRepository.Mock(state, expectedLoadingDubbel: true);
        var handler = new ProbeerAdresTeMatchenCommandHandler(repository, new AdresMatchService(
                                                                  grarClient.Object, new PerfectScoreMatchStrategy(),
                                                                  new GrarAddressVerrijkingsService(grarClient.Object)),
                                                              NullLogger<ProbeerAdresTeMatchenCommandHandler>.Instance);

        await handler.Handle(new ProbeerAdresTeMatchenCommand(verenigingWerdGeregistreerd.VCode,
                                                              firstRegisteredLocatie.LocatieId));

        var @event = repository.ShouldHaveSavedEventType<AdresKonNietOvergenomenWordenUitAdressenregister>(1).First();

        @event.Should().NotBeNull();
        @event.Adres.Should().BeEquivalentTo($"{firstRegisteredLocatie.Adres.Straatnaam} {firstRegisteredLocatie.Adres.Huisnummer}" +
                                             (!string.IsNullOrWhiteSpace(firstRegisteredLocatie.Adres.Busnummer)
                                                 ? $" bus {firstRegisteredLocatie.Adres.Busnummer}"
                                                 : string.Empty) +
                                             $", {firstRegisteredLocatie.Adres.Postcode} {firstRegisteredLocatie.Adres.Gemeente}, {firstRegisteredLocatie.Adres.Land}");
        @event!.Reden.Should().Be(ExceptionMessages.AdresKonNietGevalideerdWordenBijAdressenregister);
    }
}
