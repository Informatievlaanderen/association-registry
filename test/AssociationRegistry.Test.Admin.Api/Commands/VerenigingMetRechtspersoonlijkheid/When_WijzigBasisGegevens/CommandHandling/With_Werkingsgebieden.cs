﻿namespace AssociationRegistry.Test.Admin.Api.Commands.VerenigingMetRechtspersoonlijkheid.When_WijzigBasisGegevens.CommandHandling;

using Acties.VerenigingMetRechtspersoonlijkheid.WijzigBasisgegevens;
using AssociationRegistry.Framework;
using AutoFixture;
using Common.Framework;
using Common.Scenarios.CommandHandling;
using Events;
using Framework;
using Vereniging;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_Werkingsgebieden
{
    private readonly VerenigingRepositoryMock _verenigingRepositoryMock;
    private readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario _scenario;
    private readonly Werkingsgebied[] _werkingsgebieden;

    public With_Werkingsgebieden()
    {
        _scenario = new VerenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario();

        _verenigingRepositoryMock = new VerenigingRepositoryMock(_scenario.GetVerenigingState());

        var fixture = new Fixture().CustomizeAdminApi();
        _werkingsgebieden = fixture.CreateMany<Werkingsgebied>().Distinct().ToArray();

        var command = new WijzigBasisgegevensCommand(_scenario.VCode,
                                                     Werkingsgebieden: _werkingsgebieden);

        var commandMetadata = fixture.Create<CommandMetadata>();
        var commandHandler = new WijzigBasisgegevensCommandHandler();

        commandHandler.Handle(
            new CommandEnvelope<WijzigBasisgegevensCommand>(command, commandMetadata),
            _verenigingRepositoryMock).GetAwaiter().GetResult();
    }

    [Fact]
    public void Then_The_Correct_Vereniging_Is_Loaded_Once()
    {
        _verenigingRepositoryMock.ShouldHaveLoaded<VerenigingMetRechtspersoonlijkheid>(_scenario.VCode);
    }

    [Fact]
    public void Then_A_WerkingsgebiedenWerdenGewijzigd_Event_Is_Saved()
    {
        _verenigingRepositoryMock.ShouldHaveSaved(
            WerkingsgebiedenWerdenGewijzigd.With(_werkingsgebieden)
        );
    }
}
