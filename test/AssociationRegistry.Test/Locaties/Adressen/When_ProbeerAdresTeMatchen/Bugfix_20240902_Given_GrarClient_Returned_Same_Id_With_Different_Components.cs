namespace AssociationRegistry.Test.Locaties.Adressen.When_ProbeerAdresTeMatchen;

using AssociationRegistry.Events;
using AssociationRegistry.Grar;
using AssociationRegistry.Grar.Models;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Vereniging;
using AutoFixture;
using FluentAssertions;
using Moq;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Bugfix_20240902_Given_GrarClient_Returned_Same_Id_With_Different_Components
{
    private readonly VerenigingOfAnyKind _vereniging;

    public Bugfix_20240902_Given_GrarClient_Returned_Same_Id_With_Different_Components()
    {
        var fixture = new Fixture().CustomizeDomain();

        var locatie = fixture.Create<Registratiedata.Locatie>() with
        {
            Adres = fixture.Create<Registratiedata.Adres>(),
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
                  .ReturnsAsync(
                       new AdresMatchResponseCollection(new[]
                       {
                           fixture.Create<AddressMatchResponse>() with
                           {
                               Score = 100,
                               AdresId = locatie.AdresId,
                           },
                       })
                       );

        return grarClient;
    }

    [Fact]
    public void Then_AdresWerdOvergenomenUitAdressenregister_IsApplied()
    {
        var overgenomenEvent = _vereniging.UncommittedEvents.OfType<AdresWerdOvergenomenUitAdressenregister>().SingleOrDefault();

        overgenomenEvent.Should().NotBeNull();
    }

    [Fact]
    public void Then_LocatieDuplicaatWerdVerwijderdNaAdresMatch_IsNeverApplied()
    {
        var duplicaatEvent = _vereniging.UncommittedEvents.OfType<LocatieDuplicaatWerdVerwijderdNaAdresMatch>().SingleOrDefault();

        duplicaatEvent.Should().BeNull();
    }
}
