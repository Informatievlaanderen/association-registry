namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Activeer_Erkenning.CommandHandling.Vzer;

using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Erkenningen.ActiveerErkenning;
using AssociationRegistry.Events;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using AssociationRegistry.Test.Common.StubsMocksFakes.VerenigingsRepositories;
using AutoFixture;
using Xunit;

public class Given_Dubbele_Vereniging
{
    [Fact]
    public async ValueTask Then_It_Adds_An_ErkenningWerdGeactiveerd_Event()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var scenario = new VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithErkenningenScenario();

        var verenigingState = scenario
            .GetVerenigingState()
            .Apply(new VerenigingWerdGemarkeerdAlsDubbelVan(scenario.VCode.Value, "V0001001"));

        var verenigingRepositoryMock = new AggregateSessionMock(verenigingState, expectedLoadingDubbel: true);
        var commandHandler = new ActiveerErkenningCommandHandler(verenigingRepositoryMock);
        var teActiverenErkenningId = scenario.ErkenningWerdGeregistreerdTeActiveren.ErkenningId;

        var command = fixture.Create<ActiveerErkenningCommand>() with
        {
            VCode = scenario.VCode,
            ErkenningId = teActiverenErkenningId,
        };

        var commandMetadata = fixture.Create<CommandMetadata>() with
        {
            Initiator = scenario.ErkenningWerdGeregistreerdTeActiveren.GeregistreerdDoor.OvoCode,
        };

        await commandHandler.Handle(new CommandEnvelope<ActiveerErkenningCommand>(command, commandMetadata));

        verenigingRepositoryMock.ShouldHaveSavedExact(new ErkenningWerdGeactiveerd(command.ErkenningId));
    }
}
