namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Locaties.VerenigingOfAnyKind.When_Wijzig_Locatie.CommandHandling;

using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Locaties.WijzigLocatie;
using AssociationRegistry.DecentraalBeheer.Vereniging.Adressen;
using AssociationRegistry.DecentraalBeheer.Vereniging.Exceptions;
using AssociationRegistry.DecentraalBeheer.Vereniging.Geotags;
using AssociationRegistry.Events;
using AssociationRegistry.Framework;
using AssociationRegistry.Grar;
using AssociationRegistry.Grar.Models;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Framework;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling.FeitelijkeVereniging;
using AssociationRegistry.Vereniging;
using AutoFixture;
using Common.StubsMocksFakes.VerenigingsRepositories;
using AssociationRegistry.Integrations.Grar.Clients;
using Marten;
using Moq;
using Wolverine.Marten;
using Xunit;

public class Given_An_Existing_Locatie_Without_AdresId
{
    [Fact]
    public async ValueTask Then_A_LocatieWerdToegevoegd_Event_And_AdresWerdOvergenomenUitAdressenregister_Is_Saved()
    {
        var scenario = new FeitelijkeVerenigingWerdGeregistreerdScenario();
        var verenigingRepositoryMock = new VerenigingRepositoryMock(scenario.GetVerenigingState());

        var fixture = new Fixture().CustomizeAdminApi();

        var grarClient = new Mock<IGrarClient>();
        var martenOutbox = new Mock<IMartenOutbox>();

        var commandHandler = new WijzigLocatieCommandHandler(verenigingRepositoryMock,
                                                             martenOutbox.Object,
                                                             Mock.Of<IDocumentSession>(),
                                                             grarClient.Object,
                                                             Mock.Of<IGeotagsService>()
        );

        var adresId = fixture.Create<AdresId>();
        var teWijzigenLocatie = scenario.FeitelijkeVerenigingWerdGeregistreerd.Locaties[0];
        var nietTeWijzigenLocatie = scenario.FeitelijkeVerenigingWerdGeregistreerd.Locaties[1];

        var synchronisatieAdres = nietTeWijzigenLocatie.Adres;

        var adresDetailResponse = fixture.Create<AddressDetailResponse>() with
        {
            AdresId = new Registratiedata.AdresId(adresId.Adresbron.Code, adresId.Bronwaarde),
            Gemeente = synchronisatieAdres.Gemeente,
            Busnummer = synchronisatieAdres.Busnummer,
            Huisnummer = synchronisatieAdres.Huisnummer,
            Postcode = synchronisatieAdres.Postcode,
            Straatnaam = synchronisatieAdres.Straatnaam,
            IsActief = true,
        };

        var teWijzigenLocatieCommand = fixture.Create<WijzigLocatieCommand.Locatie>() with
        {
            LocatieId = teWijzigenLocatie.LocatieId,
            AdresId = adresId,
            Naam = nietTeWijzigenLocatie.Naam,
            Locatietype = nietTeWijzigenLocatie.Locatietype,
            Adres = null,
        };

        var command = new WijzigLocatieCommand(scenario.VCode, teWijzigenLocatieCommand);

        grarClient.Setup(s => s.GetAddressById(adresId.ToString(), It.IsAny<CancellationToken>()))
                  .ReturnsAsync(adresDetailResponse);

        await Assert.ThrowsAsync<LocatieIsNietUniek>(
            async () =>
                await commandHandler.Handle(new CommandEnvelope<WijzigLocatieCommand>(command, fixture.Create<CommandMetadata>())));
    }
}
