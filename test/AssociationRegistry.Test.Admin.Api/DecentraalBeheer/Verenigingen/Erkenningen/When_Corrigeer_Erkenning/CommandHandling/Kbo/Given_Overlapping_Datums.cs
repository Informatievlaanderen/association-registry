namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Corrigeer_Erkenning.
    CommandHandling.Kbo;

using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Erkenningen.CorrigeerErkenning;
using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Erkenningen.SchorsErkenning;
using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;
using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen.Exceptions;
using AssociationRegistry.Events;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;
using AssociationRegistry.Test.Common.StubsMocksFakes.VerenigingsRepositories;
using AutoFixture;
using FluentAssertions;
using Primitives;
using Resources;
using Xunit;

public class Given_Overlapping_Datums
{
    private readonly CorrigeerErkenningCommandHandler _commandHandler;
    private readonly Fixture _fixture;
    private readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithErkenningenScenario _scenario;
    private readonly AggregateSessionMock _verenigingRepositoryMock;

    public Given_Overlapping_Datums()
    {
        _fixture = new Fixture().CustomizeAdminApi();

        _scenario = new VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithErkenningenScenario();
        _verenigingRepositoryMock = new AggregateSessionMock(_scenario.GetVerenigingState());

        _commandHandler = new CorrigeerErkenningCommandHandler(_verenigingRepositoryMock);
    }

    [Fact]
    public async ValueTask Then_Throws_ErkenningBestaatAl()
    {
        var teCorrigerenErkenning = _scenario.ErkenningWerdGeregistreerd1.ErkenningId;

        var command = _fixture.Create<CorrigeerErkenningCommand>() with
        {
            VCode = _scenario.VCode,
            Erkenning = _fixture.Create<TeCorrigerenErkenning>() with
            {
                ErkenningId = teCorrigerenErkenning,
                StartDatum = NullOrEmpty<DateOnly>.Create(_scenario.ErkenningWerdGeregistreerd2.Startdatum.Value),
                EindDatum = NullOrEmpty<DateOnly>.Create(_scenario.ErkenningWerdGeregistreerd2.Einddatum.Value),
                Hernieuwingsdatum = NullOrEmpty<DateOnly>.Create(_scenario.ErkenningWerdGeregistreerd2.Hernieuwingsdatum.Value)
            },
        };

        var commandMetadata = _fixture.Create<CommandMetadata>() with
        {
            Initiator = _scenario.ErkenningWerdGeregistreerd1.GeregistreerdDoor.OvoCode,
        };

        var commandEnvelope = new CommandEnvelope<CorrigeerErkenningCommand>(command, commandMetadata);

        var exception = await Assert.ThrowsAsync<ErkenningBestaatAl>(async () =>
        {
            await _commandHandler.Handle(commandEnvelope);
        });

        exception.Message.Should().Be(string.Format(ExceptionMessages.ErkenningBestaatAl, teCorrigerenErkenning));
    }
}
