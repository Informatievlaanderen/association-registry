namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.WijzigBasisgegevens.FeitelijkeVereniging.When_WijzigBasisGegevens.CommandHandling.Werkingsgebieden;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.DecentraalBeheer.Vereniging.Geotags;
using AssociationRegistry.Test.Common.Framework;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling;
using AssociationRegistry.Vereniging;
using Common.StubsMocksFakes.VerenigingsRepositories;
using Events.Factories;
using Xunit;

public class With_NietBepaald_To_NietVanToepassing
{
    private readonly AggregateSessionMock _aggregateSessionMock;
    private readonly WerkingsgebiedenWerdenNietBepaaldScenario _scenario;
    private GeotagsCollection _geotags;

    public With_NietBepaald_To_NietVanToepassing()
    {
        _scenario = new WerkingsgebiedenWerdenNietBepaaldScenario();

        (_aggregateSessionMock, _geotags) = WerkingsgebiedenScenarioRunner.Run(
            _scenario,
            werkingsgebieden: fixture => Werkingsgebieden.NietVanToepassing
        );
    }

    [Fact]
    public void Then_The_Correct_Vereniging_Is_Loaded_Once()
    {
        _aggregateSessionMock.ShouldHaveLoaded<Vereniging>(_scenario.VCode);
    }

    [Fact]
    public void Then_A_WerkingsgebiedenWerdenNietVanToepassing_Event_Is_Saved()
    {
        _aggregateSessionMock.ShouldHaveSavedExact(
            EventFactory.WerkingsgebiedenWerdenNietVanToepassing(_scenario.VCode),
            EventFactory.GeotagsWerdenBepaald(_scenario.VCode, _geotags)
        );
    }
}
