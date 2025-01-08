namespace AssociationRegistry.Test.Admin.Api.Commands.VerenigingOfAnyKind.When_Adding_Locatie.CommandHandling;

using Acties.Locaties.VoegLocatieToe;
using AssociationRegistry.Framework;
using AutoFixture;
using Common.AutoFixture;
using Common.Framework;
using Common.Scenarios.CommandHandling;
using EventFactories;
using Events;
using Grar;
using Grar.Models;
using Marten;
using Messages;
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
            AdresId = new Registratiedata.AdresId(adresId.Adresbron.Code, adresId.Bronwaarde),
            IsActief = true,
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

        var maxLocatieId = scenario.GetVerenigingState().Locaties.Max(x => x.LocatieId) + 1;

        verenigingRepositoryMock.ShouldHaveSaved(
            new LocatieWerdToegevoegd(
                EventFactory.Locatie(command.Locatie) with
                {
                    LocatieId = maxLocatieId,
                }),
            new AdresWerdOvergenomenUitAdressenregister(scenario.VCode, maxLocatieId, adresDetailResponse.AdresId,
                                                        adresDetailResponse.ToAdresUitAdressenregister())
        );

        martenOutbox.Verify(expression: v => v.SendAsync(It.IsAny<TeAdresMatchenLocatieMessage>(), It.IsAny<DeliveryOptions>()),
                            Times.Never);
    }
}
