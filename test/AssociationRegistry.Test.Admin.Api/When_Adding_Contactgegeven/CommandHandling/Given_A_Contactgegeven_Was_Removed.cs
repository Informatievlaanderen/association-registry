﻿namespace AssociationRegistry.Test.Admin.Api.When_Adding_Contactgegeven.CommandHandling;

using Acties.VoegContactgegevenToe;
using AssociationRegistry.Framework;
using AutoFixture;
using Events;
using Fakes;
using Fixtures.Scenarios;
using Framework;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_A_Contactgegeven_Was_Removed
{
    private readonly VerenigingRepositoryMock _verenigingRepositoryMock;
    private readonly VoegContactgegevenToeCommandHandler _commandHandler;
    private readonly Fixture _fixture;
    private readonly VerenigingWerdGeregistreerdWithRemovedContactgegeven_Commandhandler_ScenarioBase _scenario;

    public Given_A_Contactgegeven_Was_Removed()
    {
        _scenario = new VerenigingWerdGeregistreerdWithRemovedContactgegeven_Commandhandler_ScenarioBase();
        _verenigingRepositoryMock = new VerenigingRepositoryMock(_scenario.GetVereniging());

        _fixture = new Fixture().CustomizeAll();

        _commandHandler = new VoegContactgegevenToeCommandHandler(_verenigingRepositoryMock);
    }

    [Fact]
    public async Task Then_A_ContactgegevenWerdToegevoegd_Event_Is_Saved_With_The_Next_Id()
    {
        var command = _fixture.Create<VoegContactgegevenToeCommand>() with { VCode = _scenario.VCode };

        await _commandHandler.Handle(new CommandEnvelope<VoegContactgegevenToeCommand>(command, _fixture.Create<CommandMetadata>()));

        _verenigingRepositoryMock.ShouldHaveSaved(
            new ContactgegevenWerdToegevoegd(2, command.Contactgegeven.Type, command.Contactgegeven.Waarde, command.Contactgegeven.Omschrijving ?? string.Empty, false)
        );
    }
}
