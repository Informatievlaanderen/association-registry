namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.WijzigBasisgegevens.VerenigingMetRechtspersoonlijkheid.When_WijzigBasisGegevens.CommandHandling;

using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Basisgegevens.VerenigingMetRechtspersoonlijkheid;
using AssociationRegistry.DecentraalBeheer.Vereniging;
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

public class With_HoofdactiviteitenVerenigingsloket
{
    private readonly AggregateSessionMock _aggregateSessionMock;
    private readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario _scenario;
    private readonly HoofdactiviteitVerenigingsloket[] _hoofdactiviteitenVerenigingsloket;

    public With_HoofdactiviteitenVerenigingsloket()
    {
        _scenario = new VerenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario();

        _aggregateSessionMock = new AggregateSessionMock(_scenario.GetVerenigingState());

        var fixture = new Fixture().CustomizeAdminApi();
        _hoofdactiviteitenVerenigingsloket = fixture.CreateMany<HoofdactiviteitVerenigingsloket>().Distinct().ToArray();

        var command = new WijzigBasisgegevensCommand(
            _scenario.VCode,
            HoofdactiviteitenVerenigingsloket: _hoofdactiviteitenVerenigingsloket
        );

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
    public void Then_A_HoofactiviteitenVerenigingloketWerdenGewijzigd_Event_Is_Saved()
    {
        _aggregateSessionMock.ShouldHaveSavedExact(
            EventFactory.HoofdactiviteitenVerenigingsloketWerdenGewijzigd(_hoofdactiviteitenVerenigingsloket)
        );
    }
}
