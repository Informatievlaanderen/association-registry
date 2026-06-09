namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Erkenning_Werd_Verlopen.CommandHandling.Vzer;

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

public class Given_Erkenning_InAanvraag
{
    private readonly VerloopErkenningCommandHandler _commandHandler;
    private readonly Fixture _fixture;
    private readonly VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithErkenningenScenario _scenario;
    private readonly AggregateSessionMock _verenigingRepositoryMock;

    public Given_Erkenning_InAanvraag()
    {
        _fixture = new Fixture().CustomizeAdminApi();

        _scenario = new VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithErkenningenScenario();
        _verenigingRepositoryMock = new AggregateSessionMock(
            _scenario.GetVerenigingState(),
            expectedLoadingDubbel: true
        );

        _commandHandler = new VerloopErkenningCommandHandler(_verenigingRepositoryMock);
    }

    [Fact]
    public async ValueTask Then_No_Saved_Event()
    {
        var teVerlopenErkenningId = _scenario.ErkenningWerdGeregistreerdInToekomst.ErkenningId;

        var command = _fixture.Create<VerloopErkenningCommand>() with
        {
            VCode = _scenario.VCode,
            ErkenningId = teVerlopenErkenningId,
        };

        var commandMetadata = _fixture.Create<CommandMetadata>() with
        {
            Initiator = _scenario.ErkenningWerdGeregistreerdInToekomst.GeregistreerdDoor.OvoCode,
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
                    _scenario.ErkenningWerdGeregistreerdInToekomst.ErkenningId,
                    _scenario.ErkenningWerdGeregistreerdInToekomst.Startdatum.Value,
                    _scenario.ErkenningWerdGeregistreerdInToekomst.Einddatum.Value,
                    ErkenningStatus.InAanvraag.Value
                )
            );
    }
}
