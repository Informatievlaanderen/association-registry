namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Locaties.VerenigingOfAnyKind.When_Adding_Locatie.CommandHandling;

using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Locaties.ProbeerAdresTeMatchen;
using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Locaties.VoegLocatieToe;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.DecentraalBeheer.Vereniging.Adressen;
using AssociationRegistry.Events;
using AssociationRegistry.Framework;
using AssociationRegistry.Grar;
using AssociationRegistry.Grar.Models;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Framework;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling.FeitelijkeVereniging;
using AssociationRegistry.Vereniging;
using AutoFixture;
using Common.StubsMocksFakes.Faktories;
using Common.StubsMocksFakes.VerenigingsRepositories;
using Events.Factories;
using AssociationRegistry.Integrations.Grar.Clients;
using Marten;
using Moq;
using Wolverine;
using Wolverine.Marten;
using Xunit;

public class Given_A_Locatie_With_Adres_id
{
    [Fact]
    public async ValueTask Then_A_LocatieWerdToegevoegd_Event_And_AdresWerdOvergenomenUitAdressenregister_Is_Saved()
    {
        var scenario = new FeitelijkeVerenigingWerdGeregistreerdScenario();

        var fixture = new Fixture().CustomizeAdminApi();

        var factory = new Faktory(fixture);

        var verenigingRepositoryMock = factory.VerenigingsRepository.Mock(scenario.GetVerenigingState());
        (var geotagsService, var geotags) = factory.GeotagsService.ReturnsRandomGeotags();

        var grarClient = new Mock<IGrarClient>();

        var adresId = fixture.Create<AdresId>();
        var adresDetailResponse = fixture.Create<AddressDetailResponse>() with
        {
            AdresId = new Registratiedata.AdresId(adresId.Adresbron.Code, adresId.Bronwaarde),
            IsActief = true,
        };
        grarClient.Setup(s => s.GetAddressById(adresId.ToString(), It.IsAny<CancellationToken>()))
                  .ReturnsAsync(adresDetailResponse);

        var martenOutbox = new Mock<IMartenOutbox>();

        var commandHandler = new VoegLocatieToeCommandHandler(verenigingRepositoryMock,
                                                              martenOutbox.Object,
                                                              Mock.Of<IDocumentSession>(),
                                                              grarClient.Object,
                                                              geotagsService.Object
        );



        var locatie = fixture.Create<Locatie>() with
        {
            AdresId = adresId,
            Adres = null,
        };

        var command = new VoegLocatieToeCommand(scenario.VCode, locatie);



        await commandHandler.Handle(new CommandEnvelope<VoegLocatieToeCommand>(command, fixture.Create<CommandMetadata>()));

        var maxLocatieId = scenario.GetVerenigingState().Locaties.Max(x => x.LocatieId) + 1;

        verenigingRepositoryMock.ShouldHaveSavedExact(
            new LocatieWerdToegevoegd(
                EventFactory.Locatie(command.Locatie) with
                {
                    LocatieId = maxLocatieId,
                }),
            new AdresWerdOvergenomenUitAdressenregister(scenario.VCode, maxLocatieId, adresDetailResponse.AdresId,
                                                        adresDetailResponse.ToAdresUitAdressenregister()),
            new GeotagsWerdenBepaald(scenario.VCode, geotags.Select(x => new Registratiedata.Geotag(x.Identificatie)).ToArray())
        );

        martenOutbox.Verify(expression: v => v.SendAsync(It.IsAny<ProbeerAdresTeMatchenCommand>(), It.IsAny<DeliveryOptions>()),
                            Times.Never);
    }
}
