namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Vertegenwoordigers.VerenigingOfAnyKind.When_Wijzig_Vertegenwoordiger.CommandHandling;

using AssociationRegistry.DecentraalBeheer.Vertegenwoordigers.WijzigVertegenwoordiger;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Framework;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling.FeitelijkeVereniging;
using AssociationRegistry.Vereniging;
using AssociationRegistry.Vereniging.Emails;
using AssociationRegistry.Vereniging.SocialMedias;
using AssociationRegistry.Vereniging.TelefoonNummers;
using AutoFixture;
using FluentAssertions;
using Xunit;

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

    public async ValueTask InitializeAsync()
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

    public ValueTask DisposeAsync()
        => ValueTask.CompletedTask;
}
