namespace AssociationRegistry.Test.Admin.Api.VerenigingOfAnyKind.When_Wijzig_Locatie.CommandHandling;

using AssociationRegistry.Acties.VoegLocatieToe;
using AssociationRegistry.Acties.WijzigLocatie;
using AssociationRegistry.Admin.Api.Verenigingen.Locaties.FeitelijkeVereniging.WijzigLocatie.RequestModels;
using AssociationRegistry.Events;
using AssociationRegistry.Framework;
using AssociationRegistry.Grar;
using AssociationRegistry.Grar.Models;
using AssociationRegistry.Test.Admin.Api.Fakes;
using AssociationRegistry.Test.Admin.Api.Fixtures.Scenarios.CommandHandling;
using AssociationRegistry.Test.Admin.Api.Framework;
using AssociationRegistry.Vereniging;
using AssociationRegistry.Vereniging.Exceptions;
using AutoFixture;
using Marten;
using Moq;
using Wolverine.Marten;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_An_Existing_Locatie_Without_AdresId
{

    [Fact]
    public async Task Then_A_LocatieWerdToegevoegd_Event_And_AdresWerdOvergenomenUitAdressenregister_Is_Saved()
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
        var teWijzigenLocatie = scenario.FeitelijkeVerenigingWerdGeregistreerd.Locaties[0];
        var nietTeWijzigenLocatie = scenario.FeitelijkeVerenigingWerdGeregistreerd.Locaties[1];

        var synchronisatieAdres = nietTeWijzigenLocatie.Adres;
        var adresDetailResponse = fixture.Create<AddressDetailResponse>() with
        {
            AdresId = new Registratiedata.AdresId(adresId.Adresbron, adresId.Bronwaarde),
            Gemeente = synchronisatieAdres.Gemeente,
            Busnummer = synchronisatieAdres.Busnummer,
            Huisnummer = synchronisatieAdres.Huisnummer,
            Postcode = synchronisatieAdres.Postcode,
            Straatnaam = synchronisatieAdres.Straatnaam,
            IsActief = true
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
