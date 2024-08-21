namespace AssociationRegistry.Test.Admin.Api.Commands.VerenigingOfAnyKind.When_Wijzig_Locatie.CommandHandling;

using Acties.WijzigLocatie;
using AssociationRegistry.Admin.ProjectionHost.Constants;
using Events;
using AssociationRegistry.Framework;
using AssociationRegistry.Grar;
using AssociationRegistry.Grar.AddressMatch;
using AssociationRegistry.Grar.Models;
using Framework;
using AssociationRegistry.Test.Common.Framework;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling;
using Vereniging;
using AutoFixture;
using Marten;
using Moq;
using Wolverine;
using Wolverine.Marten;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_A_Locatie_With_Adres_id
{
    [Fact]
    public async Task Then_A_LocatieWerdToegevoegd_Event_Is_Saved()
    {
        var scenario = new FeitelijkeVerenigingWerdGeregistreerdScenario();
        var verenigingRepositoryMock = new VerenigingRepositoryMock(scenario.GetVerenigingState());

        var fixture = new Fixture().CustomizeAdminApi();

        var grarClient = new Mock<IGrarClient>();
        var martenOutbox = new Mock<IMartenOutbox>();

        var commandHandler = new WijzigLocatieCommandHandler(verenigingRepositoryMock,
                                                             martenOutbox.Object,
                                                             Mock.Of<IDocumentSession>(),
                                                             grarClient.Object
        );

        var adresId = fixture.Create<AdresId>();
        var adresDetailResponse = fixture.Create<AddressDetailResponse>() with
        {
            AdresId = new Registratiedata.AdresId(adresId.Adresbron, adresId.Bronwaarde),
            IsActief = true
        };

        var locatie = new WijzigLocatieCommand.Locatie(
            scenario.GetVerenigingState().Locaties.First().LocatieId,
            Locatietypes.Activiteiten,
            false,
            "De sjiekste club",
            null,
            adresId);

        var command = new WijzigLocatieCommand(scenario.VCode, locatie);

        grarClient.Setup(s => s.GetAddressById(adresId.ToString(), It.IsAny<CancellationToken>()))
                  .ReturnsAsync(adresDetailResponse);
        grarClient.Setup(s => s.GetPostalInformation(It.IsAny<string>()))
                  .ReturnsAsync(fixture.Create<PostalInformationResponse>() with
                   {
                       Gemeentenaam = adresDetailResponse.Gemeente,
                       Postcode = adresDetailResponse.Postcode,
                       Postnamen = Array.Empty<string>()
                   });

        await commandHandler.Handle(new CommandEnvelope<WijzigLocatieCommand>(command, fixture.Create<CommandMetadata>()));

        verenigingRepositoryMock.ShouldHaveSaved(
            new LocatieWerdGewijzigd(
                Registratiedata.Locatie.With(Locatie.Hydrate(locatie.LocatieId, locatie.Naam, false, locatie.Locatietype, locatie.Adres, locatie.AdresId))),
            new AdresWerdOvergenomenUitAdressenregister(scenario.VCode, locatie.LocatieId, adresDetailResponse.AdresId, adresDetailResponse.ToAdresUitAdressenregister())
        );

        martenOutbox.Verify(v => v.SendAsync(It.IsAny<TeAdresMatchenLocatieMessage>(), It.IsAny<DeliveryOptions>()), Times.Never);
    }
}
