﻿namespace AssociationRegistry.Test.Admin.Api.VerenigingMetRechtspersoonlijkheid.When_WijzigBasisGegevens.CommandHandling;

using Acties.VerenigingMetRechtspersoonlijkheid.WijzigBasisgegevens;
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
public class With_A_FeitelijkeVereniging
{
    private readonly VerenigingRepositoryMock _verenigingRepositoryMock;
    private readonly WijzigBasisgegevensCommandHandler _commandHandler;
    private readonly CommandEnvelope<WijzigBasisgegevensCommand> _envelope;
    private const string NieuweKorteBeschrijving = "Een nieuwe beschrijving van de vereniging";

    public With_A_FeitelijkeVereniging()
    {
        var scenario = new FeitelijkeVerenigingWerdGeregistreerdScenario();
        _verenigingRepositoryMock = new VerenigingRepositoryMock(scenario.GetVerenigingState());

        var fixture = new Fixture().CustomizeAdminApi();
        var command = new WijzigBasisgegevensCommand(scenario.VCode, KorteBeschrijving: NieuweKorteBeschrijving);
        var commandMetadata = fixture.Create<CommandMetadata>();
        _commandHandler = new WijzigBasisgegevensCommandHandler();
        _envelope = new CommandEnvelope<WijzigBasisgegevensCommand>(command, commandMetadata);

    }

    [Fact]
    public async Task Then_A_UnsupportedOperationException_Is_Thrown()
    {
        var method = ()=>  _commandHandler.Handle(_envelope, _verenigingRepositoryMock);
        await method.Should().ThrowAsync<UnsupportedOperationForVerenigingstype>();
    }
}
