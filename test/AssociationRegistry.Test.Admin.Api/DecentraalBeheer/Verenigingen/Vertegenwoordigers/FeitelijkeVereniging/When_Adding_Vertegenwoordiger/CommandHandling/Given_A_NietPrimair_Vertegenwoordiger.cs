namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Vertegenwoordigers.FeitelijkeVereniging.When_Adding_Vertegenwoordiger.CommandHandling;

using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Vertegenwoordigers.VoegVertegenwoordigerToe;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Events;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Framework;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling.FeitelijkeVereniging;
using AssociationRegistry.Vereniging;
using AutoFixture;
using Common.StubsMocksFakes.VerenigingsRepositories;
using Common.StubsMocksFakes.VertegenwoordigerPersoonsgegevensRepositories;
using FluentAssertions;
using Moq;
using Persoonsgegevens;
using Xunit;
using VertegenwoordigerPersoonsgegevens = Persoonsgegevens.VertegenwoordigerPersoonsgegevens;

public class Given_A_NietPrimair_Vertegenwoordiger
{
    private VerenigingRepositoryMock _verenigingRepositoryMock;
    private VoegVertegenwoordigerToeCommand _command;
    private FeitelijkeVerenigingWerdGeregistreerdScenario _scenario;
    private VoegVertegenwoordigerToeCommandHandler _commandHandler;
    private Fixture _fixture;
    private VertegenwoordigerPersoonsgegevensRepositoryMock _vertegenwoordigerRepositoryMock;
    private readonly EntityCommandResult _result;

    public Given_A_NietPrimair_Vertegenwoordiger()
    {
        _scenario = new FeitelijkeVerenigingWerdGeregistreerdScenario();
        var verenigingState = _scenario.GetVerenigingState();
        _verenigingRepositoryMock = new VerenigingRepositoryMock(verenigingState);
        _vertegenwoordigerRepositoryMock = new VertegenwoordigerPersoonsgegevensRepositoryMock();
        verenigingState.VertegenwoordigerPersoonsgegevensRepository = _vertegenwoordigerRepositoryMock;

        _fixture = new Fixture().CustomizeAdminApi();

        _commandHandler = new VoegVertegenwoordigerToeCommandHandler(_verenigingRepositoryMock, _vertegenwoordigerRepositoryMock);

        _command = new VoegVertegenwoordigerToeCommand(
            _scenario.VCode,
            _fixture.Create<Vertegenwoordiger>());

        _result = _commandHandler
           .Handle(new CommandEnvelope<VoegVertegenwoordigerToeCommand>(_command, _fixture.Create<CommandMetadata>()))
           .GetAwaiter().GetResult();
    }

    [Fact]
    public async ValueTask Then_A_VertegenwoordigerWerdToegevoegd_Event_Is_Saved()
    {
        _verenigingRepositoryMock.ShouldHaveSavedExact(
            new VertegenwoordigerWerdToegevoegd(
                _vertegenwoordigerRepositoryMock.SavedRefIds.Last(),
                _scenario.FeitelijkeVerenigingWerdGeregistreerd.Vertegenwoordigers.Max(v => v.VertegenwoordigerId) + 1,
                _command.Vertegenwoordiger.IsPrimair)
        );
    }

    [Fact]
    public async ValueTask Then_It_Should_Have_Saved_A_VerenigingPersoonsgegevensDocument()
    {
        var refId = _vertegenwoordigerRepositoryMock.SavedRefIds.Last();
        var vertegenwoordigerPersoonsgegevens = new VertegenwoordigerPersoonsgegevens(refId,
                                                               VCode.Hydrate(_scenario.VCode),
                                                               _scenario.FeitelijkeVerenigingWerdGeregistreerd.Vertegenwoordigers.Max(v => v.VertegenwoordigerId) + 1,
                                                               _command.Vertegenwoordiger.Insz,
                                                               _command.Vertegenwoordiger.Roepnaam ?? string.Empty,
                                                               _command.Vertegenwoordiger.Rol ?? string.Empty,
                                                               _command.Vertegenwoordiger.Voornaam,
                                                               _command.Vertegenwoordiger.Achternaam,
                                                               _command.Vertegenwoordiger.Email.Waarde,
                                                               _command.Vertegenwoordiger.Telefoon.Waarde,
                                                               _command.Vertegenwoordiger.Mobiel.Waarde,
                                                               _command.Vertegenwoordiger.SocialMedia.Waarde);

        var actualSaved = await _vertegenwoordigerRepositoryMock.Get(refId);
        actualSaved.Should().BeEquivalentTo(vertegenwoordigerPersoonsgegevens);
    }

    [Fact]
    public async ValueTask Then_A_EntityId_Is_Returned()
    {
        var vertegenwoordigerId = _verenigingRepositoryMock.SaveInvocations[0].Vereniging.UncommittedEvents.ToArray()[0]
                                                           .As<VertegenwoordigerWerdToegevoegd>().VertegenwoordigerId;

        _result.EntityId.Should().Be(vertegenwoordigerId);
    }
}
