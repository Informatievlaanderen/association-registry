﻿namespace AssociationRegistry.Test.Admin.Api.Commands.FeitelijkeVereniging.When_StopVereniging.CommandHandling;

using AssociationRegistry.Acties.StopVereniging;
using AssociationRegistry.Events;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Admin.Api.Framework;
using AssociationRegistry.Test.Common.Framework;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling;
using AssociationRegistry.Vereniging;
using AutoFixture;
using Framework.Fakes;
using Xunit;

public class With_A_Einddatum
{
    private readonly FeitelijkeVerenigingWerdGeregistreerdScenario _scenario;
    private readonly VerenigingRepositoryMock _verenigingRepositoryMock;
    private readonly StopVerenigingCommand _command;

    public With_A_Einddatum()
    {
        _scenario = new FeitelijkeVerenigingWerdGeregistreerdScenario();
        _verenigingRepositoryMock = new VerenigingRepositoryMock(_scenario.GetVerenigingState());

        var fixture = new Fixture().CustomizeAdminApi();

        _command = new StopVerenigingCommand(_scenario.VCode,
                                             Datum.Hydrate(_scenario.FeitelijkeVerenigingWerdGeregistreerd.Startdatum!.Value.AddDays(1)));

        var commandMetadata = fixture.Create<CommandMetadata>();

        var commandHandler = new StopVerenigingCommandHandler(_verenigingRepositoryMock,
                                                              new ClockStub(_command.Einddatum.Value.AddDays(1)));

        commandHandler.Handle(
            new CommandEnvelope<StopVerenigingCommand>(_command, commandMetadata),
            CancellationToken.None).GetAwaiter().GetResult();
    }

    [Fact]
    public void Then_The_Correct_Vereniging_Is_Loaded_Once()
    {
        _verenigingRepositoryMock.ShouldHaveLoaded<Vereniging>(_scenario.VCode);
    }

    [Fact]
    public void Then_A_VerenigingWerdGestopt_Event_Is_Saved()
    {
        _verenigingRepositoryMock.ShouldHaveSaved(
            new VerenigingWerdGestopt(_command.Einddatum.Value)
        );
    }
}
