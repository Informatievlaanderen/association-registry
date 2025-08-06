namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Subtype.CommandHandling.VerfijnNaarFeitelijkeVereniging;

using AssociationRegistry.DecentraalBeheer.Acties.Subtype;
using Events;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Framework;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using AutoFixture;
using Common.StubsMocksFakes.VerenigingsRepositories;
using Xunit;

public class Given_A_Different_Subtype
{
    [Fact]
    public async ValueTask Then_It_Saves_A_SubtypeWerdVerfijndNaarFeitelijkeVereniging()
    {
        var fixture = new Fixture().CustomizeDomain();

        var scenario = new VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdScenario();

        var verenigingRepositoryMock = new VerenigingRepositoryMock(scenario.GetVerenigingState());

        var commandHandler = new VerfijnSubtypeNaarFeitelijkeVerenigingCommandHandler(verenigingRepositoryMock);

        var command = new VerfijnSubtypeNaarFeitelijkeVerenigingCommand(scenario.VCode);

        await commandHandler.Handle(new CommandEnvelope<VerfijnSubtypeNaarFeitelijkeVerenigingCommand>(command, fixture.Create<CommandMetadata>()));

        verenigingRepositoryMock.ShouldHaveSavedExact(
            new VerenigingssubtypeWerdVerfijndNaarFeitelijkeVereniging(
                scenario.VCode));
    }
}
