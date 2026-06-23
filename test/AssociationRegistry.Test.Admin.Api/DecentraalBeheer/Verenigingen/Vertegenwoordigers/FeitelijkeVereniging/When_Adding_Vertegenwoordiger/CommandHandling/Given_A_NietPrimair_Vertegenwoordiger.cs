namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Vertegenwoordigers.FeitelijkeVereniging.When_Adding_Vertegenwoordiger.CommandHandling;

using AssociationRegistry.DecentraalBeheer.Vereniging.Bewaartermijnen.Messages;
using Common.Scenarios.CommandHandling.FeitelijkeVereniging;
using Events;
using FluentAssertions;
using Moq;
using Wolverine;
using Xunit;

public class Given_A_NietPrimair_Vertegenwoordiger
{
    private readonly VoegVertegenwoordigerToeContext<FeitelijkeVerenigingWerdGeregistreerdScenario> _ctx =
        new(new FeitelijkeVerenigingWerdGeregistreerdScenario());

    [Fact]
    public async ValueTask Then_A_VertegenwoordigerWerdToegevoegd_Event_Is_Saved()
    {
        var command = _ctx.CreateCommand();

        await _ctx.Handle(command);

        _ctx.AggregateSessionMock.ShouldHaveSavedExact(
            new VertegenwoordigerWerdToegevoegd(
                _ctx.Scenario.FeitelijkeVerenigingWerdGeregistreerd.Vertegenwoordigers.Max(v => v.VertegenwoordigerId) + 1,
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
    public async ValueTask Then_A_EntityId_Is_Returned()
    {
        var command = _ctx.CreateCommand();

        var result = await _ctx.Handle(command);

        var vertegenwoordigerId = _ctx.AggregateSessionMock
            .SaveInvocations[0]
            .Vereniging.UncommittedEvents.ToArray()[0]
            .As<VertegenwoordigerWerdToegevoegd>()
            .VertegenwoordigerId;

        result.EntityId.Should().Be(vertegenwoordigerId);
    }

    [Fact]
    public async ValueTask Then_BewaartermijnMessage_Is_Not_Sent()
    {
        var command = _ctx.CreateCommand();

        await _ctx.Handle(command);

        _ctx.OutboxMock.Verify(
            x => x.SendAsync(
                It.IsAny<StartBewaartermijnMessage>(),
                It.IsAny<DeliveryOptions>()),
            Times.Never
        );
    }
}
