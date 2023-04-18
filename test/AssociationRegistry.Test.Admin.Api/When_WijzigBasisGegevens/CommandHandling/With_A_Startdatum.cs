﻿namespace AssociationRegistry.Test.Admin.Api.When_WijzigBasisGegevens.CommandHandling;

using Acties.WijzigBasisgegevens;
using Events;
using AssociationRegistry.Framework;
using Fakes;
using Fixtures.Scenarios;
using AutoFixture;
using Vereniging;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_A_Startdatum
{
    private readonly VerenigingRepositoryMock _verenigingRepositoryMock;
    private readonly VerenigingWerdGeregistreerdScenario _scenario;
    private readonly DateOnly _nieuweStartdatum;

    public With_A_Startdatum()
    {
        _scenario = new VerenigingWerdGeregistreerdScenario();

        _verenigingRepositoryMock = new VerenigingRepositoryMock(_scenario.GetVereniging());

        var fixture = new Fixture();
        _nieuweStartdatum = _scenario.Startdatum.AddDays(-1);
        var command = new WijzigBasisgegevensCommand(_scenario.VCode, Startdatum: Startdatum.Create(_nieuweStartdatum));
        var commandMetadata = fixture.Create<CommandMetadata>();
        var commandHandler = new WijzigBasisgegevensCommandHandler();

        commandHandler.Handle(
            new CommandEnvelope<WijzigBasisgegevensCommand>(command, commandMetadata),
            _verenigingRepositoryMock,
            new ClockStub(_nieuweStartdatum)).GetAwaiter().GetResult();
    }

    [Fact]
    public void Then_The_Correct_Vereniging_Is_Loaded_Once()
    {
        _verenigingRepositoryMock.ShouldHaveLoaded(_scenario.VCode);
    }

    [Fact]
    public void Then_A_StartdatumWerdGewijzigd_Event_Is_Saved()
    {
        _verenigingRepositoryMock.ShouldHaveSaved(
            new StartdatumWerdGewijzigd(_scenario.VCode, _nieuweStartdatum)
        );
    }
}
