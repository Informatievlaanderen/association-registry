﻿namespace AssociationRegistry.Test.Admin.Api.When_Adding_Contactgegeven.CommandHandling;

using Acties.VoegContactgegevenToe;
using AssociationRegistry.Framework;
using AutoFixture;
using Fakes;
using Fixtures.Scenarios;
using Fixtures.Scenarios.CommandHandling;
using FluentAssertions;
using Framework;
using Vereniging;
using Vereniging.Exceptions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_A_Second_Primair_Contactgegeven_Of_The_Same_Type
{
    private readonly VoegContactgegevenToeCommandHandler _commandHandler;
    private readonly Fixture _fixture;
    private readonly FeitelijkeVerenigingWerdGeregistreerdWithAPrimairEmailContactgegevenScenario _scenario;

    public Given_A_Second_Primair_Contactgegeven_Of_The_Same_Type()
    {
        _scenario = new FeitelijkeVerenigingWerdGeregistreerdWithAPrimairEmailContactgegevenScenario();
        var verenigingRepositoryMock = new VerenigingRepositoryMock(_scenario.GetVereniging());

        _fixture = new Fixture().CustomizeAll();

        _commandHandler = new VoegContactgegevenToeCommandHandler(verenigingRepositoryMock);
    }

    [Fact]
    public async Task Then_A_MultiplePrimaryContactgegevens_Is_Thrown()
    {
        var command = new VoegContactgegevenToeCommand(
            _scenario.VCode,
            Contactgegeven.Create(
                ContactgegevenType.Email,
                "test2@example.org",
                _fixture.Create<string?>(),
                true));

        var handleCall = async () => await _commandHandler.Handle(new CommandEnvelope<VoegContactgegevenToeCommand>(command, _fixture.Create<CommandMetadata>()));

        await handleCall.Should()
            .ThrowAsync<MultiplePrimaryContactgegevens>()
            .WithMessage(new MultiplePrimaryContactgegevens(ContactgegevenType.Email.ToString()).Message);
    }
}
