namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Locaties.VerenigingOfAnyKind.When_Adding_Locatie.CommandHandling;

using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Locaties.VoegLocatieToe;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.DecentraalBeheer.Vereniging.Adressen;
using AssociationRegistry.Framework;
using AssociationRegistry.Grar;
using AssociationRegistry.Grar.Exceptions;
using AssociationRegistry.Resources;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Framework;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling.FeitelijkeVereniging;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;
using AssociationRegistry.Vereniging;
using AutoFixture;
using Common.StubsMocksFakes.Faktories;
using Common.StubsMocksFakes.VerenigingsRepositories;
using FluentAssertions;
using AssociationRegistry.Integrations.Grar.Clients;
using Integrations.Grar.Exceptions;
using Marten;
using Moq;
using System.Net;
using Wolverine.Marten;
using Xunit;

public class Given_A_Locatie_With_AdresId_And_Adressenregister_Returned_ClientErrorStatusCode
{
    [Theory]
    [MemberData(nameof(Data))]
    public async ValueTask Then_Throws_AdressenregisterReturnedClientErrorStatusCode(CommandhandlerScenarioBase scenario, int expectedLocatieId)
    {
        var fixture = new Fixture().CustomizeAdminApi();

        var factory = new Faktory(fixture);

        var verenigingRepositoryMock = factory.VerenigingsRepository.Mock(scenario.GetVerenigingState());

        (var geotagsService, _) = factory.GeotagsService.ReturnsRandomGeotags();

        var grarClient = new Mock<IGrarClient>();

        var commandHandler = new VoegLocatieToeCommandHandler(verenigingRepositoryMock,
                                                              Mock.Of<IMartenOutbox>(),
                                                              Mock.Of<IDocumentSession>(),
                                                              grarClient.Object,
                                                              geotagsService.Object

        );

        var adresId = fixture.Create<AdresId>();

        var locatie = fixture.Create<Locatie>() with
        {
            AdresId = adresId,
            Adres = null,
        };

        var command = new VoegLocatieToeCommand(scenario.VCode, locatie);

        grarClient.Setup(s => s.GetAddressById(adresId.ToString(), It.IsAny<CancellationToken>()))
                  .ThrowsAsync(new AdressenregisterReturnedClientErrorStatusCode(HttpStatusCode.InternalServerError, ExceptionMessages.AdresKonNietGevalideerdWordenBijAdressenregister));

         var exception = await Assert.ThrowsAsync<AdressenregisterReturnedClientErrorStatusCode>(
            async () => await commandHandler.Handle(
                new CommandEnvelope<VoegLocatieToeCommand>(command, fixture.Create<CommandMetadata>())));

         exception.Message.Should().Be(ExceptionMessages.AdresKonNietGevalideerdWordenBijAdressenregister);
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
