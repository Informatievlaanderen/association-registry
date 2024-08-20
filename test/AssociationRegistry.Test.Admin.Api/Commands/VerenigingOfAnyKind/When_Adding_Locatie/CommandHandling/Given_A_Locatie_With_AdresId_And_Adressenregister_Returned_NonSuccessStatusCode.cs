namespace AssociationRegistry.Test.Admin.Api.Commands.VerenigingOfAnyKind.When_Adding_Locatie.CommandHandling;

using AssociationRegistry.Acties.VoegLocatieToe;
using AssociationRegistry.Framework;
using AssociationRegistry.Grar;
using AssociationRegistry.Grar.Exceptions;
using AssociationRegistry.Test.Admin.Api.Framework;
using AssociationRegistry.Test.Common.Framework;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling;
using AssociationRegistry.Vereniging;
using AutoFixture;
using Marten;
using Moq;
using System.Net;
using Wolverine.Marten;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_A_Locatie_With_AdresId_And_Adressenregister_Returned_NonSuccessStatusCode
{
    [Theory]
    [MemberData(nameof(Data))]
    public async Task Then_Throws_AdressenregisterReturnedNonSuccessStatusCode(CommandhandlerScenarioBase scenario, int expectedLocatieId)
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

        var locatie = fixture.Create<Locatie>() with
        {
            AdresId = adresId,
            Adres = null,
        };
        var command = new VoegLocatieToeCommand(scenario.VCode, locatie);

        grarClient.Setup(s => s.GetAddressById(adresId.ToString(), It.IsAny<CancellationToken>()))
                  .ThrowsAsync(new AdressenregisterReturnedNonSuccessStatusCode(HttpStatusCode.InternalServerError));

        await Assert.ThrowsAsync<AdressenregisterReturnedNonSuccessStatusCode>(async () => await commandHandler.Handle(new CommandEnvelope<VoegLocatieToeCommand>(command, fixture.Create<CommandMetadata>())));
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
