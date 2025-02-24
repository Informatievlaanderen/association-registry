﻿namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.WijzigBasisgegevens.FeitelijkeVereniging.CommandHandling.Werkingsgebieden;

using AssociationRegistry.Test.Common.Framework;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling;
using AssociationRegistry.Vereniging;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_NietBepaald_To_NietBepaald
{
    private readonly VerenigingRepositoryMock _verenigingRepositoryMock;
    private readonly WerkingsgebiedenWerdenNietBepaaldScenario _scenario;

    public With_NietBepaald_To_NietBepaald()
    {
        _scenario = new WerkingsgebiedenWerdenNietBepaaldScenario();

        _verenigingRepositoryMock =
            WerkingsgebiedenScenarioRunner.Run(_scenario, werkingsgebieden: fixture => Werkingsgebieden.NietBepaald);
    }

    [Fact]
    public void Then_The_Correct_Vereniging_Is_Loaded_Once()
    {
        _verenigingRepositoryMock.ShouldHaveLoaded<Vereniging>(_scenario.VCode);
    }

    [Fact]
    public void Then_No_Event_Is_Saved()
    {
        _verenigingRepositoryMock.ShouldNotHaveAnySaves();
    }
}
