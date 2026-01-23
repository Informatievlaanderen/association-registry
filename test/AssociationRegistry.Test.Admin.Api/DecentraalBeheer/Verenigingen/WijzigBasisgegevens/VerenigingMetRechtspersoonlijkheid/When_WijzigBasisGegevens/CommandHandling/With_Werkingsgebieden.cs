namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.WijzigBasisgegevens.VerenigingMetRechtspersoonlijkheid.When_WijzigBasisGegevens.CommandHandling;

using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Basisgegevens.VerenigingMetRechtspersoonlijkheid;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.DecentraalBeheer.Vereniging.Geotags;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Framework;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;
using AssociationRegistry.Vereniging;
using AutoFixture;
using Common.StubsMocksFakes.Faktories;
using Common.StubsMocksFakes.VerenigingsRepositories;
using Events.Factories;
using Moq;
using Xunit;

public class With_Werkingsgebieden
{
    private readonly AggregateSessionMock _aggregateSessionMock;
    private readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario _scenario;
    private readonly Werkingsgebied[] _werkingsgebieden;

    public With_Werkingsgebieden()
    {
        _scenario = new VerenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario();

        _aggregateSessionMock = new AggregateSessionMock(_scenario.GetVerenigingState());

        var fixture = new Fixture().CustomizeAdminApi();
        _werkingsgebieden = fixture.CreateMany<Werkingsgebied>().Distinct().ToArray();

        var command = new WijzigBasisgegevensCommand(_scenario.VCode, Werkingsgebieden: _werkingsgebieden);

        var commandMetadata = fixture.Create<CommandMetadata>();
        var commandHandler = new WijzigBasisgegevensCommandHandler(
            Faktory.New().GeotagsService.ReturnsEmptyGeotags().Object
        );

        commandHandler
            .Handle(new CommandEnvelope<WijzigBasisgegevensCommand>(command, commandMetadata), _aggregateSessionMock)
            .GetAwaiter()
            .GetResult();
    }

    [Fact]
    public void Then_The_Correct_Vereniging_Is_Loaded_Once()
    {
        _aggregateSessionMock.ShouldHaveLoaded<VerenigingMetRechtspersoonlijkheid>(_scenario.VCode);
    }

    [Fact]
    public void Then_A_WerkingsgebiedenWerdenGewijzigd_Event_Is_Saved()
    {
        _aggregateSessionMock.ShouldHaveSavedExact(
            EventFactory.WerkingsgebiedenWerdenGewijzigd(_scenario.VCode, _werkingsgebieden),
            EventFactory.GeotagsWerdenBepaald(VCode.Create(_scenario.VCode), GeotagsCollection.Empty)
        );
    }
}
