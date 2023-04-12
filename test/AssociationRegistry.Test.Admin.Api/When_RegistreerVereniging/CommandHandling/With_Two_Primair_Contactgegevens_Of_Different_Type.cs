namespace AssociationRegistry.Test.Admin.Api.When_RegistreerVereniging.CommandHandling;

using AssociationRegistry.Framework;
using Magda;
using Primitives;
using Fakes;
using Framework;
using Vereniging.DuplicateDetection;
using Vereniging.RegistreerVereniging;
using AutoFixture;
using ContactGegevens;
using Events;
using Moq;
using Startdatums;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_Two_Primair_Contactgegevens_Of_Different_Type : IAsyncLifetime
{
    private readonly CommandEnvelope<RegistreerVerenigingCommand> _commandEnvelope;
    private readonly RegistreerVerenigingCommandHandler _commandHandler;
    private VerenigingRepositoryMock _repositoryMock;
    private InMemorySequentialVCodeService _vCodeService;

    public With_Two_Primair_Contactgegevens_Of_Different_Type()
    {
        var fixture = new Fixture().CustomizeAll();
        _repositoryMock = new VerenigingRepositoryMock();
        var today = fixture.Create<DateOnly>();

        var command = new RegistreerVerenigingCommand(
            fixture.Create<string>(),
            null,
            null,
            Startdatum.Leeg,
            null,
            new[]
            {
                new RegistreerVerenigingCommand.Contactgegeven(ContactgegevenType.Email, "test@example.org", fixture.Create<string>(), true),
                new RegistreerVerenigingCommand.Contactgegeven(ContactgegevenType.Website, "http://example.org", fixture.Create<string>(), true),
            },
            Array.Empty<RegistreerVerenigingCommand.Locatie>(),
            Array.Empty<RegistreerVerenigingCommand.Vertegenwoordiger>(),
            Array.Empty<string>(),
            true);

        var commandMetadata = fixture.Create<CommandMetadata>();
        _vCodeService = new InMemorySequentialVCodeService();
        _commandHandler = new RegistreerVerenigingCommandHandler(
            _repositoryMock,
            _vCodeService,
            Mock.Of<IMagdaFacade>(),
            Mock.Of<IDuplicateDetectionService>(),
            new ClockStub(today));

        _commandEnvelope = new CommandEnvelope<RegistreerVerenigingCommand>(command, commandMetadata);
    }

    public async Task InitializeAsync()
        => await _commandHandler.Handle(_commandEnvelope, CancellationToken.None);


    [Fact]
    public void Then_it_saves_the_event()
    {
        _repositoryMock.ShouldHaveSaved(
            new VerenigingWerdGeregistreerd(
                _vCodeService.GetLast(),
                _commandEnvelope.Command.Naam,
                null,
                null,
                null,
                null,
                new[]
                {
                    new VerenigingWerdGeregistreerd.Contactgegeven(
                        1,
                        ContactgegevenType.Email,
                        _commandEnvelope.Command.Contactgegevens[0].Waarde,
                        _commandEnvelope.Command.Contactgegevens[0].Omschrijving ?? string.Empty,
                        _commandEnvelope.Command.Contactgegevens[0].IsPrimair
                    ),
                    new VerenigingWerdGeregistreerd.Contactgegeven(
                        2,
                        ContactgegevenType.Website,
                        _commandEnvelope.Command.Contactgegevens[1].Waarde,
                        _commandEnvelope.Command.Contactgegevens[1].Omschrijving ?? string.Empty,
                        _commandEnvelope.Command.Contactgegevens[1].IsPrimair
                    ),
                },
                Array.Empty<VerenigingWerdGeregistreerd.Locatie>(),
                Array.Empty<VerenigingWerdGeregistreerd.Vertegenwoordiger>(),
                Array.Empty<VerenigingWerdGeregistreerd.HoofdactiviteitVerenigingsloket>()));
    }

    public Task DisposeAsync()
        => Task.CompletedTask;
}
