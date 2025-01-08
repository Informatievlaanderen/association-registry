namespace AssociationRegistry.Test.Admin.Api.Commands.FeitelijkeVereniging.When_WijzigBasisGegevens.CommandHandling.Werkingsgebieden;

using AssociationRegistry.Framework;
using AutoFixture;
using Common.AutoFixture;
using Common.Framework;
using Common.Scenarios.CommandHandling;
using DecentraalBeheer.Basisgegevens.FeitelijkeVereniging;
using Framework.Fakes;
using Vereniging;

public static class WerkingsgebiedenScenarioRunner
{
    public static VerenigingRepositoryMock Run(CommandhandlerScenarioBase scenario, Func<Fixture, Werkingsgebied[]> werkingsgebieden)
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var verenigingRepositoryMock = new VerenigingRepositoryMock(scenario.GetVerenigingState());
        var command = new WijzigBasisgegevensCommand(scenario.VCode, Werkingsgebieden: werkingsgebieden(fixture));
        var commandMetadata = fixture.Create<CommandMetadata>();
        var commandHandler = new WijzigBasisgegevensCommandHandler();

        commandHandler.Handle(
            new CommandEnvelope<WijzigBasisgegevensCommand>(command, commandMetadata),
            verenigingRepositoryMock,
            new ClockStub(fixture.Create<DateOnly>())).GetAwaiter().GetResult();

        return verenigingRepositoryMock;
    }
}
