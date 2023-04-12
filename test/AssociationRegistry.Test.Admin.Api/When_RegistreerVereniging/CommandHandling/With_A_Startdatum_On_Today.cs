namespace AssociationRegistry.Test.Admin.Api.When_RegistreerVereniging.CommandHandling;

using Events;
using AssociationRegistry.Framework;
using Magda;
using Fakes;
using Fixtures;
using Fixtures.Scenarios;
using Vereniging.RegistreerVereniging;
using AutoFixture;
using Framework;
using Moq;
using Primitives;
using Startdatums;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_A_Startdatum_On_Today : IClassFixture<CommandHandlerScenarioFixture<Empty_Commandhandler_ScenarioBase>>
{
    private const string Naam = "naam1";

    private readonly VerenigingRepositoryMock _verenigingRepositoryMock;
    private readonly InMemorySequentialVCodeService _vCodeService;
    private DateOnly _today;

    public With_A_Startdatum_On_Today(CommandHandlerScenarioFixture<Empty_Commandhandler_ScenarioBase> classFixture)
    {
        _verenigingRepositoryMock = classFixture.VerenigingRepositoryMock;
        _vCodeService = new InMemorySequentialVCodeService();

        var fixture = new Fixture().CustomizeAll();
        _today = fixture.Create<DateOnly>();

        var clock = new ClockStub(_today);

        var command = new RegistreerVerenigingCommand(
            Naam,
            null,
            null,
            Startdatum.Create(_today),
            null,
            Array.Empty<RegistreerVerenigingCommand.Contactgegeven>(),
            Array.Empty<RegistreerVerenigingCommand.Locatie>(),
            Array.Empty<RegistreerVerenigingCommand.Vertegenwoordiger>(),
            Array.Empty<string>());
        var commandMetadata = fixture.Create<CommandMetadata>();
        var commandHandler = new RegistreerVerenigingCommandHandler(_verenigingRepositoryMock, _vCodeService, Mock.Of<IMagdaFacade>(), new NoDuplicateDetectionService(), clock);

        commandHandler
            .Handle(new CommandEnvelope<RegistreerVerenigingCommand>(command, commandMetadata), CancellationToken.None)
            .GetAwaiter()
            .GetResult();
    }

    [Fact]
    public void Then_it_saves_the_event()
    {
        _verenigingRepositoryMock.ShouldHaveSaved(
            new VerenigingWerdGeregistreerd(
                VCode: _vCodeService.GetLast(),
                Naam: Naam,
                KorteNaam: null,
                KorteBeschrijving: null,
                Startdatum: _today,
                KboNummer: null,
                Contactgegevens: Array.Empty<VerenigingWerdGeregistreerd.Contactgegeven>(),
                Locaties: Array.Empty<VerenigingWerdGeregistreerd.Locatie>(),
                Vertegenwoordigers: Array.Empty<VerenigingWerdGeregistreerd.Vertegenwoordiger>(),
                HoofdactiviteitenVerenigingsloket: Array.Empty<VerenigingWerdGeregistreerd.HoofdactiviteitVerenigingsloket>()));
    }
}
