namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.WijzigBasisgegevens.FeitelijkeVereniging.When_WijzigBasisGegevens.CommandHandling;

using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Basisgegevens.VerenigingZonderEigenRechtspersoonlijkheid;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Events;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Admin.Api.Framework.Fakes;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Framework;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling.FeitelijkeVereniging;
using AssociationRegistry.Vereniging;
using AutoFixture;
using Common.StubsMocksFakes.Clocks;
using Common.StubsMocksFakes.Faktories;
using Common.StubsMocksFakes.VerenigingsRepositories;
using Moq;
using Xunit;

public class With_The_Same_HoofdactiviteitenVerenigingsloket
{
    private readonly AggregateSessionMock _aggregateSessionMock;
    private readonly FeitelijkeVerenigingWerdGeregistreerdScenario _scenario;

    public With_The_Same_HoofdactiviteitenVerenigingsloket()
    {
        _scenario = new FeitelijkeVerenigingWerdGeregistreerdScenario();

        _aggregateSessionMock = new AggregateSessionMock(_scenario.GetVerenigingState());

        var fixture = new Fixture().CustomizeAdminApi();

        var command = new WijzigBasisgegevensCommand(
            _scenario.VCode,
            HoofdactiviteitenVerenigingsloket: _scenario
                .FeitelijkeVerenigingWerdGeregistreerd.HoofdactiviteitenVerenigingsloket.Select(h =>
                    HoofdactiviteitVerenigingsloket.Create(h.Code)
                )
                .ToArray()
        );

        var commandMetadata = fixture.Create<CommandMetadata>();
        var commandHandler = new WijzigBasisgegevensCommandHandler(
            Faktory.New().GeotagsService.ReturnsEmptyGeotags().Object
        );

        commandHandler
            .Handle(
                new CommandEnvelope<WijzigBasisgegevensCommand>(command, commandMetadata),
                _aggregateSessionMock,
                new ClockStub(fixture.Create<DateOnly>())
            )
            .GetAwaiter()
            .GetResult();
    }

    [Fact]
    public void Then_The_Correct_Vereniging_Is_Loaded_Once()
    {
        _aggregateSessionMock.ShouldHaveLoaded<Vereniging>(_scenario.VCode);
    }

    [Fact]
    public void Then_No_HoofactiviteitenVerenigingloketWerdenGewijzigd_Event_Is_Saved()
    {
        _aggregateSessionMock.ShouldNotHaveSaved<HoofdactiviteitenVerenigingsloketWerdenGewijzigd>();
    }
}
