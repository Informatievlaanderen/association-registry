namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.WijzigBasisgegevens.FeitelijkeVereniging.When_WijzigBasisGegevens.CommandHandling;

using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Basisgegevens.VerenigingZonderEigenRechtspersoonlijkheid;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Events;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Framework;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling.FeitelijkeVereniging;
using AssociationRegistry.Test.Framework;
using AssociationRegistry.Vereniging;
using AutoFixture;
using Common.StubsMocksFakes.Faktories;
using Common.StubsMocksFakes.VerenigingsRepositories;
using Moq;
using Xunit;

public class Given_Vereniging_With_No_Existing_HoofdActiviteit
{
    private FeitelijkeVerenigingWerdGeregistreerdWithNoHoofdActiviteitScenario _scenario;
    private AggregateSessionMock _aggregateSessionMock;
    private Fixture _fixture;

    public Given_Vereniging_With_No_Existing_HoofdActiviteit()
    {
        _scenario = new FeitelijkeVerenigingWerdGeregistreerdWithNoHoofdActiviteitScenario();
        _aggregateSessionMock = new AggregateSessionMock(_scenario.GetVerenigingState());

        _fixture = new Fixture().CustomizeAdminApi();
    }

    [Fact]
    public async ValueTask WithEmptyHoofdActiviteitenRequest_ThenNothing()
    {
        var command = new WijzigBasisgegevensCommand(
            _scenario.VCode,
            HoofdactiviteitenVerenigingsloket: Array.Empty<HoofdactiviteitVerenigingsloket>()
        );

        var commandHandler = new WijzigBasisgegevensCommandHandler(
            Faktory.New().GeotagsService.ReturnsEmptyGeotags().Object
        );
        var commandMetadata = _fixture.Create<CommandMetadata>();

        await commandHandler.Handle(
            new CommandEnvelope<WijzigBasisgegevensCommand>(command, commandMetadata),
            _aggregateSessionMock,
            new ClockStub(_fixture.Create<DateOnly>())
        );

        _aggregateSessionMock.ShouldNotHaveSaved<HoofdactiviteitenVerenigingsloketWerdenGewijzigd>();
    }
}
