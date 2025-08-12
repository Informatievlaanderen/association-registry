namespace AssociationRegistry.Test.Locaties.Adressen.When_ProbeerAdresTeMatchen;

using AssociationRegistry.Grar;
using AssociationRegistry.Grar.AdresMatch;
using AssociationRegistry.Grar.Models;
using Events;
using Formats;
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
using Wolverine;
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
    public async Task Then_AdresHeeftGeenVerschillenMetAdressenregister_IsApplied(Type verenigingType)
    {
        var fixture = new Fixture().CustomizeDomain();
        var context = new SpecimenContext(fixture);

        var verenigingWerdGeregistreerd = (IVerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd)context.Resolve(verenigingType);
        var locatie = verenigingWerdGeregistreerd.Locaties.First();

        var grarClient = SetupGrarClientMock(fixture, locatie);

        var vereniging = new VerenigingOfAnyKind();

        var state = new VerenigingState()
           .Apply((dynamic)verenigingWerdGeregistreerd);

        vereniging.Hydrate(state);

        var faktory = Faktory.New();

        VerenigingRepositoryMock verenigingsRepository = faktory.VerenigingsRepository.Mock(state, expectedLoadingDubbel: true);
        var handler = new ProbeerAdresTeMatchenCommandHandler(verenigingsRepository, new AdresMatchService(
                                                                  grarClient.Object, new PerfectScoreMatchStrategy(),
                                                                  new GrarAddressVerrijkingsService(grarClient.Object)),
                                                              NullLogger<ProbeerAdresTeMatchenCommandHandler>.Instance);

        await handler.Handle(new ProbeerAdresTeMatchenCommand(verenigingWerdGeregistreerd.VCode,
                                                              verenigingWerdGeregistreerd.Locaties.ToArray()[0].LocatieId));

        verenigingsRepository.ShouldHaveSavedEventType<AdresHeeftGeenVerschillenMetAdressenregister>(1);
    }
}
