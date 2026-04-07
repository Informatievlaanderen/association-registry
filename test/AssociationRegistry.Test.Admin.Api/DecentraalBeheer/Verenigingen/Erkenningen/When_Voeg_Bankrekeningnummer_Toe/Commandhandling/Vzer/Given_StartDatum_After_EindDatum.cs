namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Voeg_Bankrekeningnummer_Toe.Commandhandling.Vzer;

using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;
using AssociationRegistry.DecentraalBeheer.Vereniging.Exceptions;
using AssociationRegistry.Framework;
using AutoFixture;
using CommandHandling.DecentraalBeheer.Acties.Erkenningen.RegistreerErkenning;
using Common.AutoFixture;
using Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using Common.StubsMocksFakes.VerenigingsRepositories;
using FluentAssertions;
using Resources;
using Xunit;

public class Given_StartDatum_After_EindDatum
{
    private readonly RegistreerErkenningCommandHandler _commandHandler;
    private readonly Fixture _fixture;
    private readonly VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdScenario _scenario;
    private readonly AggregateSessionMock _aggregateSessionMock;

    public Given_StartDatum_After_EindDatum()
    {
        _fixture = new Fixture().CustomizeAdminApi();

        _scenario = new VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdScenario();
        _aggregateSessionMock = new AggregateSessionMock(_scenario.GetVerenigingState());

        _commandHandler = new RegistreerErkenningCommandHandler(_aggregateSessionMock);
    }

    [Fact]
    public async ValueTask Then_An_StartdatumMustBeBeforeEinddatum_Exception_Is_Thrown()
    {

        var command = _fixture.Create<RegistreerErkenningCommand>() with
        {
            VCode = _scenario.VCode,
            Erkenning = _fixture.Create<TeRegistrerenErkenning>() with
            {
                Startdatum = new DateOnly(2026, 1, 1),
                Einddatum = new DateOnly(2026, 1, 1),

            },
        };

        var exception = await Assert.ThrowsAsync<EinddatumLigtVoorStartdatum>(async () =>
                                                                                  await _commandHandler.Handle(
                                                                                      new CommandEnvelope<RegistreerErkenningCommand>(command, _fixture.Create<CommandMetadata>())
                                                                                  )
        );

        exception.Message.Should().Be(ExceptionMessages.StartdatumIsAfterEinddatum);
    }
}
