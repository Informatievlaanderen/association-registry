namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Erkenning_Werd_Verlopen.CommandHandling.Kbo;

using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Erkenningen.VerloopErkenning;
using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;
using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen.Exceptions;
using AssociationRegistry.Framework;
using AutoFixture;
using Common.AutoFixture;
using Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;
using Common.StubsMocksFakes.VerenigingsRepositories;
using FluentAssertions;
using Xunit;

public class Given_Geschorste_Erkenning
{
    private readonly VerloopErkenningCommandHandler _commandHandler;
    private readonly Fixture _fixture;
    private readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithGeschorsteErkenningScenario _scenario;
    private readonly AggregateSessionMock _verenigingRepositoryMock;

    public Given_Geschorste_Erkenning()
    {
        _fixture = new Fixture().CustomizeAdminApi();

        _scenario = new VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithGeschorsteErkenningScenario();
        _verenigingRepositoryMock = new AggregateSessionMock(_scenario.GetVerenigingState());

        _commandHandler = new VerloopErkenningCommandHandler(_verenigingRepositoryMock);
    }

    [Fact]
    public async ValueTask Then_No_Saved_Event()
    {
        var teVerlopenErkenningId = _scenario.ErkenningWerdGeschorst.ErkenningId;

        var command = _fixture.Create<VerloopErkenningCommand>() with
        {
            VCode = _scenario.VCode,
            ErkenningId = teVerlopenErkenningId,
        };

        var commandMetadata = _fixture.Create<CommandMetadata>() with
        {
            Initiator = _scenario.ErkenningWerdGeregistreerd.GeregistreerdDoor.OvoCode,
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
                    _scenario.ErkenningWerdGeschorst.ErkenningId,
                    _scenario.ErkenningWerdGeregistreerd.Startdatum.Value,
                    _scenario.ErkenningWerdGeregistreerd.Einddatum.Value,
                    ErkenningStatus.Geschorst.Value
                )
            );
    }
}
