namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Registreer_Erkenning.Commandhandling;

using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Erkenningen.RegistreerErkenning;
using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;
using AssociationRegistry.DecentraalBeheer.Vereniging.Exceptions;
using AssociationRegistry.Framework;
using AssociationRegistry.Resources;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using AssociationRegistry.Test.Common.StubsMocksFakes.VerenigingsRepositories;
using AutoFixture;
using FluentAssertions;
using Xunit;

public class Given_StartDatum_After_EindDatum
{
    private readonly RegistreerErkenningCommandHandler _commandHandler;
    private readonly Fixture _fixture;
    private readonly VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdScenario _scenario;

    public Given_StartDatum_After_EindDatum()
    {
        _fixture = new Fixture().CustomizeAdminApi();

        _scenario = new VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdScenario();
        var aggregateSessionMock = new AggregateSessionMock(_scenario.GetVerenigingState());

        _commandHandler = new RegistreerErkenningCommandHandler(aggregateSessionMock);
    }

    [Fact]
    public async ValueTask Then_An_StartdatumMustBeBeforeEinddatum_Exception_Is_Thrown()
    {
        var startDatum = _fixture.Create<DateOnly>();
        var days = _fixture.Create<int>();

        var command = _fixture.Create<RegistreerErkenningCommand>() with
        {
            VCode = _scenario.VCode,
            Erkenning = _fixture.Create<TeRegistrerenErkenning>() with
            {
                Startdatum = startDatum,
                Einddatum = startDatum.AddYears(-days),
            },
        };

        var exception = await Assert.ThrowsAsync<StartdatumLigtNaEinddatum>(async () =>
            await _commandHandler.Handle(
                new CommandEnvelope<RegistreerErkenningCommand>(command, _fixture.Create<CommandMetadata>()),
                _fixture.Create<IpdcProduct>()
            )
        );

        exception.Message.Should().Be(ExceptionMessages.StartdatumIsAfterEinddatum);
    }
}
