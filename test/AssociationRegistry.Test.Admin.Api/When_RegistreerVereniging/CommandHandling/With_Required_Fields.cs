namespace AssociationRegistry.Test.Admin.Api.When_RegistreerVereniging.CommandHandling;

using Events;
using AssociationRegistry.Framework;
using Magda;
using Fakes;
using Fixtures;
using Fixtures.Scenarios;
using Vereniging.RegistreerVereniging;
using AutoFixture;
using Moq;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_Required_Fields : IClassFixture<CommandHandlerScenarioFixture<Empty_Commandhandler_Scenario>>
{
    private const string Naam = "naam1";

    private readonly VerenigingRepositoryMock _verenigingRepositoryMock;
    private readonly InMemorySequentialVCodeService _vCodeService;

    public With_Required_Fields(CommandHandlerScenarioFixture<Empty_Commandhandler_Scenario> classFixture)
    {
        _verenigingRepositoryMock = classFixture.VerenigingRepositoryMock;
        _vCodeService = new InMemorySequentialVCodeService();

        var fixture = new Fixture();
        var today = fixture.Create<DateTime>();

        var clock = new ClockStub(today);

        var command = new RegistreerVerenigingCommand(
            Naam,
            null,
            null,
            null,
            null,
            Array.Empty<RegistreerVerenigingCommand.ContactInfo>(),
            Array.Empty<RegistreerVerenigingCommand.Locatie>(),
            Array.Empty<RegistreerVerenigingCommand.Vertegenwoordiger>());
        var commandMetadata = fixture.Create<CommandMetadata>();
        var commandHandler = new RegistreerVerenigingCommandHandler(_verenigingRepositoryMock, _vCodeService, Mock.Of<IMagdaFacade>(), clock);

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
                Array.Empty<VerenigingWerdGeregistreerd.Locatie>(),
                Array.Empty<VerenigingWerdGeregistreerd.Vertegenwoordiger>()));
    }
}
