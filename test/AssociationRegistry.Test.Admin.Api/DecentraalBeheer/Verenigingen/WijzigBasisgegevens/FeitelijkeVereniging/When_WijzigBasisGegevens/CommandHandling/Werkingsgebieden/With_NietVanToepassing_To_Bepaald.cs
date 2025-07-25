﻿namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.WijzigBasisgegevens.FeitelijkeVereniging.When_WijzigBasisGegevens.CommandHandling.Werkingsgebieden;

using AssociationRegistry.EventFactories;
using AssociationRegistry.Test.Common.Framework;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling;
using AssociationRegistry.Vereniging;
using AutoFixture;
using Common.StubsMocksFakes.VerenigingsRepositories;
using Vereniging.Geotags;
using Xunit;

public class With_NietVanToepassing_To_Bepaald
{
    private readonly VerenigingRepositoryMock _verenigingRepositoryMock;
    private readonly WerkingsgebiedenWerdenNietVanToepassingScenario _scenario;
    private Werkingsgebied[] _werkingsgebieden;
    private GeotagsCollection _geotags;

    public With_NietVanToepassing_To_Bepaald()
    {
        _scenario = new WerkingsgebiedenWerdenNietVanToepassingScenario();

        (_verenigingRepositoryMock, _geotags) =
            WerkingsgebiedenScenarioRunner.Run(_scenario, werkingsgebieden: fixture =>
            {
                _werkingsgebieden = fixture.CreateMany<Werkingsgebied>().Distinct().ToArray();

                return _werkingsgebieden;
            });
    }

    [Fact]
    public void Then_The_Correct_Vereniging_Is_Loaded_Once()
    {
        _verenigingRepositoryMock.ShouldHaveLoaded<Vereniging>(_scenario.VCode);
    }

    [Fact]
    public void Then_A_WerkingsgebiedenWerdenBepaald_Event_Is_Saved()
    {
        _verenigingRepositoryMock.ShouldHaveSavedExact(
            EventFactory.WerkingsgebiedenWerdenBepaald(_scenario.VCode, _werkingsgebieden),
            EventFactory.GeotagsWerdenBepaald(VCode.Create(_scenario.VCode), _geotags));


    }
}
