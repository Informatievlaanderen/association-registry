namespace AssociationRegistry.Test.Locaties.Adressen.When_ProbeerAdresTeMatchen;

using AssociationRegistry.Grar.AdresMatch;
using AssociationRegistry.Grar.Models;
using Events;
using AssociationRegistry.Test.Common.AutoFixture;
using AutoFixture;
using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Locaties.ProbeerAdresTeMatchen;
using Common.StubsMocksFakes.Faktories;
using Common.StubsMocksFakes.VerenigingsRepositories;
using DecentraalBeheer.Vereniging;
using FluentAssertions;
using AssociationRegistry.Integrations.Grar.AdresMatch;
using AssociationRegistry.Integrations.Grar.Clients;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

public class Given_Duplicate_Locaties_With_Same_Name
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

        var feitelijkeVerenigingWerdGeregistreerd = fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>()
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
                VCode = feitelijkeVerenigingWerdGeregistreerd.VCode,
                LocatieId = feitelijkeVerenigingWerdGeregistreerd.Locaties.First().LocatieId,
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
                             .Apply(feitelijkeVerenigingWerdGeregistreerd)
                             .Apply(adresWerdOvergenomen);

        vereniging.Hydrate(state);

        var faktory = Faktory.New();

        VerenigingRepositoryMock repository = faktory.VerenigingsRepository.Mock(state, expectedLoadingDubbel: true);

        var handler = new ProbeerAdresTeMatchenCommandHandler(repository, new AdresMatchService(
                                                                  grarClient.Object, new PerfectScoreMatchStrategy(),
                                                                  new GrarAddressVerrijkingsService(grarClient.Object)),
                                                              NullLogger<ProbeerAdresTeMatchenCommandHandler>.Instance);

        await handler.Handle(new ProbeerAdresTeMatchenCommand(feitelijkeVerenigingWerdGeregistreerd.VCode,
                                                              feitelijkeVerenigingWerdGeregistreerd.Locaties.ToArray()[1].LocatieId));

        var evnt = repository.ShouldHaveSavedEventType<LocatieDuplicaatWerdVerwijderdNaAdresMatch>(1)
                                                                   .First();

        evnt.VerwijderdeLocatieId.Should().Be(feitelijkeVerenigingWerdGeregistreerd.Locaties[verwijderdeLocatieIndex].LocatieId);
        evnt.BehoudenLocatieId.Should().Be(feitelijkeVerenigingWerdGeregistreerd.Locaties[behoudenLocatieIndex].LocatieId);
        evnt.LocatieNaam.Should().Be(feitelijkeVerenigingWerdGeregistreerd.Locaties[behoudenLocatieIndex].Naam);
    }
}

public class Given_Duplicate_Locaties_With_Different_Names
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

        var feitelijkeVerenigingWerdGeregistreerd = fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>()
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
                VCode = feitelijkeVerenigingWerdGeregistreerd.VCode,
                LocatieId = feitelijkeVerenigingWerdGeregistreerd.Locaties.First().LocatieId,
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
                             .Apply(feitelijkeVerenigingWerdGeregistreerd)
                             .Apply(adresWerdOvergenomen);

        vereniging.Hydrate(state);

        var faktory = Faktory.New();

        var repository = faktory.VerenigingsRepository.Mock(state, expectedLoadingDubbel: true);
        var handler = new ProbeerAdresTeMatchenCommandHandler(repository, new AdresMatchService(
                                                                  grarClient.Object, new PerfectScoreMatchStrategy(),
                                                                  new GrarAddressVerrijkingsService(grarClient.Object)),
                                                              NullLogger<ProbeerAdresTeMatchenCommandHandler>.Instance);

        await handler.Handle(new ProbeerAdresTeMatchenCommand(feitelijkeVerenigingWerdGeregistreerd.VCode,
                                                              feitelijkeVerenigingWerdGeregistreerd.Locaties.ToArray()[1].LocatieId));

        var @event = vereniging.UncommittedEvents.OfType<LocatieDuplicaatWerdVerwijderdNaAdresMatch>().SingleOrDefault();

        @event.Should().BeNull();
    }
}
