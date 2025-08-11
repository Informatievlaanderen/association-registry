namespace AssociationRegistry.Test.Locaties.Adressen.When_ProbeerAdresTeMatchen;

using AssociationRegistry.Grar;
using AssociationRegistry.Grar.AdresMatch;
using AssociationRegistry.Grar.Clients;
using AssociationRegistry.Grar.Models;
using Events;
using AssociationRegistry.Test.Common.AutoFixture;
using Vereniging;
using AutoFixture;
using CommandHandling.DecentraalBeheer.Acties.Locaties.ProbeerAdresTeMatchen;
using Common.StubsMocksFakes.Faktories;
using DecentraalBeheer.Vereniging;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

public class Given_Duplicate_Locaties_With_Same_Name_For_VZER
{
    [Theory]
    [InlineData(true, false, 1, 0)]
    [InlineData(false, false, 1, 0)]
    [InlineData(false, true, 0, 1)]
    public async Task Then_AdresKonNietOvergenomenWordenUitAdressenregister(
        bool isPrimair1,
        bool isPrimair2,
        int verwijderdeLocatieIndex,
        int behoudenLocatieIndex)
    {
        var fixture = new Fixture().CustomizeDomain();

        var grarClient = new Mock<IGrarClient>();
        var vereniging = new VerenigingOfAnyKind();

        var locatie1 = fixture.Create<Registratiedata.Locatie>() with
        {
            IsPrimair = isPrimair1,
        };

        var locatie2 = fixture.Create<Registratiedata.Locatie>() with
        {
            Naam = locatie1.Naam,
            IsPrimair = isPrimair2,
        };

        var verenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd = fixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>()
            with
            {
                Locaties = new[]
                {
                    locatie1,
                    locatie2,
                },
            };

        var adresId = fixture.Create<Registratiedata.AdresId>();

        var adresWerdOvergenomen = fixture.Create<AdresWerdOvergenomenUitAdressenregister>()
            with
            {
                VCode = verenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode,
                LocatieId = verenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.Locaties.First().LocatieId,
                Adres = fixture.Create<Registratiedata.AdresUitAdressenregister>(),
                AdresId = adresId,
            };

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
                           AdresId = adresId,
                       },
                   }));

        var state = new VerenigingState()
                             .Apply(verenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd)
                             .Apply(adresWerdOvergenomen);

        vereniging.Hydrate(state);

        var faktory = Faktory.New();

        var repository = faktory.VerenigingsRepository.Mock(state, expectedLoadingDubbel: true);
        var handler = new ProbeerAdresTeMatchenCommandHandler(repository, new AdresMatchService(
                                                                  grarClient.Object, new PerfectScoreMatchStrategy(),
                                                                  new GemeenteVerrijkingService(grarClient.Object)),
                                                              NullLogger<ProbeerAdresTeMatchenCommandHandler>.Instance);

        await handler.Handle(new ProbeerAdresTeMatchenCommand(verenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode,
                                                              verenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.Locaties.ToArray()[1].LocatieId));

        var @event = repository.ShouldHaveSavedEventType<LocatieDuplicaatWerdVerwijderdNaAdresMatch>(1).First();

        @event.Should().NotBeNull();
        @event.VerwijderdeLocatieId.Should().Be(verenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.Locaties[verwijderdeLocatieIndex].LocatieId);
        @event.BehoudenLocatieId.Should().Be(verenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.Locaties[behoudenLocatieIndex].LocatieId);
        @event.LocatieNaam.Should().Be((verenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.Locaties[behoudenLocatieIndex].Naam));
    }
}

public class Given_Duplicate_Locaties_With_Different_Names_For_VZER
{
    [Fact]
    public async ValueTask Then_AdresKonNietOvergenomenWordenUitAdressenregister()
    {
        var fixture = new Fixture().CustomizeDomain();

        var grarClient = new Mock<IGrarClient>();
        var vereniging = new VerenigingOfAnyKind();

        var locatie1 = fixture.Create<Registratiedata.Locatie>();

        var locatie2 = fixture.Create<Registratiedata.Locatie>() with
        {
            Naam = fixture.Create<string>(),
        };

        var verenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd = fixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>()
            with
            {
                Locaties = new[]
                {
                    locatie1,
                    locatie2,
                },
            };

        var adresId = fixture.Create<Registratiedata.AdresId>();

        var adresWerdOvergenomen = fixture.Create<AdresWerdOvergenomenUitAdressenregister>()
            with
            {
                VCode = verenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode,
                LocatieId = verenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.Locaties.First().LocatieId,
                Adres = fixture.Create<Registratiedata.AdresUitAdressenregister>(),
                AdresId = adresId,
            };

        grarClient.Setup(x => x.GetAddressMatches(
                             It.IsAny<string>(),
                             It.IsAny<string>(),
                             It.IsAny<string>(),
                             It.IsAny<string>(),
                             It.IsAny<string>(),
                             It.IsAny<CancellationToken>()))
                  .ReturnsAsync(new AdresMatchResponseCollection(new[]
                   {
                       fixture.Create<AddressMatchResponse>() with
                       {
                           Score = 100,
                           AdresId = adresId,
                       },
                   }));

        var state = new VerenigingState()
                             .Apply(verenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd)
                             .Apply(adresWerdOvergenomen);

        vereniging.Hydrate(
            state);

        var faktory = Faktory.New();

        var repository = faktory.VerenigingsRepository.Mock(state, expectedLoadingDubbel: true);
        var handler = new ProbeerAdresTeMatchenCommandHandler(repository, new AdresMatchService(
                                                                  grarClient.Object, new PerfectScoreMatchStrategy(),
                                                                  new GemeenteVerrijkingService(grarClient.Object)),
                                                              NullLogger<ProbeerAdresTeMatchenCommandHandler>.Instance);

        await handler.Handle(new ProbeerAdresTeMatchenCommand(verenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode,
                                                              verenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.Locaties.ToArray()[1].LocatieId));

        repository.ShouldNotHaveSaved<LocatieDuplicaatWerdVerwijderdNaAdresMatch>();
    }
}
