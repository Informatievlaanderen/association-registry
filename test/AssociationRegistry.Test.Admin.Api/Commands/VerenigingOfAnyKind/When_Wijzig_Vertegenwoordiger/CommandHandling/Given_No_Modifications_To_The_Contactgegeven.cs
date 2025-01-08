namespace AssociationRegistry.Test.Admin.Api.Commands.VerenigingOfAnyKind.When_Wijzig_Vertegenwoordiger.CommandHandling;

using AssociationRegistry.Framework;
using AutoFixture;
using Common.AutoFixture;
using Common.Framework;
using Common.Scenarios.CommandHandling;
using DecentraalBeheer.Vertegenwoordigers.WijzigVertegenwoordiger;
using FluentAssertions;
using Vereniging;
using Vereniging.Emails;
using Vereniging.SocialMedias;
using Vereniging.TelefoonNummers;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_No_Modifications_To_The_Vertegenwoordiger : IAsyncLifetime
{
    private readonly WijzigVertegenwoordigerCommandHandler _commandHandler;
    private readonly Fixture _fixture;
    private readonly FeitelijkeVerenigingWerdGeregistreerdWithAPrimairVertegenwoordigerScenario _scenario;
    private readonly VerenigingRepositoryMock _verenigingRepositoryMock;
    private CommandResult _commandResult = null!;

    public Given_No_Modifications_To_The_Vertegenwoordiger()
    {
        _scenario = new FeitelijkeVerenigingWerdGeregistreerdWithAPrimairVertegenwoordigerScenario();
        _verenigingRepositoryMock = new VerenigingRepositoryMock(_scenario.GetVerenigingState());

        _fixture = new Fixture().CustomizeAdminApi();

        _commandHandler = new WijzigVertegenwoordigerCommandHandler(_verenigingRepositoryMock);
    }

    public async Task InitializeAsync()
    {
        var command = new WijzigVertegenwoordigerCommand(
            _scenario.VCode,
            new WijzigVertegenwoordigerCommand.CommandVertegenwoordiger(
                _scenario.VertegenwoordigerWerdToegevoegd.VertegenwoordigerId,
                _scenario.VertegenwoordigerWerdToegevoegd.Rol,
                _scenario.VertegenwoordigerWerdToegevoegd.Roepnaam,
                Email.Create(_scenario.VertegenwoordigerWerdToegevoegd.Email),
                TelefoonNummer.Create(_scenario.VertegenwoordigerWerdToegevoegd.Telefoon),
                TelefoonNummer.Create(_scenario.VertegenwoordigerWerdToegevoegd.Mobiel),
                SocialMedia.Create(_scenario.VertegenwoordigerWerdToegevoegd.SocialMedia),
                _scenario.VertegenwoordigerWerdToegevoegd.IsPrimair));

        _commandResult =
            await _commandHandler.Handle(new CommandEnvelope<WijzigVertegenwoordigerCommand>(command, _fixture.Create<CommandMetadata>()));
    }

    [Fact]
    public void Then_No_Event_Is_Saved()
    {
        _verenigingRepositoryMock.ShouldNotHaveAnySaves();
    }

    [Fact]
    public void Then_CommandResult_Has_No_Changes()
    {
        _commandResult.HasChanges().Should().BeFalse();
    }

    public Task DisposeAsync()
        => Task.CompletedTask;
}
