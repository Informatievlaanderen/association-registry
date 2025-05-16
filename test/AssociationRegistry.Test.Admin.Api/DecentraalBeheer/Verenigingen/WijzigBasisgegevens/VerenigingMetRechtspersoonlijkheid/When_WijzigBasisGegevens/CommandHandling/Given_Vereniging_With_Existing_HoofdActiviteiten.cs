namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.WijzigBasisgegevens.VerenigingMetRechtspersoonlijkheid.When_WijzigBasisGegevens.CommandHandling;

using AssociationRegistry.DecentraalBeheer.Basisgegevens.VerenigingMetRechtspersoonlijkheid;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Framework;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;
using AssociationRegistry.Vereniging;
using AssociationRegistry.Vereniging.Exceptions;
using AutoFixture;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_Vereniging_With_Existing_HoofdActiviteiten
{
    private VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithHoofdActiviteitenScenario _scenario;
    private VerenigingRepositoryMock _verenigingRepositoryMock;
    private Fixture _fixture;

    public Given_Vereniging_With_Existing_HoofdActiviteiten()
    {
        _scenario = new VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithHoofdActiviteitenScenario();
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
