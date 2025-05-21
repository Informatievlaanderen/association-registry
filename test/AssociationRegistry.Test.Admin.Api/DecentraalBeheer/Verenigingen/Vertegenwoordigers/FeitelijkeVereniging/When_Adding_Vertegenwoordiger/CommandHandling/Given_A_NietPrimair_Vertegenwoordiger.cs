namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Vertegenwoordigers.FeitelijkeVereniging.When_Adding_Vertegenwoordiger.CommandHandling;

using AssociationRegistry.DecentraalBeheer.Vertegenwoordigers.VoegVertegenwoordigerToe;
using AssociationRegistry.Events;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Framework;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling.FeitelijkeVereniging;
using AssociationRegistry.Vereniging;
using AutoFixture;
using Common.StubsMocksFakes.VerenigingsRepositories;
using FluentAssertions;
using Xunit;

public class Given_A_NietPrimair_Vertegenwoordiger
{
    private VerenigingRepositoryMock _verenigingRepositoryMock;
    private VoegVertegenwoordigerToeCommand _command;
    private FeitelijkeVerenigingWerdGeregistreerdScenario _scenario;
    private VoegVertegenwoordigerToeCommandHandler _commandHandler;
    private Fixture _fixture;

    public Given_A_NietPrimair_Vertegenwoordiger()
    {
        _scenario = new FeitelijkeVerenigingWerdGeregistreerdScenario();
        _verenigingRepositoryMock = new VerenigingRepositoryMock(_scenario.GetVerenigingState());

        _fixture = new Fixture().CustomizeAdminApi();

        _commandHandler = new VoegVertegenwoordigerToeCommandHandler(_verenigingRepositoryMock);

        _command = new VoegVertegenwoordigerToeCommand(
            _scenario.VCode,
            _fixture.Create<Vertegenwoordiger>());
    }

    [Fact]
    public async ValueTask Then_A_VertegenwoordigerWerdToegevoegd_Event_Is_Saved()
    {
        await _commandHandler
           .Handle(new CommandEnvelope<VoegVertegenwoordigerToeCommand>(_command, _fixture.Create<CommandMetadata>()));

        _verenigingRepositoryMock.ShouldHaveSaved(
            new VertegenwoordigerWerdToegevoegd(
                _scenario.FeitelijkeVerenigingWerdGeregistreerd.Vertegenwoordigers.Max(v => v.VertegenwoordigerId) + 1,
                _command.Vertegenwoordiger.Insz,
                _command.Vertegenwoordiger.IsPrimair,
                _command.Vertegenwoordiger.Roepnaam ?? string.Empty,
                _command.Vertegenwoordiger.Rol ?? string.Empty,
                _command.Vertegenwoordiger.Voornaam,
                _command.Vertegenwoordiger.Achternaam,
                _command.Vertegenwoordiger.Email.Waarde,
                _command.Vertegenwoordiger.Telefoon.Waarde,
                _command.Vertegenwoordiger.Mobiel.Waarde,
                _command.Vertegenwoordiger.SocialMedia.Waarde)
        );
    }

    [Fact]
    public async ValueTask Then_A_EntityId_Is_Returned()
    {
        var result = await _commandHandler
           .Handle(new CommandEnvelope<VoegVertegenwoordigerToeCommand>(_command, _fixture.Create<CommandMetadata>()));

        var vertegenwoordigerId = _verenigingRepositoryMock.SaveInvocations[0].Vereniging.UncommittedEvents.ToArray()[0]
                                                           .As<VertegenwoordigerWerdToegevoegd>().VertegenwoordigerId;

        result.EntityId.Should().Be(vertegenwoordigerId);
    }
}
