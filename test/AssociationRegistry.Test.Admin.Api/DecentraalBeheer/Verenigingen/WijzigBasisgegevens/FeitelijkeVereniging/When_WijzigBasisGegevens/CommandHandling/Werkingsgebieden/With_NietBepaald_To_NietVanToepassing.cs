﻿namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.WijzigBasisgegevens.FeitelijkeVereniging.When_WijzigBasisGegevens.CommandHandling.Werkingsgebieden;

using AssociationRegistry.EventFactories;
using AssociationRegistry.Test.Common.Framework;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling;
using AssociationRegistry.Vereniging;
using Common.StubsMocksFakes.VerenigingsRepositories;
using Vereniging.Geotags;
using Xunit;

public class With_NietBepaald_To_NietVanToepassing
{
    private readonly VerenigingRepositoryMock _verenigingRepositoryMock;
    private readonly WerkingsgebiedenWerdenNietBepaaldScenario _scenario;
    private GeotagsCollection _geotags;

    public With_NietBepaald_To_NietVanToepassing()
    {
        _scenario = new WerkingsgebiedenWerdenNietBepaaldScenario();

        (_verenigingRepositoryMock, _geotags) =
            WerkingsgebiedenScenarioRunner.Run(_scenario, werkingsgebieden: fixture => Werkingsgebieden.NietVanToepassing);
    }

    [Fact]
    public void Then_The_Correct_Vereniging_Is_Loaded_Once()
    {
        _verenigingRepositoryMock.ShouldHaveLoaded<Vereniging>(_scenario.VCode);
    }

    [Fact]
    public void Then_A_WerkingsgebiedenWerdenNietVanToepassing_Event_Is_Saved()
    {
        _verenigingRepositoryMock.ShouldHaveSavedExact(
            EventFactory.WerkingsgebiedenWerdenNietVanToepassing(_scenario.VCode),
            EventFactory.GeotagsWerdenBepaald(_scenario.VCode, _geotags));    }
}
