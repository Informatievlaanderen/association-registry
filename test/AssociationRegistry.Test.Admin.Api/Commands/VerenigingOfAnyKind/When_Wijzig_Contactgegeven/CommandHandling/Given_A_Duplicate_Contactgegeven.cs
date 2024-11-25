namespace AssociationRegistry.Test.Admin.Api.Commands.VerenigingOfAnyKind.When_Wijzig_Contactgegeven.CommandHandling;

using Acties.WijzigContactgegeven;
using AssociationRegistry.Framework;
using AutoFixture;
using Common.AutoFixture;
using Common.Framework;
using Common.Scenarios.CommandHandling;
using FluentAssertions;
using Framework;
using Vereniging.Exceptions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_A_Duplicate_Contactgegeven
{
    private readonly WijzigContactgegevenCommandHandler _commandHandler;
    private readonly Fixture _fixture;
    private readonly FeitelijkeVerenigingWerdGeregistreerd_WithMultipleContactgegevens_Commandhandler_Scenario _scenario;

    public Given_A_Duplicate_Contactgegeven()
    {
        _scenario = new FeitelijkeVerenigingWerdGeregistreerd_WithMultipleContactgegevens_Commandhandler_Scenario();
        var verenigingRepositoryMock = new VerenigingRepositoryMock(_scenario.GetVerenigingState());

        _fixture = new Fixture().CustomizeAdminApi();

        _commandHandler = new WijzigContactgegevenCommandHandler(verenigingRepositoryMock);
    }

    [Fact]
    public async Task Then_A_Duplicate_Contactgegeven_Exception_Is_Thrown()
    {
        var command = new WijzigContactgegevenCommand(
            _scenario.VCode,
            new WijzigContactgegevenCommand.CommandContactgegeven(
                _scenario.ContactgegevenWerdToegevoegd2.ContactgegevenId,
                _scenario.ContactgegevenWerdToegevoegd1.Waarde, // <== changed value
                _scenario.ContactgegevenWerdToegevoegd2.Beschrijving,
                _scenario.ContactgegevenWerdToegevoegd2.IsPrimair));

        var handle = ()
            => _commandHandler.Handle(new CommandEnvelope<WijzigContactgegevenCommand>(command, _fixture.Create<CommandMetadata>()));

        await handle.Should().ThrowAsync<ContactgegevenIsDuplicaat>();
    }
}
