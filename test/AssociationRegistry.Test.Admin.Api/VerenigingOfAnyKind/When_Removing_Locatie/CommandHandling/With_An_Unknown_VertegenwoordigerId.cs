﻿namespace AssociationRegistry.Test.Admin.Api.VerenigingOfAnyKind.When_Removing_Locatie.CommandHandling;

using Acties.VerwijderLocatie;
using AssociationRegistry.Framework;
using AutoFixture;
using Fakes;
using Fixtures.Scenarios.CommandHandling;
using FluentAssertions;
using Framework;
using Vereniging.Exceptions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_An_Unknown_LocatieId
{
    private readonly FeitelijkeVerenigingWerdGeregistreerdWithMinimalFields _scenario;
    private readonly VerwijderLocatieCommandHandler _commandHandler;
    private readonly Fixture _fixture;

    public With_An_Unknown_LocatieId()
    {
        _scenario = new FeitelijkeVerenigingWerdGeregistreerdWithMinimalFields();

        var verenigingRepositoryMock = new VerenigingRepositoryMock(_scenario.GetVerenigingState());

        _fixture = new Fixture().CustomizeAdminApi();
        _commandHandler = new VerwijderLocatieCommandHandler(verenigingRepositoryMock);
    }

    [Fact]
    public async Task Then_A_UnknownVertegenoordigerException_Is_Thrown()
    {
        var command = new VerwijderLocatieCommand(_scenario.VCode, _fixture.Create<int>());
        var commandMetadata = _fixture.Create<CommandMetadata>();

        var handle = () => _commandHandler.Handle(new CommandEnvelope<VerwijderLocatieCommand>(command, commandMetadata));

        await handle.Should().ThrowAsync<LocatieIsNietGekend>();
    }
}
