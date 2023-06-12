﻿namespace AssociationRegistry.Test.Admin.Api.FeitelijkeVereniging.When_Removing_Vertegenwoordiger.CommandHandling;

using Acties.VerwijderVertegenwoordiger;
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
public class With_An_Unknown_VertegenwoordigerId
{
    private readonly FeitelijkeVerenigingWerdGeregistreerdWithoutVertegenwoordigers _scenario;
    private readonly VerwijderVertegenwoordigerCommandHandler _commandHandler;
    private readonly Fixture _fixture;

    public With_An_Unknown_VertegenwoordigerId()
    {
        _scenario = new FeitelijkeVerenigingWerdGeregistreerdWithoutVertegenwoordigers();

        var verenigingRepositoryMock = new VerenigingRepositoryMock(_scenario.GetVerenigingState());

        _fixture = new Fixture().CustomizeAll();
        _commandHandler = new VerwijderVertegenwoordigerCommandHandler(verenigingRepositoryMock);
    }

    [Fact]
    public async Task Then_A_UnknownVertegenoordigerException_Is_Thrown()
    {
        var command = new VerwijderVertegenwoordigerCommand(_scenario.VCode, _fixture.Create<int>());
        var commandMetadata = _fixture.Create<CommandMetadata>();

        var handle = () => _commandHandler.Handle(new CommandEnvelope<VerwijderVertegenwoordigerCommand>(command, commandMetadata));

        await handle.Should().ThrowAsync<UnknownVertegenwoordiger>();
    }
}
