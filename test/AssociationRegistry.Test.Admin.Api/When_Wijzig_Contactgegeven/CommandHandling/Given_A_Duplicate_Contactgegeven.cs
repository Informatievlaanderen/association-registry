namespace AssociationRegistry.Test.Admin.Api.When_Wijzig_Contactgegeven.CommandHandling;

using AssociationRegistry.Framework;
using AutoFixture;
using ContactGegevens;
using ContactGegevens.Emails;
using ContactGegevens.Exceptions;
using Events;
using Fakes;
using Fixtures.Scenarios;
using FluentAssertions;
using Framework;
using Vereniging.VoegContactgegevenToe;
using Vereniging.WijzigContactgegeven;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_A_Duplicate_Contactgegeven
{
    private readonly WijzigContactgegevenCommandHandler _commandHandler;
    private readonly Fixture _fixture;
    private readonly VerenigingWerdGeregistreerd_WithMultipleContactgegevens_Commandhandler_Scenario _scenario;
    private readonly VerenigingRepositoryMock _verenigingRepositoryMock;

    public Given_A_Duplicate_Contactgegeven()
    {
        _scenario = new VerenigingWerdGeregistreerd_WithMultipleContactgegevens_Commandhandler_Scenario();
        _verenigingRepositoryMock = new VerenigingRepositoryMock(_scenario.GetVereniging());

        _fixture = new Fixture().CustomizeAll();

        _commandHandler = new WijzigContactgegevenCommandHandler(_verenigingRepositoryMock);
    }

    [Fact]
    public async Task Then_A_Duplicate_Contactgegeven_Exception_Is_Thrown()
    {
        var command = new WijzigContactgegevenCommand(
            _scenario.VCode,
            new WijzigContactgegevenCommand.CommandContactgegeven(
                _scenario.ContactgegevenWerdToegevoegd2.ContactgegevenId,
                _scenario.ContactgegevenWerdToegevoegd1.Waarde, // <== changed value
                _scenario.ContactgegevenWerdToegevoegd2.Omschrijving,
                _scenario.ContactgegevenWerdToegevoegd2.IsPrimair));

        var handle = () => _commandHandler.Handle(new CommandEnvelope<WijzigContactgegevenCommand>(command, _fixture.Create<CommandMetadata>()));
        await handle.Should().ThrowAsync<DuplicateContactgegeven>();
    }
}
