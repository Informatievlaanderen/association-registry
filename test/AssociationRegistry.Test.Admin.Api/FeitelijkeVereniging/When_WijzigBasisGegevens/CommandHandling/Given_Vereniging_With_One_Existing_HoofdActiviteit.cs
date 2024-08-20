namespace AssociationRegistry.Test.Admin.Api.FeitelijkeVereniging.When_WijzigBasisGegevens.CommandHandling;

using Acties.WijzigBasisgegevens;
using AssociationRegistry.Framework;
using AutoFixture;
using Fakes;
using Fixtures.Scenarios.CommandHandling;
using Framework;
using Vereniging;
using Vereniging.Exceptions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_Vereniging_With_One_Existing_HoofdActiviteit
{
    [Fact]
    public async Task With_Empty_HoofdActiviteiten_Request_Then_Throws_NonSuccessStatusCode()
    {
        var scenario = new FeitelijkeVerenigingWerdGeregistreerdWithOneHoofdActiviteitScenario();
        var verenigingRepositoryMock = new VerenigingRepositoryMock(scenario.GetVerenigingState());

        var fixture = new Fixture().CustomizeAdminApi();

        var command = new WijzigBasisgegevensCommand(scenario.VCode,
                                                     HoofdactiviteitenVerenigingsloket: Array.Empty<HoofdactiviteitVerenigingsloket>());

        var commandHandler = new WijzigBasisgegevensCommandHandler();
        var commandMetadata = fixture.Create<CommandMetadata>();

        await Assert.ThrowsAsync<LaatsteHoofdActiviteitKanNietVerwijderdWorden>(async () => await commandHandler.Handle(
                                                                                   new CommandEnvelope<WijzigBasisgegevensCommand>(
                                                                                       command, commandMetadata),
                                                                                   verenigingRepositoryMock,
                                                                                   new ClockStub(fixture.Create<DateOnly>())));
    }
}
