namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.WijzigBasisgegevens.FeitelijkeVereniging.When_WijzigBasisGegevens.CommandHandling;

using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Basisgegevens.VerenigingZonderEigenRechtspersoonlijkheid;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Events;
using AssociationRegistry.Framework;
using AssociationRegistry.Primitives;
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

public class With_A_Startdatum
{
    private readonly AggregateSessionMock _aggregateSessionMock;
    private readonly FeitelijkeVerenigingWerdGeregistreerdScenario _scenario;
    private readonly DateOnly _nieuweStartdatum;

    public With_A_Startdatum()
    {
        _scenario = new FeitelijkeVerenigingWerdGeregistreerdScenario();

        _aggregateSessionMock = new AggregateSessionMock(_scenario.GetVerenigingState());

        var fixture = new Fixture().CustomizeAdminApi();
        _nieuweStartdatum = new DateOnly(year: 2023, month: 3, day: 6).AddDays(-1);

        var command = new WijzigBasisgegevensCommand(
            _scenario.VCode,
            Startdatum: NullOrEmpty<Datum>.Create(Datum.Create(_nieuweStartdatum))
        );

        var commandMetadata = fixture.Create<CommandMetadata>();
        var commandHandler = new WijzigBasisgegevensCommandHandler(
            Faktory.New().GeotagsService.ReturnsEmptyGeotags().Object
        );

        commandHandler
            .Handle(
                new CommandEnvelope<WijzigBasisgegevensCommand>(command, commandMetadata),
                _aggregateSessionMock,
                new ClockStub(_nieuweStartdatum)
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
    public void Then_A_StartdatumWerdGewijzigd_Event_Is_Saved()
    {
        _aggregateSessionMock.ShouldHaveSavedExact(new StartdatumWerdGewijzigd(_scenario.VCode, _nieuweStartdatum));
    }
}
