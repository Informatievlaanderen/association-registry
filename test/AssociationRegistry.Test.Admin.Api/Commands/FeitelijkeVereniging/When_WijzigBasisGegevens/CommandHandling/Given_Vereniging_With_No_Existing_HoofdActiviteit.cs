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
    private FeitelijkeVerenigingWerdGeregistreerdWithNoHoofdActiviteitScenario _scenario;
    private VerenigingRepositoryMock _verenigingRepositoryMock;
    private Fixture _fixture;

    public Given_Vereniging_With_No_Existing_HoofdActiviteit()
    {
        _scenario = new FeitelijkeVerenigingWerdGeregistreerdWithNoHoofdActiviteitScenario();
        _verenigingRepositoryMock = new VerenigingRepositoryMock(_scenario.GetVerenigingState());

        _fixture = new Fixture().CustomizeAdminApi();
    }


    [Fact]
    public async Task WithEmptyHoofdActiviteitenRequest_ThenNothing()
    {


        var command = new WijzigBasisgegevensCommand(_scenario.VCode,
                                                     HoofdactiviteitenVerenigingsloket: Array.Empty<HoofdactiviteitVerenigingsloket>());

        var commandHandler = new WijzigBasisgegevensCommandHandler();
        var commandMetadata = _fixture.Create<CommandMetadata>();

        await commandHandler.Handle(
            new CommandEnvelope<WijzigBasisgegevensCommand>(command, commandMetadata),
            _verenigingRepositoryMock,
            new ClockStub(_fixture.Create<DateOnly>()));

        _verenigingRepositoryMock.ShouldNotHaveSaved<HoofdactiviteitenVerenigingsloketWerdenGewijzigd>();
    }
}
