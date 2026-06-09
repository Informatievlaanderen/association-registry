namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Erkenning_Werd_Verlopen.
    CommandHandling.Vzer;

using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Erkenningen.ActiveerErkenning;
using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Erkenningen.VerloopErkenning;
using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;
using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen.Exceptions;
using AssociationRegistry.Framework;
using AutoFixture;
using Common.AutoFixture;
using Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using Common.StubsMocksFakes.VerenigingsRepositories;
using FluentAssertions;
using Xunit;

public class Given_Verlopen_Erkenning
{
    private readonly VerloopErkenningCommandHandler _commandHandler;
    private readonly Fixture _fixture;
    private readonly VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithErkenningenScenario _scenario;
    private readonly AggregateSessionMock _verenigingRepositoryMock;

    public Given_Verlopen_Erkenning()
    {
        _fixture = new Fixture().CustomizeAdminApi();

        _scenario = new VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithErkenningenScenario();
        _verenigingRepositoryMock = new AggregateSessionMock(_scenario.GetVerenigingState());

        _commandHandler = new VerloopErkenningCommandHandler(_verenigingRepositoryMock);
    }

    [Fact]
    public async ValueTask Then_No_Saved_Event()
    {
        var teActiverenErkenningId = _scenario.ErkenningWerdGeregistreerdInVerleden.ErkenningId;

        var command = _fixture.Create<VerloopErkenningCommand>() with
        {
            VCode = _scenario.VCode,
            ErkenningId = teActiverenErkenningId,
        };

        var commandMetadata = _fixture.Create<CommandMetadata>() with
        {
            Initiator = _scenario.ErkenningWerdGeregistreerdInVerleden.GeregistreerdDoor.OvoCode,
        };

        var exception = await Assert.ThrowsAsync<ErkenningKanNietVerlopenWorden>(async () =>
            await _commandHandler.Handle(new CommandEnvelope<VerloopErkenningCommand>(command, commandMetadata))
        );

        _verenigingRepositoryMock.ShouldNotHaveAnySaves();
        exception
           .Message.Should()
           .Be(
                string.Format(
                    "Erkenning met id: {0}, startdatum: {1}, einddatum: {2} en status: {3} kan niet verlopen worden.",
                    _scenario.ErkenningWerdGeregistreerdInVerleden.ErkenningId,
                    _scenario.ErkenningWerdGeregistreerdInVerleden.Startdatum.Value,
                    _scenario.ErkenningWerdGeregistreerdInVerleden.Einddatum.Value,
                    ErkenningStatus.Verlopen.Value
                )
            );
    }
}
