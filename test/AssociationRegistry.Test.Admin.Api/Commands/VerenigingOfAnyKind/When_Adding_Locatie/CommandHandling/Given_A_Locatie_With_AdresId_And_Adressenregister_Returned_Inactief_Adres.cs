namespace AssociationRegistry.Test.Admin.Api.Commands.VerenigingOfAnyKind.When_Adding_Locatie.CommandHandling;

using AssociationRegistry.Framework;
using AutoFixture;
using Common.AutoFixture;
using Common.Framework;
using Common.Scenarios.CommandHandling;
using Common.Scenarios.CommandHandling.FeitelijkeVereniging;
using Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;
using DecentraalBeheer.Locaties.VoegLocatieToe;
using Events;
using Grar.Clients;
using Grar.Exceptions;
using Grar.Models;
using Marten;
using Moq;
using Vereniging;
using Wolverine.Marten;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_A_Locatie_With_AdresId_And_Adressenregister_Returned_Inactief_Adres
{
    [Theory]
    [MemberData(nameof(Data))]
    public async Task Then_Throws_AdressenregisterReturnedInactiefAdres(CommandhandlerScenarioBase scenario, int expectedLocatieId)
    {
        var verenigingRepositoryMock = new VerenigingRepositoryMock(scenario.GetVerenigingState());

        var fixture = new Fixture().CustomizeAdminApi();

        var grarClient = new Mock<IGrarClient>();

        var commandHandler = new VoegLocatieToeCommandHandler(verenigingRepositoryMock,
                                                              Mock.Of<IMartenOutbox>(),
                                                              Mock.Of<IDocumentSession>(),
                                                              grarClient.Object
        );

        var adresId = fixture.Create<AdresId>();

        var adresDetailResponse = fixture.Create<AddressDetailResponse>() with
        {
            AdresId = new Registratiedata.AdresId(adresId.Adresbron.Code, adresId.Bronwaarde),
            IsActief = false,
        };

        var locatie = fixture.Create<Locatie>() with
        {
            AdresId = adresId,
            Adres = null,
        };

        var command = new VoegLocatieToeCommand(scenario.VCode, locatie);

        grarClient.Setup(s => s.GetAddressById(adresId.ToString(), It.IsAny<CancellationToken>()))
                  .ReturnsAsync(adresDetailResponse);

        await Assert.ThrowsAsync<AdressenregisterReturnedInactiefAdres>(
            async () => await commandHandler.Handle(
                new CommandEnvelope<VoegLocatieToeCommand>(command, fixture.Create<CommandMetadata>())));
    }

    public static IEnumerable<object[]> Data
    {
        get
        {
            var feitelijkeVerenigingWerdGeregistreerdScenario = new FeitelijkeVerenigingWerdGeregistreerdScenario();

            yield return new object[]
            {
                feitelijkeVerenigingWerdGeregistreerdScenario,
                feitelijkeVerenigingWerdGeregistreerdScenario.FeitelijkeVerenigingWerdGeregistreerd.Locaties.Max(l => l.LocatieId) + 1,
            };

            var verenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario =
                new VerenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario();

            yield return new object[]
            {
                verenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario,
                1,
            };
        }
    }
}
