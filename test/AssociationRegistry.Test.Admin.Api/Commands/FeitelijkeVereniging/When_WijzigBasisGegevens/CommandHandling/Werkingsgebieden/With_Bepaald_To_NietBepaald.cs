﻿namespace AssociationRegistry.Test.Admin.Api.Commands.FeitelijkeVereniging.When_WijzigBasisGegevens.CommandHandling.Werkingsgebieden;

using Common.Framework;
using Common.Scenarios.CommandHandling;
using EventFactories;
using Events;
using Vereniging;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_Bepaald_To_NietBepaald
{
    private readonly VerenigingRepositoryMock _verenigingRepositoryMock;
    private readonly WerkingsgebiedenWerdenBepaaldScenario _scenario;

    public With_Bepaald_To_NietBepaald()
    {
        _scenario = new WerkingsgebiedenWerdenBepaaldScenario();

        _verenigingRepositoryMock =
            WerkingsgebiedenScenarioRunner.Run(_scenario, werkingsgebieden: fixture => Werkingsgebieden.NietBepaald);
    }

    [Fact]
    public void Then_The_Correct_Vereniging_Is_Loaded_Once()
    {
        _verenigingRepositoryMock.ShouldHaveLoaded<Vereniging>(_scenario.VCode);
    }

    [Fact]
    public void Then_A_WerkingsgebiedenWerdenBepaald_Event_Is_Saved()
    {
        _verenigingRepositoryMock.ShouldHaveSaved(
            EventFactory.WerkingsgebiedenWerdenNietBepaald(_scenario.VCode)
        );
    }
}