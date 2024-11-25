namespace AssociationRegistry.Test.Admin.Api.Commands.FeitelijkeVereniging.When_WijzigBasisGegevens.CommandHandling.Werkingsgebieden;

using Common.Framework;
using Common.Scenarios.CommandHandling;
using Events;
using Vereniging;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_NietBepaald_To_NietVanToepassing
{
    private readonly VerenigingRepositoryMock _verenigingRepositoryMock;
    private readonly WerkingsgebiedenWerdenNietBepaaldScenario _scenario;

    public With_NietBepaald_To_NietVanToepassing()
    {
        _scenario = new WerkingsgebiedenWerdenNietBepaaldScenario();

        _verenigingRepositoryMock =
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
        _verenigingRepositoryMock.ShouldHaveSaved(WerkingsgebiedenWerdenNietVanToepassing.With(_scenario.VCode));
    }
}
