namespace AssociationRegistry.Test.Locaties.Adressen.When_ProbeerAdresTeMatchen;

using AssociationRegistry.Events;
using AssociationRegistry.Formats;
using AssociationRegistry.Grar;
using AssociationRegistry.Grar.Models;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Vereniging;
using AutoFixture;
using FluentAssertions;
using Grar.Clients;
using Moq;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_AdresHeeftGeenVerschillenMetAdressenregister
{
    private readonly VerenigingOfAnyKind _vereniging;

    public Given_AdresHeeftGeenVerschillenMetAdressenregister()
    {
        var fixture = new Fixture().CustomizeDomain();

        var locatie = fixture.Create<Registratiedata.Locatie>() with
        {
            Adres = fixture.Create<Registratiedata.Adres>()
                with
                {
                    Land = Adres.België,
                },
            AdresId = fixture.Create<Registratiedata.AdresId>(),
        };

        var feitelijkeVerenigingWerdGeregistreerd = fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>()
            with
            {
                Locaties = new[]
                {
                    locatie,
                },
            };

        var grarClient = SetupGrarClientMock(fixture, locatie);

        _vereniging = new VerenigingOfAnyKind();

        _vereniging.Hydrate(
            new VerenigingState()
               .Apply(feitelijkeVerenigingWerdGeregistreerd));

        _vereniging.ProbeerAdresTeMatchen(grarClient.Object, feitelijkeVerenigingWerdGeregistreerd.Locaties.ToArray()[0].LocatieId,
                                          CancellationToken.None)
                   .GetAwaiter().GetResult();
    }

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

    [Fact]
    public void Then_AdresHeeftGeenVerschillenMetAdressenregister_IsApplied()
    {
        var overgenomenEvent = _vereniging.UncommittedEvents.OfType<AdresHeeftGeenVerschillenMetAdressenregister>().SingleOrDefault();

        overgenomenEvent.Should().NotBeNull();
    }
}
