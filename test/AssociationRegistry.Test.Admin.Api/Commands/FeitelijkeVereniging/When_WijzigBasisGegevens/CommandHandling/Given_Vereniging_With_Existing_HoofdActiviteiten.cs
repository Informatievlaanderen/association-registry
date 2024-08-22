namespace AssociationRegistry.Test.Admin.Api.Commands.FeitelijkeVereniging.When_WijzigBasisGegevens.CommandHandling;

using Acties.WijzigBasisgegevens;
using AssociationRegistry.Framework;
using AutoFixture;
using Common.Framework;
using Common.Scenarios.CommandHandling;
using Framework;
using Test.Framework;
using Vereniging;
using Vereniging.Exceptions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_Vereniging_With_Existing_HoofdActiviteiten
{
    private readonly FeitelijkeVerenigingWerdGeregistreerdWithHoofdActiviteitenScenario _scenario;
    private readonly VerenigingRepositoryMock _verenigingRepositoryMock;
    private readonly Fixture _fixture;

    public Given_Vereniging_With_Existing_HoofdActiviteiten()
    {
        _scenario = new FeitelijkeVerenigingWerdGeregistreerdWithHoofdActiviteitenScenario();
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
                                                                                    new ClockStub(_fixture.Create<DateOnly>())));
    }
}
