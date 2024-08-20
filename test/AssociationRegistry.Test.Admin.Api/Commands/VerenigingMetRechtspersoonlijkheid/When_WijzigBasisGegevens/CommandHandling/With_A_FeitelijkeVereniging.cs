namespace AssociationRegistry.Test.Admin.Api.Commands.VerenigingMetRechtspersoonlijkheid.When_WijzigBasisGegevens.CommandHandling;

using AssociationRegistry.Acties.VerenigingMetRechtspersoonlijkheid.WijzigBasisgegevens;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Admin.Api.Framework;
using AssociationRegistry.Test.Common.Framework;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling;
using AssociationRegistry.Vereniging.Exceptions;
using AutoFixture;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_A_FeitelijkeVereniging
{
    private readonly VerenigingRepositoryMock _verenigingRepositoryMock;
    private readonly WijzigBasisgegevensCommandHandler _commandHandler;
    private readonly CommandEnvelope<WijzigBasisgegevensCommand> _envelope;
    private const string NieuweKorteBeschrijving = "Een nieuwe beschrijving van de vereniging";

    public With_A_FeitelijkeVereniging()
    {
        var scenario = new FeitelijkeVerenigingWerdGeregistreerdScenario();
        _verenigingRepositoryMock = new VerenigingRepositoryMock(scenario.GetVerenigingState());

        var fixture = new Fixture().CustomizeAdminApi();
        var command = new WijzigBasisgegevensCommand(scenario.VCode, KorteBeschrijving: NieuweKorteBeschrijving);
        var commandMetadata = fixture.Create<CommandMetadata>();
        _commandHandler = new WijzigBasisgegevensCommandHandler();
        _envelope = new CommandEnvelope<WijzigBasisgegevensCommand>(command, commandMetadata);
    }

    [Fact]
    public async Task Then_A_UnsupportedOperationException_Is_Thrown()
    {
        var method = () => _commandHandler.Handle(_envelope, _verenigingRepositoryMock);
        await method.Should().ThrowAsync<ActieIsNietToegestaanVoorVerenigingstype>();
    }
}
