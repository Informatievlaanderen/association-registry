namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Locaties.VerenigingOfAnyKind.When_Removing_Locatie.CommandHandling;

using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Locaties.VerwijderLocatie;
using AssociationRegistry.DecentraalBeheer.Vereniging.Exceptions;
using AssociationRegistry.DecentraalBeheer.Vereniging.Geotags;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Framework;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling.FeitelijkeVereniging;
using AutoFixture;
using Common.StubsMocksFakes.VerenigingsRepositories;
using Moq;
using Xunit;

public class With_One_Locatie
{
    private readonly VerenigingRepositoryMock _verenigingRepositoryMock;
    private readonly FeitelijkeVerenigingWerdGeregistreerdWithALocatieScenario _scenario;
    private Fixture _fixture;

    public With_One_Locatie()
    {
        _scenario = new FeitelijkeVerenigingWerdGeregistreerdWithALocatieScenario();

        _verenigingRepositoryMock = new VerenigingRepositoryMock(_scenario.GetVerenigingState());

        _fixture = new Fixture().CustomizeAdminApi();
    }

    [Fact]
    public async ValueTask Then_A_VertegenwoordigerWerdVerwijderd_Event_Is_Saved()
    {
        var command = new VerwijderLocatieCommand(_scenario.VCode, _scenario.LocatieWerdToegevoegd.Locatie.LocatieId);
        var commandMetadata = _fixture.Create<CommandMetadata>();
        var commandHandler = new VerwijderLocatieCommandHandler(_verenigingRepositoryMock, Mock.Of<IGeotagsService>());
        await Assert.ThrowsAsync<LaatsteLocatieKanNietVerwijderdWorden>(async () => await commandHandler.Handle(
                                                                                    new CommandEnvelope<VerwijderLocatieCommand>(
                                                                                        command, commandMetadata),
                                                                                    CancellationToken.None));
    }
}
