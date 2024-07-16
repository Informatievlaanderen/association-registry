namespace AssociationRegistry.Test.Admin.Api.VerenigingOfAnyKind.When_Adding_Locatie.CommandHandling;

using Acties.VoegLocatieToe;
using AssociationRegistry.Framework;
using AssociationRegistry.Grar;
using AssociationRegistry.Grar.AddressMatch;
using AssociationRegistry.Grar.Models;
using AutoFixture;
using Events;
using Fakes;
using Fixtures.Scenarios.CommandHandling;
using Framework;
using Marten;
using Moq;
using Vereniging;
using Wolverine;
using Wolverine.Marten;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_A_Locatie_With_Adres_id
{

    [Fact]
    public async Task Then_A_LocatieWerdToegevoegd_Event_And_AdresWerdOvergenomenUitAdressenregister_Is_Saved()
    {
        var scenario = new FeitelijkeVerenigingWerdGeregistreerdScenario();
        var verenigingRepositoryMock = new VerenigingRepositoryMock(scenario.GetVerenigingState());

        var fixture = new Fixture().CustomizeAdminApi();

        var grarClient = new Mock<IGrarClient>();
        var martenOutbox = new Mock<IMartenOutbox>();

        var commandHandler = new VoegLocatieToeCommandHandler(verenigingRepositoryMock,
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

        var locatie = fixture.Create<Locatie>() with
        {
            AdresId = adresId,
            Adres = null,
        };
        var command = new VoegLocatieToeCommand(scenario.VCode, locatie);

        grarClient.Setup(s => s.GetAddressById(adresId.ToString(), It.IsAny<CancellationToken>()))
                  .ReturnsAsync(adresDetailResponse);

        await commandHandler.Handle(new CommandEnvelope<VoegLocatieToeCommand>(command, fixture.Create<CommandMetadata>()));

        verenigingRepositoryMock.ShouldHaveSaved(
            new LocatieWerdToegevoegd(
                Registratiedata.Locatie.With(command.Locatie) with
                {
                    LocatieId = scenario.GetVerenigingState().Locaties.Max(x => x.LocatieId) + 1,
                }),
            new AdresWerdOvergenomenUitAdressenregister(scenario.VCode, locatie.LocatieId, adresDetailResponse.AdresId, adresDetailResponse.ToAdresUitAdressenregister())
        );

        martenOutbox.Verify(v => v.SendAsync(It.IsAny<TeAdresMatchenLocatieMessage>(), It.IsAny<DeliveryOptions>()), Times.Never);
    }
}
