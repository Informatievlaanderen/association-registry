namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Vertegenwoordigers.FeitelijkeVereniging.When_Removing_Vertegenwoordiger.CommandHandling;

using AssociationRegistry.DecentraalBeheer.Vertegenwoordigers.VerwijderVertegenwoordiger;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Framework;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling.FeitelijkeVereniging;
using AssociationRegistry.Vereniging.Exceptions;
using AutoFixture;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_One_Vertegenwoordiger
{
    private readonly VerenigingRepositoryMock _verenigingRepositoryMock;
    private readonly FeitelijkeVerenigingWerdGeregistreerdWithOneVertegenwoordigerScenario _scenario;
    private readonly Fixture _fixture;

    public With_One_Vertegenwoordiger()
    {
        _scenario = new FeitelijkeVerenigingWerdGeregistreerdWithOneVertegenwoordigerScenario();
        _verenigingRepositoryMock = new VerenigingRepositoryMock(_scenario.GetVerenigingState());
        _fixture = new Fixture().CustomizeAdminApi();
    }

    [Fact]
    public async Task Then_Throws_LaatsteVertegenwoordigerKanNietVerwijderdWordenException()
    {
        var command = new VerwijderVertegenwoordigerCommand(_scenario.VCode, _scenario.VertegenwoordigerId);
        var commandMetadata = _fixture.Create<CommandMetadata>();
        var commandHandler = new VerwijderVertegenwoordigerCommandHandler(_verenigingRepositoryMock);
        await Assert.ThrowsAsync<LaatsteVertegenwoordigerKanNietVerwijderdWorden>(async () => await commandHandler.Handle(
                                                                                      new CommandEnvelope<VerwijderVertegenwoordigerCommand>(
                                                                                          command, commandMetadata),
                                                                                      CancellationToken.None));
    }
}
