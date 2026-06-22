namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Vertegenwoordigers.FeitelijkeVereniging.
    When_Adding_Vertegenwoordiger.CommandHandling;

using AssociationRegistry.DecentraalBeheer.Vereniging.Bewaartermijnen;
using AssociationRegistry.DecentraalBeheer.Vereniging.Bewaartermijnen.Messages;
using Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using Events;
using Moq;
using Wolverine;
using Xunit;

public class Given_A_Gestopte_Vereniging
{
    private readonly VoegVertegenwoordigerToeContext<VerenigingZonderEigenRechtspersoonlijkheidWerdGestoptScenario> _ctx =
        new(new VerenigingZonderEigenRechtspersoonlijkheidWerdGestoptScenario());

    [Fact]
    public async ValueTask Then_A_VertegenwoordigerWerdToegevoegd_Event_Is_Saved()
    {
        var command = _ctx.CreateCommand();

        await _ctx.Handle(command);

        _ctx.AggregateSessionMock.ShouldHaveSavedExact(
            new VertegenwoordigerWerdToegevoegd(
                _ctx.Scenario.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.Vertegenwoordigers
                    .Max(v => v.VertegenwoordigerId) + 1,
                command.Vertegenwoordiger.Insz,
                command.Vertegenwoordiger.IsPrimair,
                command.Vertegenwoordiger.Roepnaam ?? string.Empty,
                command.Vertegenwoordiger.Rol ?? string.Empty,
                command.Vertegenwoordiger.Voornaam,
                command.Vertegenwoordiger.Achternaam,
                command.Vertegenwoordiger.Email.Waarde,
                command.Vertegenwoordiger.Telefoon.Waarde,
                command.Vertegenwoordiger.Mobiel.Waarde,
                command.Vertegenwoordiger.SocialMedia.Waarde
            )
        );
    }

    [Fact]
    public async ValueTask Then_A_BewaartermijnCommand_Is_Sent()
    {
        var command = _ctx.CreateCommand();

        await _ctx.Handle(command);

        var toegevoegdeVertegenwoordigerId = _ctx.Scenario.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd
                                                 .Vertegenwoordigers.Max(v => v.VertegenwoordigerId) + 1;

        _ctx.OutboxMock.Verify(
            x => x.SendAsync(
                It.Is<StartBewaartermijnMessage>(m =>
                    m.StreamKey == _ctx.Scenario.VCode.Value &&
                    m.PersoonsgegevensType == PersoonsgegevensType.Vertegenwoordigers.Value &&
                    m.EntityId == toegevoegdeVertegenwoordigerId &&
                    m.Reden == BewaartermijnReden.VerenigingWerdGestopt),
                It.IsAny<DeliveryOptions>()),
            Times.Once
        );
    }
}
