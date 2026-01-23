namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.WijzigBasisgegevens.FeitelijkeVereniging.When_WijzigBasisGegevens.CommandHandling.Werkingsgebieden;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Test.Common.Framework;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling;
using AssociationRegistry.Vereniging;
using Common.StubsMocksFakes.VerenigingsRepositories;
using Xunit;

public class With_NietVanToepassing_To_NietVanToepassing
{
    private readonly AggregateSessionMock _aggregateSessionMock;
    private readonly WerkingsgebiedenWerdenNietVanToepassingScenario _scenario;

    public With_NietVanToepassing_To_NietVanToepassing()
    {
        _scenario = new WerkingsgebiedenWerdenNietVanToepassingScenario();

        (_aggregateSessionMock, _) = WerkingsgebiedenScenarioRunner.Run(
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
    public void Then_No_Event_Is_Saved()
    {
        _aggregateSessionMock.ShouldNotHaveAnySaves();
    }
}
