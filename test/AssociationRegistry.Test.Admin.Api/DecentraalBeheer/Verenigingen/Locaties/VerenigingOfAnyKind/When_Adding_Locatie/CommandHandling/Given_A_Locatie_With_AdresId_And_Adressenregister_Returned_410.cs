namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Locaties.VerenigingOfAnyKind.When_Adding_Locatie.CommandHandling;

using AssociationRegistry.DecentraalBeheer.Locaties.VoegLocatieToe;
using AssociationRegistry.Framework;
using AssociationRegistry.Grar.Clients;
using AssociationRegistry.Grar.Exceptions;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Framework;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling.FeitelijkeVereniging;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;
using AssociationRegistry.Vereniging;
using AutoFixture;
using Common.StubsMocksFakes.Faktories;
using Marten;
using Moq;
using Vereniging.Geotags;
using Wolverine.Marten;
using Xunit;

public class Given_A_Locatie_With_AdresId_And_Adressenregister_Returned_410
{
    private readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_With_AllFields_Scenario _scenario;
    private Geotag[] _geotags;
    private readonly Fixture _fixture;
    private VoegLocatieToeCommandHandler _commandHandler;
    private readonly Mock<IGrarClient> _grarClient;

    public Given_A_Locatie_With_AdresId_And_Adressenregister_Returned_410()
    {
        _fixture = new Fixture().CustomizeAdminApi();

        var factory = new Faktory(_fixture);

        _scenario = new VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_With_AllFields_Scenario();
        var verenigingRepositoryMock = factory.VerenigingsRepository.Mock(_scenario.GetVerenigingState());

        (var geotagsService, _geotags) = factory.GeotagsService.ReturnsRandomGeotags([_scenario.MaatschappelijkeZetelWerdOvergenomenUitKbo.Locatie.Adres.Postcode], Array.Empty<string>());

        _grarClient = new Mock<IGrarClient>();

        _commandHandler = new VoegLocatieToeCommandHandler(verenigingRepositoryMock,
                                                           Mock.Of<IMartenOutbox>(),
                                                           Mock.Of<IDocumentSession>(),
                                                           _grarClient.Object,
                                                           geotagsService.Object
        );

    }


    [Theory]
    [MemberData(nameof(Data))]
    public async ValueTask Then_Throws_AdressenregisterReturnedGoneStatusCode(CommandhandlerScenarioBase scenario, int expectedLocatieId)
    {

        var adresId = _fixture.Create<AdresId>();

        var locatie = _fixture.Create<Locatie>() with
        {
            AdresId = adresId,
            Adres = null,
        };

        var command = new VoegLocatieToeCommand(scenario.VCode, locatie);

        _grarClient.Setup(s => s.GetAddressById(adresId.ToString(), It.IsAny<CancellationToken>()))
                  .ThrowsAsync(new AdressenregisterReturnedGoneStatusCode());

        await Assert.ThrowsAsync<AdressenregisterReturnedGoneStatusCode>(
            async () => await _commandHandler.Handle(
                new CommandEnvelope<VoegLocatieToeCommand>(command, _fixture.Create<CommandMetadata>())));
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
