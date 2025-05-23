﻿namespace AssociationRegistry.Test.Locaties.Adressen.When_ProbeerAdresTeMatchen;

using AssociationRegistry.Grar;
using AssociationRegistry.Grar.Clients;
using AssociationRegistry.Grar.Models;
using Events;
using AssociationRegistry.Test.Common.AutoFixture;
using Vereniging;
using AutoFixture;
using FluentAssertions;
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

        vereniging.Hydrate(
            new VerenigingState()
               .Apply(verenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd)
               .Apply(adresWerdOvergenomen));

        await vereniging.ProbeerAdresTeMatchen(grarClient.Object, verenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.Locaties.ToArray()[1].LocatieId,
                                               CancellationToken.None);

        var @event = vereniging.UncommittedEvents.OfType<LocatieDuplicaatWerdVerwijderdNaAdresMatch>().SingleOrDefault();

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

        vereniging.Hydrate(
            new VerenigingState()
               .Apply(verenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd)
               .Apply(adresWerdOvergenomen));

        await vereniging.ProbeerAdresTeMatchen(grarClient.Object, verenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.Locaties.ToArray()[1].LocatieId,
                                               CancellationToken.None);

        var @event = vereniging.UncommittedEvents.OfType<LocatieDuplicaatWerdVerwijderdNaAdresMatch>().SingleOrDefault();

        @event.Should().BeNull();
    }
}
