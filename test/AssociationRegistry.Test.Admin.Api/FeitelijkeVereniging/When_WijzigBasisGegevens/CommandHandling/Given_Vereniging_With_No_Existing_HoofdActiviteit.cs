namespace AssociationRegistry.Test.Admin.Api.FeitelijkeVereniging.When_WijzigBasisGegevens.CommandHandling;

using Acties.WijzigBasisgegevens;
using AssociationRegistry.Framework;
using AutoFixture;
using Events;
using Fakes;
using Fixtures.Scenarios.CommandHandling;
using Framework;
using Vereniging;
using Vereniging.Exceptions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_Vereniging_With_No_Existing_HoofdActiviteit
{
    [Fact]
    public async Task With_Empty_HoofdActiviteiten_Request_Then_Nothing()
    {
        var scenario = new FeitelijkeVerenigingWerdGeregistreerdWithNoHoofdActiviteitScenario();
        var verenigingRepositoryMock = new VerenigingRepositoryMock(scenario.GetVerenigingState());

        var fixture = new Fixture().CustomizeAdminApi();

        var command = new WijzigBasisgegevensCommand(scenario.VCode,
                                                     HoofdactiviteitenVerenigingsloket: Array.Empty<HoofdactiviteitVerenigingsloket>());

        var commandHandler = new WijzigBasisgegevensCommandHandler();
        var commandMetadata = fixture.Create<CommandMetadata>();

        await commandHandler.Handle(
            new CommandEnvelope<WijzigBasisgegevensCommand>(command, commandMetadata),
            verenigingRepositoryMock,
            new ClockStub(fixture.Create<DateOnly>()));

        verenigingRepositoryMock.ShouldNotHaveSaved<HoofdactiviteitenVerenigingsloketWerdenGewijzigd>();
    }
}
