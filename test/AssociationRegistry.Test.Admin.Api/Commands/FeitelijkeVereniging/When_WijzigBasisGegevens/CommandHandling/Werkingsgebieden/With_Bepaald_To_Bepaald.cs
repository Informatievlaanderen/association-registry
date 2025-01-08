namespace AssociationRegistry.Test.Admin.Api.Commands.FeitelijkeVereniging.When_WijzigBasisGegevens.CommandHandling.Werkingsgebieden;

using AutoFixture;
using Common.Framework;
using Common.Scenarios.CommandHandling;
using EventFactories;
using Events;
using Vereniging;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_Bepaald_To_Bepaald
{
    private readonly VerenigingRepositoryMock _verenigingRepositoryMock;
    private readonly WerkingsgebiedenWerdenBepaaldScenario _scenario;
    private Werkingsgebied[] _werkingsgebieden;

    public With_Bepaald_To_Bepaald()
    {
        _scenario = new WerkingsgebiedenWerdenBepaaldScenario();

        _verenigingRepositoryMock =
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
    public void Then_A_WerkingsgebiedenWerdenGewijzigd_Event_Is_Saved()
    {
        _verenigingRepositoryMock.ShouldHaveSaved(
            EventFactory.WerkingsgebiedenWerdenGewijzigd(_scenario.VCode, _werkingsgebieden)
        );
    }
}
