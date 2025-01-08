namespace AssociationRegistry.Test.Admin.Api.Commands.VerenigingOfAnyKind.When_Adding_Locatie.CommandHandling;

using AssociationRegistry.Framework;
using AutoFixture;
using Common.AutoFixture;
using Common.Framework;
using Common.Scenarios.CommandHandling;
using DecentraalBeheer.Locaties.VoegLocatieToe;
using Events;
using Grar;
using Grar.Clients;
using Grar.Models;
using Marten;
using Moq;
using Vereniging;
using Vereniging.Exceptions;
using Wolverine.Marten;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_An_Existing_Locatie_Without_AdresId
{
    [Fact]
    public async Task Then_A_LocatieWerdToegevoegd_Event_And_AdresWerdOvergenomenUitAdressenregister_Is_Saved()
    {
        var scenario = new FeitelijkeVerenigingWerdGeregistreerdWithALocatieScenario();
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
        var scenarioAdres = scenario.LocatieWerdToegevoegd.Locatie.Adres;

        var adresDetailResponse = fixture.Create<AddressDetailResponse>() with
        {
            AdresId = new Registratiedata.AdresId(adresId.Adresbron.Code, adresId.Bronwaarde),
            Gemeente = scenarioAdres.Gemeente,
            Busnummer = scenarioAdres.Busnummer,
            Huisnummer = scenarioAdres.Huisnummer,
            Postcode = scenarioAdres.Postcode,
            Straatnaam = scenarioAdres.Straatnaam,
            IsActief = true,
        };

        var locatie = fixture.Create<Locatie>() with
        {
            AdresId = adresId,
            Naam = scenario.LocatieWerdToegevoegd.Locatie.Naam,
            Locatietype = scenario.LocatieWerdToegevoegd.Locatie.Locatietype,
            Adres = null,
        };

        var command = new VoegLocatieToeCommand(scenario.VCode, locatie);

        grarClient.Setup(s => s.GetAddressById(adresId.ToString(), It.IsAny<CancellationToken>()))
                  .ReturnsAsync(adresDetailResponse);

        await Assert.ThrowsAsync<LocatieIsNietUniek>(
            async () =>
                await commandHandler.Handle(new CommandEnvelope<VoegLocatieToeCommand>(command, fixture.Create<CommandMetadata>())));
    }
}
