namespace AssociationRegistry.Test.Admin.Api.Commands.VerenigingOfAnyKind.When_Adding_Contactgegeven.CommandHandling;

using AssociationRegistry.Acties.VoegContactgegevenToe;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Admin.Api.Framework;
using AssociationRegistry.Test.Common.Framework;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling;
using AssociationRegistry.Vereniging.Exceptions;
using AutoFixture;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_A_Duplicate_Contactgegeven
{
    private readonly VoegContactgegevenToeCommandHandler _commandHandler;
    private readonly Fixture _fixture;
    private readonly FeitelijkeVerenigingWerdGeregistreerdScenario _scenario;

    public Given_A_Duplicate_Contactgegeven()
    {
        _scenario = new FeitelijkeVerenigingWerdGeregistreerdScenario();
        var verenigingRepositoryMock = new VerenigingRepositoryMock(_scenario.GetVerenigingState());

        _fixture = new Fixture().CustomizeAdminApi();

        _commandHandler = new VoegContactgegevenToeCommandHandler(verenigingRepositoryMock);
    }

    [Fact]
    public async Task Then_A_DuplicateContactgegeven_Is_Thrown()
    {
        var command = _fixture.Create<VoegContactgegevenToeCommand>() with { VCode = _scenario.VCode };

        await _commandHandler.Handle(new CommandEnvelope<VoegContactgegevenToeCommand>(command, _fixture.Create<CommandMetadata>()));

        var handleCall = async ()
            => await _commandHandler.Handle(new CommandEnvelope<VoegContactgegevenToeCommand>(command, _fixture.Create<CommandMetadata>()));

        await handleCall.Should()
                        .ThrowAsync<ContactgegevenIsDuplicaat>()
                        .WithMessage(new ContactgegevenIsDuplicaat(command.Contactgegeven.Contactgegeventype).Message);
    }
}
