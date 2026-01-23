namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Subtype.CommandHandling.VerfijnNaarFeitelijkeVereniging;

using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Subtype;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Framework;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using AutoFixture;
using Common.StubsMocksFakes.VerenigingsRepositories;
using Events;
using Xunit;

public class Given_The_Same_Subtype
{
    [Fact]
    public async ValueTask Then_No_Event_Is_Saved()
    {
        var fixture = new Fixture().CustomizeDomain();

        var scenario = new SubtypeWerdVerfijndNaarFeitelijkeVerenigingScenario();

        var verenigingRepositoryMock = new AggregateSessionMock(scenario.GetVerenigingState());

        var commandHandler = new VerfijnSubtypeNaarFeitelijkeVerenigingCommandHandler(verenigingRepositoryMock);

        var command = new VerfijnSubtypeNaarFeitelijkeVerenigingCommand(scenario.VCode);

        await commandHandler.Handle(
            new CommandEnvelope<VerfijnSubtypeNaarFeitelijkeVerenigingCommand>(
                command,
                fixture.Create<CommandMetadata>()
            )
        );

        verenigingRepositoryMock.ShouldNotHaveSaved<VerenigingssubtypeWerdVerfijndNaarFeitelijkeVereniging>();
    }
}
