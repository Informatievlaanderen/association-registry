namespace AssociationRegistry.Test.Admin.Api.VerenigingMetRechtspersoonlijkheid.When_WijzigBasisGegevens.CommandHandling;

using Acties.VerenigingMetRechtspersoonlijkheid.WijzigBasisgegevens;
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
    private VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithOneHoofdActiviteitScenario _scenario;
    private VerenigingRepositoryMock _verenigingRepositoryMock;
    private Fixture _fixture;

    public Given_Vereniging_With_One_Existing_HoofdActiviteit()
    {
        _scenario = new VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithOneHoofdActiviteitScenario();
        _verenigingRepositoryMock = new VerenigingRepositoryMock(_scenario.GetVerenigingState());

        _fixture = new Fixture().CustomizeAdminApi();
    }

    [Fact]
    public async Task WithEmptyHoofdActiviteitenRequest_ThenThrowsNonSuccessStatusCode()
    {
        var command = new WijzigBasisgegevensCommand(_scenario.VCode,
                                                     HoofdactiviteitenVerenigingsloket: Array.Empty<HoofdactiviteitVerenigingsloket>());

        var commandHandler = new WijzigBasisgegevensCommandHandler();
        var commandMetadata = _fixture.Create<CommandMetadata>();

        await Assert.ThrowsAsync<LaatsteHoofdActiviteitKanNietVerwijderdWorden>(async () => await commandHandler.Handle(
                                                                                   new CommandEnvelope<WijzigBasisgegevensCommand>(
                                                                                       command, commandMetadata),
                                                                                   _verenigingRepositoryMock,
                                                                                   CancellationToken.None));
    }
}
