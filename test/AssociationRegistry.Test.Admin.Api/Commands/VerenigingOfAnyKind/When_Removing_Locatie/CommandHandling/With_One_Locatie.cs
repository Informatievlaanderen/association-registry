namespace AssociationRegistry.Test.Admin.Api.Commands.VerenigingOfAnyKind.When_Removing_Locatie.CommandHandling;

using Acties.VerwijderLocatie;
using AssociationRegistry.Framework;
using AutoFixture;
using Common.AutoFixture;
using Common.Framework;
using Common.Scenarios.CommandHandling;
using Vereniging.Exceptions;
using Xunit;
using Xunit.Categories;

[UnitTest]
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
    public async Task Then_A_VertegenwoordigerWerdVerwijderd_Event_Is_Saved()
    {
        var command = new VerwijderLocatieCommand(_scenario.VCode, _scenario.LocatieWerdToegevoegd.Locatie.LocatieId);
        var commandMetadata = _fixture.Create<CommandMetadata>();
        var commandHandler = new VerwijderLocatieCommandHandler(_verenigingRepositoryMock);
        await Assert.ThrowsAsync<LaatsteLocatieKanNietVerwijderdWorden>(async () => await commandHandler.Handle(
                                                                                    new CommandEnvelope<VerwijderLocatieCommand>(
                                                                                        command, commandMetadata),
                                                                                    CancellationToken.None));
    }
}
