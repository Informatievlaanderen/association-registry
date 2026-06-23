namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Vertegenwoordigers.FeitelijkeVereniging.When_Adding_Vertegenwoordiger.CommandHandling;

using AssociationRegistry.DecentraalBeheer.Vereniging.Bewaartermijnen.Messages;
using AssociationRegistry.DecentraalBeheer.Vereniging.Exceptions;
using Common.Scenarios.CommandHandling.FeitelijkeVereniging;
using Moq;
using Wolverine;
using Xunit;

public class Given_A_Second_Primair_Vertegenwoordiger
{
    private readonly VoegVertegenwoordigerToeContext<FeitelijkeVerenigingWerdGeregistreerdScenario> _ctx =
        new(new FeitelijkeVerenigingWerdGeregistreerdScenario());

    [Fact]
    public async ValueTask Then_A_DuplicateVertegenwoordiger_Is_Thrown()
    {
        await _ctx.Handle(_ctx.CreatePrimairCommand());

        await Assert.ThrowsAsync<MeerderePrimaireVertegenwoordigers>(async () =>
            await _ctx.Handle(_ctx.CreatePrimairCommand()));
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
