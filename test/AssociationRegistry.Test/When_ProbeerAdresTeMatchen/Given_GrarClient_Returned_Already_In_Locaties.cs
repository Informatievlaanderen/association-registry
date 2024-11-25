namespace AssociationRegistry.Test.When_ProbeerAdresTeMatchen;

using AutoFixture;
using Common.AutoFixture;
using Events;
using FluentAssertions;
using Framework.Customizations;
using Grar;
using Grar.Models;
using Moq;
using Vereniging;
using Xunit;
using Xunit.Categories;

[UnitTest]
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

        vereniging.Hydrate(
            new VerenigingState()
               .Apply(feitelijkeVerenigingWerdGeregistreerd)
               .Apply(adresWerdOvergenomen));

        await vereniging.ProbeerAdresTeMatchen(grarClient.Object, feitelijkeVerenigingWerdGeregistreerd.Locaties.ToArray()[1].LocatieId,
                                               CancellationToken.None);

        var @event = vereniging.UncommittedEvents.OfType<LocatieDuplicaatWerdVerwijderdNaAdresMatch>().SingleOrDefault();

        @event.Should().NotBeNull();
        @event.VerwijderdeLocatieId.Should().Be(feitelijkeVerenigingWerdGeregistreerd.Locaties[verwijderdeLocatieIndex].LocatieId);
        @event.BehoudenLocatieId.Should().Be(feitelijkeVerenigingWerdGeregistreerd.Locaties[behoudenLocatieIndex].LocatieId);
        @event.LocatieNaam.Should().Be((feitelijkeVerenigingWerdGeregistreerd.Locaties[behoudenLocatieIndex].Naam));
    }
}

[UnitTest]
public class Given_Duplicate_Locaties_With_Different_Names
{
    [Fact]
    public async Task Then_AdresKonNietOvergenomenWordenUitAdressenregister()
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

        vereniging.Hydrate(
            new VerenigingState()
               .Apply(feitelijkeVerenigingWerdGeregistreerd)
               .Apply(adresWerdOvergenomen));

        await vereniging.ProbeerAdresTeMatchen(grarClient.Object, feitelijkeVerenigingWerdGeregistreerd.Locaties.ToArray()[1].LocatieId,
                                               CancellationToken.None);

        var @event = vereniging.UncommittedEvents.OfType<LocatieDuplicaatWerdVerwijderdNaAdresMatch>().SingleOrDefault();

        @event.Should().BeNull();
    }
}
