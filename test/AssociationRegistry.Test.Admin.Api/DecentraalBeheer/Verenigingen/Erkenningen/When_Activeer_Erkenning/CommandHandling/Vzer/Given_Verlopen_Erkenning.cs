namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Activeer_Erkenning.CommandHandling.Vzer;

using System.Threading.Tasks;
using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Erkenningen.ActiveerErkenning;
using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;
using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen.Exceptions;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;
using AssociationRegistry.Test.Common.StubsMocksFakes.VerenigingsRepositories;
using AutoFixture;
using Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using FluentAssertions;
using Xunit;

public class Given_Verlopen_Erkenning
{
    private readonly ActiveerErkenningCommandHandler _commandHandler;
    private readonly Fixture _fixture;
    private readonly VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithErkenningenScenario _scenario;
    private readonly AggregateSessionMock _verenigingRepositoryMock;

    public Given_Verlopen_Erkenning()
    {
        _fixture = new Fixture().CustomizeAdminApi();

        _scenario = new VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithErkenningenScenario();
        _verenigingRepositoryMock = new AggregateSessionMock(
            _scenario.GetVerenigingState(),
            expectedLoadingDubbel: true
        );

        _commandHandler = new ActiveerErkenningCommandHandler(_verenigingRepositoryMock);
    }

    [Fact]
    public async ValueTask Then_Saves_An_ErkenningWerdGeactiveerd_Event()
    {
        var teActiverenErkenningId = _scenario.ErkenningWerdGeregistreerdInVerleden.ErkenningId;

        var command = _fixture.Create<ActiveerErkenningCommand>() with
        {
            VCode = _scenario.VCode,
            ErkenningId = teActiverenErkenningId,
        };

        var commandMetadata = _fixture.Create<CommandMetadata>() with
        {
            Initiator = _scenario.ErkenningWerdGeregistreerdInVerleden.GeregistreerdDoor.OvoCode,
        };

        var exception = await Assert.ThrowsAsync<ErkenningKanNietGeactiveerdWorden>(async () =>
            await _commandHandler.Handle(new CommandEnvelope<ActiveerErkenningCommand>(command, commandMetadata))
        );

        _verenigingRepositoryMock.ShouldNotHaveAnySaves();
        exception
            .Message.Should()
            .Be(
                string.Format(
                    "Erkenning met id: {0}, startdatum: {1}, einddatum: {2} en status: {3} kan niet geactiveerd worden.",
                    _scenario.ErkenningWerdGeregistreerdInVerleden.ErkenningId,
                    _scenario.ErkenningWerdGeregistreerdInVerleden.Startdatum.Value,
                    _scenario.ErkenningWerdGeregistreerdInVerleden.Einddatum.Value,
                    ErkenningStatus.Verlopen.Value
                )
            );
    }
}
