namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Locaties.VerenigingOfAnyKind.When_Wijzig_Locatie.CommandHandling;

using AssociationRegistry.Admin.ProjectionHost.Constants;
using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Locaties.ProbeerAdresTeMatchen;
using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Locaties.WijzigLocatie;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.DecentraalBeheer.Vereniging.Adressen;
using AssociationRegistry.Events;
using AssociationRegistry.Framework;
using AssociationRegistry.Grar.Clients;
using AssociationRegistry.Grar.Models;
using AssociationRegistry.Grar.Models.PostalInfo;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Framework;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling.FeitelijkeVereniging;
using AssociationRegistry.Vereniging;
using AutoFixture;
using Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;
using Common.StubsMocksFakes.Faktories;
using Common.StubsMocksFakes.VerenigingsRepositories;
using Events.Factories;
using Marten;
using Moq;
using Wolverine;
using Wolverine.Marten;
using Xunit;

public class Given_A_Locatie_With_Adres_id
{
    [Fact]
    public async ValueTask Then_A_LocatieWerdToegevoegd_Event_Is_Saved()
    {
        var scenario = new FeitelijkeVerenigingWerdGeregistreerdScenario();
        var fixture = new Fixture().CustomizeAdminApi();
        var (geotagsService, geotags) = Faktory.New(fixture).GeotagsService.ReturnsRandomGeotags();

        var adresId = fixture.Create<AdresId>();
        var locatie = new WijzigLocatieCommand.Locatie(
            scenario.GetVerenigingState().Locaties.First().LocatieId,
            Locatietypes.Activiteiten,
            IsPrimair: false,
            Naam: "De sjiekste club",
            Adres: null,
            adresId);

        var factory = new Faktory(fixture);
        var verenigingRepositoryMock = factory.VerenigingsRepository.Mock(scenario.GetVerenigingState());
        var grarClient = new Mock<IGrarClient>();
        var martenOutbox = new Mock<IMartenOutbox>();

        var commandHandler = new WijzigLocatieCommandHandler(verenigingRepositoryMock,
                                                             martenOutbox.Object,
                                                             Mock.Of<IDocumentSession>(),
                                                             grarClient.Object,
                                                             geotagsService.Object
        );

        var adresDetailResponse = fixture.Create<AddressDetailResponse>() with
        {
            AdresId = new Registratiedata.AdresId(adresId.Adresbron.Code, adresId.Bronwaarde),
            IsActief = true,
        };

        var command = new WijzigLocatieCommand(scenario.VCode, locatie);

        grarClient.Setup(s => s.GetAddressById(adresId.ToString(), It.IsAny<CancellationToken>()))
                  .ReturnsAsync(adresDetailResponse);

        grarClient.Setup(s => s.GetPostalInformationDetail(It.IsAny<string>()))
                  .ReturnsAsync(fixture.Create<PostalInfoDetailResponse>() with
                   {
                       Gemeentenaam = adresDetailResponse.Gemeente,
                       Postcode = adresDetailResponse.Postcode,
                       Postnamen = Postnamen.Empty,
                   });

        await commandHandler.Handle(new CommandEnvelope<WijzigLocatieCommand>(command, fixture.Create<CommandMetadata>()));

        verenigingRepositoryMock.ShouldHaveSavedExact(
            new LocatieWerdGewijzigd(
                EventFactory.Locatie(Locatie.Hydrate(locatie.LocatieId, locatie.Naam, isPrimair: false, locatie.Locatietype,
                                                             locatie.Adres, locatie.AdresId))),
            new AdresWerdOvergenomenUitAdressenregister(scenario.VCode, locatie.LocatieId, adresDetailResponse.AdresId,
                                                        adresDetailResponse.ToAdresUitAdressenregister()),
            EventFactory.GeotagsWerdenBepaald(scenario.VCode, geotags)
        );

        martenOutbox.Verify(expression: v => v.SendAsync(It.IsAny<ProbeerAdresTeMatchenCommand>(), It.IsAny<DeliveryOptions>()),
                            Times.Never);
    }
}
