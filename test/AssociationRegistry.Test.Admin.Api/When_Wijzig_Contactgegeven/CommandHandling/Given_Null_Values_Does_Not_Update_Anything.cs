namespace AssociationRegistry.Test.Admin.Api.When_Wijzig_Contactgegeven.CommandHandling;

using AssociationRegistry.Framework;
using AutoFixture;
using ContactGegevens;
using ContactGegevens.Emails;
using Events;
using Fakes;
using Fixtures.Scenarios;
using Framework;
using Vereniging.WijzigContactgegeven;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_Null_Values_Does_Not_Update_Anything
{
    private readonly WijzigContactgegevenCommandHandler _commandHandler;
    private readonly Fixture _fixture;
    private readonly VerenigingWerdGeregistreerd_WithAPrimairEmailContactgegeven_Commandhandler_Scenario _scenario;
    private readonly VerenigingRepositoryMock _verenigingRepositoryMock;

    public Given_Null_Values_Does_Not_Update_Anything()
    {
        _scenario = new VerenigingWerdGeregistreerd_WithAPrimairEmailContactgegeven_Commandhandler_Scenario();
        _verenigingRepositoryMock = new VerenigingRepositoryMock(_scenario.GetVereniging());

        _fixture = new Fixture().CustomizeAll();

        _commandHandler = new WijzigContactgegevenCommandHandler(_verenigingRepositoryMock);
    }

    [Fact]
    public async Task Then_It_Does_Not_Update_Anything()
    {
        var command = new WijzigContactgegevenCommand(
            _scenario.VCode,
            new WijzigContactgegevenCommand.CommandContactgegeven(
                ContacgegevenId: _scenario.ContactgegevenId,
                Waarde: null,
                Omschrijving: null,
                IsPrimair: null));

        await _commandHandler.Handle(new CommandEnvelope<WijzigContactgegevenCommand>(command, _fixture.Create<CommandMetadata>()));

        _verenigingRepositoryMock.ShouldNotHaveAnySaves();
    }
}

[UnitTest]
public class Given_Null_For_Omschrijving_Does_Not_Update_Omschrijving
{
    private readonly WijzigContactgegevenCommandHandler _commandHandler;
    private readonly Fixture _fixture;
    private readonly VerenigingWerdGeregistreerd_WithAPrimairEmailContactgegeven_Commandhandler_Scenario _scenario;
    private readonly VerenigingRepositoryMock _verenigingRepositoryMock;

    public Given_Null_For_Omschrijving_Does_Not_Update_Omschrijving()
    {
        _scenario = new VerenigingWerdGeregistreerd_WithAPrimairEmailContactgegeven_Commandhandler_Scenario();
        _verenigingRepositoryMock = new VerenigingRepositoryMock(_scenario.GetVereniging());

        _fixture = new Fixture().CustomizeAll();

        _commandHandler = new WijzigContactgegevenCommandHandler(_verenigingRepositoryMock);
    }

    [Fact]
    public async Task Then_It_Does_Not_Update_Anything()
    {
        var command = new WijzigContactgegevenCommand(
            _scenario.VCode,
            new WijzigContactgegevenCommand.CommandContactgegeven(
                ContacgegevenId: _scenario.ContactgegevenId,
                Waarde: _fixture.Create<Email>().Waarde,
                Omschrijving: null,
                IsPrimair: true));

        await _commandHandler.Handle(new CommandEnvelope<WijzigContactgegevenCommand>(command, _fixture.Create<CommandMetadata>()));

        _verenigingRepositoryMock.ShouldHaveSaved(
            new ContactgegevenWerdGewijzigd(
                ContactgegevenId: _scenario.ContactgegevenId,
                ContactgegevenType.Email,
                command.Contactgegeven.Waarde!,
                _scenario.Omschrijving, // <== this must stay the same
                IsPrimair: true)
        );
    }
}
