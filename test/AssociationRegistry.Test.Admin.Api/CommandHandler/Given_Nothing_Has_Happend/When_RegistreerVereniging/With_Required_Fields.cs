namespace AssociationRegistry.Test.Admin.Api.CommandHandler.Given_A_VerenigingWerdGeregistreerd.When_RegistreerVereniging;

using AssociationRegistry.Framework;
using AutoFixture;
using Events;
using Scenarios;
using Vereniging.RegistreerVereniging;
using Xunit;

public class With_Required_Fields : IClassFixture<Given_A_Scenario_CommandHandlerFixture<EmptyScenario>>
{
    private const string Naam = "naam1";

    private readonly VerenigingRepositoryMock _verenigingRepositoryMock;
    private readonly DateTime _today;
    private readonly InMemorySequentialVCodeService _vCodeService;

    public With_Required_Fields(Given_A_Scenario_CommandHandlerFixture<EmptyScenario> classFixture)
    {
        _verenigingRepositoryMock = classFixture.VerenigingRepositoryMock;
        _vCodeService = new InMemorySequentialVCodeService();

        var fixture = new Fixture();
        _today = fixture.Create<DateTime>();

        var clock = new ClockStub(_today);

        var command = new RegistreerVerenigingCommand(
            Naam,
            null,
            null,
            null,
            null,
            Array.Empty<RegistreerVerenigingCommand.ContactInfo>(),
            Array.Empty<RegistreerVerenigingCommand.Locatie>());
        var commandMetadata = fixture.Create<CommandMetadata>();
        var commandHandler = new RegistreerVerenigingCommandHandler(_verenigingRepositoryMock, _vCodeService, clock);

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
                _vCodeService.GetLast(),
                Naam,
                null,
                null,
                null,
                null,
                Array.Empty<VerenigingWerdGeregistreerd.ContactInfo>(),
                Array.Empty< VerenigingWerdGeregistreerd.Locatie>(),
                DateOnly.FromDateTime(_today)));
    }
}
