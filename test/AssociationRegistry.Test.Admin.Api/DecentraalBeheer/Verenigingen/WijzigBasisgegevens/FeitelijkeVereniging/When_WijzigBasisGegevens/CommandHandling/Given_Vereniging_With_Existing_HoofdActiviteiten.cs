namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.WijzigBasisgegevens.FeitelijkeVereniging.When_WijzigBasisGegevens.CommandHandling;

using AssociationRegistry.DecentraalBeheer.Basisgegevens.FeitelijkeVereniging;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Framework;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling.FeitelijkeVereniging;
using AssociationRegistry.Test.Framework;
using AssociationRegistry.Vereniging;
using AssociationRegistry.Vereniging.Exceptions;
using AutoFixture;
using Xunit;

public class Given_Vereniging_With_Existing_HoofdActiviteiten
{
    private FeitelijkeVerenigingWerdGeregistreerdWithHoofdActiviteitenScenario _scenario;
    private VerenigingRepositoryMock _verenigingRepositoryMock;
    private Fixture _fixture;

    public Given_Vereniging_With_Existing_HoofdActiviteiten()
    {
        _scenario = new FeitelijkeVerenigingWerdGeregistreerdWithHoofdActiviteitenScenario();
        _verenigingRepositoryMock = new VerenigingRepositoryMock(_scenario.GetVerenigingState());

        _fixture = new Fixture().CustomizeAdminApi();
    }

    [Fact]
    public async ValueTask WithEmptyHoofdActiviteitenRequest_ThenThrowsNonSuccessStatusCode()
    {
        var command = new WijzigBasisgegevensCommand(_scenario.VCode,
                                                     HoofdactiviteitenVerenigingsloket: Array.Empty<HoofdactiviteitVerenigingsloket>());

        var commandHandler = new WijzigBasisgegevensCommandHandler();
        var commandMetadata = _fixture.Create<CommandMetadata>();

        await Assert.ThrowsAsync<LaatsteHoofdActiviteitKanNietVerwijderdWorden>(async () => await commandHandler.Handle(
                                                                                   new CommandEnvelope<WijzigBasisgegevensCommand>(
                                                                                       command, commandMetadata),
                                                                                   _verenigingRepositoryMock,
                                                                                   new ClockStub(_fixture.Create<DateOnly>())));
    }
}
