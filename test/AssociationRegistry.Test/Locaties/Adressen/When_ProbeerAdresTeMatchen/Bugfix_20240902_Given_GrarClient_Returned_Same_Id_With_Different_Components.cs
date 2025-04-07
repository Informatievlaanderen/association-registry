namespace AssociationRegistry.Test.Locaties.Adressen.When_ProbeerAdresTeMatchen;

using AssociationRegistry.Grar;
using AssociationRegistry.Grar.Clients;
using AssociationRegistry.Grar.Models;
using Events;
using AssociationRegistry.Test.Common.AutoFixture;
using Vereniging;
using AutoFixture;
using AutoFixture.Kernel;
using FluentAssertions;
using Moq;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Bugfix_20240902_Given_GrarClient_Returned_Same_Id_With_Different_Components
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

    [Theory]
    [InlineData(typeof(FeitelijkeVerenigingWerdGeregistreerd))]
    [InlineData(typeof(VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd))]
    public void Then_AdresWerdOvergenomenUitAdressenregister_IsApplied_And_LocatieDuplicaatWerdVerwijderdNaAdresMatch_IsNeverApplied(Type verenigingType)
    {
        var fixture = new Fixture().CustomizeDomain();
        var context = new SpecimenContext(fixture);

        var verenigingWerdGeregistreerd = (IVerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd)context.Resolve(verenigingType);

        var locatie = verenigingWerdGeregistreerd.Locaties.First();

        var grarClient = SetupGrarClientMock(fixture, locatie);

        var vereniging = new VerenigingOfAnyKind();
        vereniging.Hydrate(
            new VerenigingState()
               .Apply((dynamic)verenigingWerdGeregistreerd));

        vereniging.ProbeerAdresTeMatchen(grarClient.Object, verenigingWerdGeregistreerd.Locaties.ToArray()[0].LocatieId,
                                          CancellationToken.None)
                   .GetAwaiter().GetResult();

        var overgenomenEvent = vereniging.UncommittedEvents.OfType<AdresWerdOvergenomenUitAdressenregister>().SingleOrDefault();

        overgenomenEvent.Should().NotBeNull();

        var duplicaatEvent = vereniging.UncommittedEvents.OfType<LocatieDuplicaatWerdVerwijderdNaAdresMatch>().SingleOrDefault();

        duplicaatEvent.Should().BeNull();
    }
}
