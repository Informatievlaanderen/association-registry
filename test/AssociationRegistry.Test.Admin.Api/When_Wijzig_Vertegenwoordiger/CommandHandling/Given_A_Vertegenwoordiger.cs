namespace AssociationRegistry.Test.Admin.Api.When_Wijzig_Vertegenwoordiger.CommandHandling;

using Acties.WijzigVertegenwoordiger;
using AssociationRegistry.Framework;
using AutoFixture;
using Events;
using Fakes;
using Fixtures.Scenarios;
using Framework;
using Vereniging.Emails;
using Vereniging.SocialMedias;
using Vereniging.TelefoonNummers;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_A_Vertegenwoordiger
{
    private readonly WijzigVertegenwoordigerCommandHandler _commandHandler;
    private readonly Fixture _fixture;
    private readonly VerenigingWerdGeregistreerdWithAPrimairVertegenwoordigerScenario _scenario;
    private readonly VerenigingRepositoryMock _verenigingRepositoryMock;

    public Given_A_Vertegenwoordiger()
    {
        _fixture = new Fixture().CustomizeAll();

        _scenario = new VerenigingWerdGeregistreerdWithAPrimairVertegenwoordigerScenario();
        _verenigingRepositoryMock = new VerenigingRepositoryMock(_scenario.GetVereniging());

        _commandHandler = new WijzigVertegenwoordigerCommandHandler(_verenigingRepositoryMock);
    }

    [Fact]
    public async Task Then_A_VertegenwoordigerWerdGewijzigd_Event_Is_Saved_With_The_Next_Id()
    {
        var command = new WijzigVertegenwoordigerCommand(
            _scenario.VCode,
            new WijzigVertegenwoordigerCommand.CommandVertegenwoordiger(
                _scenario.VertegenwoordigerWerdToegevoegd.VertegenwoordigerId,
                _fixture.Create<string?>(),
                _fixture.Create<string?>(),
                _fixture.Create<Email>(),
                _fixture.Create<TelefoonNummer>(),
                _fixture.Create<TelefoonNummer>(),
                _fixture.Create<SocialMedia>(),
                IsPrimair: false));

        await _commandHandler.Handle(new CommandEnvelope<WijzigVertegenwoordigerCommand>(command, _fixture.Create<CommandMetadata>()));

        _verenigingRepositoryMock.ShouldHaveSaved(
            new VertegenwoordigerWerdGewijzigd(
                _scenario.VertegenwoordigerWerdToegevoegd.VertegenwoordigerId,
                command.Vertegenwoordiger.Rol,
                command.Vertegenwoordiger.Roepnaam,
                command.Vertegenwoordiger.Email!.Waarde,
                command.Vertegenwoordiger.Telefoon!.Waarde,
                command.Vertegenwoordiger.Mobiel!.Waarde,
                command.Vertegenwoordiger.SocialMedia!.Waarde,
                command.Vertegenwoordiger.IsPrimair
                )
        );
    }
}
