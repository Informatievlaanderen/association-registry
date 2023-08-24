﻿namespace AssociationRegistry.Test.Admin.Api.VerenigingOfAnyKind.When_Wijzig_Contactgegeven.CommandHandling;

using Acties.WijzigContactgegeven;
using AssociationRegistry.Framework;
using Fakes;
using AssociationRegistry.Test.Admin.Api.Fixtures.Scenarios.CommandHandling;
using Framework;
using Vereniging.Exceptions;
using AutoFixture;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_Multiple_Primair_Contactgegeven
{
    private readonly WijzigContactgegevenCommandHandler _commandHandler;
    private readonly Fixture _fixture;
    private readonly FeitelijkeVerenigingWerdGeregistreerd_WithMultipleContactgegevens_Commandhandler_Scenario _scenario;

    public Given_Multiple_Primair_Contactgegeven()
    {
        _scenario = new FeitelijkeVerenigingWerdGeregistreerd_WithMultipleContactgegevens_Commandhandler_Scenario();
        var verenigingRepositoryMock = new VerenigingRepositoryMock(_scenario.GetVerenigingState());

        _fixture = new Fixture().CustomizeAdminApi();

        _commandHandler = new WijzigContactgegevenCommandHandler(verenigingRepositoryMock);
    }

    [Fact]
    public async Task Then_A_MultiplePrimaryContactgegevens_Is_Thrown()
    {
        var command = new WijzigContactgegevenCommand(
            _scenario.VCode,
            new WijzigContactgegevenCommand.CommandContactgegeven(
                _scenario.ContactgegevenWerdToegevoegd2.ContactgegevenId,
                _scenario.ContactgegevenWerdToegevoegd2.Waarde,
                _scenario.ContactgegevenWerdToegevoegd2.Beschrijving,
                true)); // <== changed value

        var handle = () => _commandHandler.Handle(new CommandEnvelope<WijzigContactgegevenCommand>(command, _fixture.Create<CommandMetadata>()));
        await handle.Should().ThrowAsync<MultiplePrimairContactgegevens>();
    }
}
