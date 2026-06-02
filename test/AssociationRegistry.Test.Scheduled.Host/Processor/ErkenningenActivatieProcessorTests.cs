namespace AssociationRegistry.Test.Scheduled.Host.Processor;

using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Erkenningen.ActiveerErkenning;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Framework;
using AssociationRegistry.Scheduled.Host.Erkenningen;
using AssociationRegistry.Test.Common.Extensions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Wolverine;

public class ErkenningenActivatieProcessorTests
{
    [Fact]
    public async Task Given_No_Te_Activeren_Erkenningen_Then_MessageBus_Is_Never_Called()
    {
        var query = new Mock<ITeActiverenErkenningenQuery>();

        query
            .Setup(x => x.ExecuteAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<TeActiverenErkenning>());

        var messageBus = new Mock<IMessageBus>();

        var sut = new ErkenningenActivatieProcessor(
            query.Object,
            messageBus.Object,
            NullLogger<ErkenningenActivatieProcessor>.Instance
        );

        await sut.ActiveerErkenningen(CancellationToken.None);

        messageBus.Verify(
            x =>
                x.InvokeAsync(
                    It.IsAny<CommandEnvelope<ActiveerErkenningCommand>>(),
                    It.IsAny<CancellationToken>(),
                    It.IsAny<TimeSpan?>()
                ),
            Times.Never
        );
    }

    [Fact]
    public async Task Given_Te_Activeren_Erkenningen_Then_MessageBus_Is_Invoked_With_ActiveerErkenningCommand()
    {
        var first = new TeActiverenErkenning(VCode.Create("V0001001"), 1);
        var second = new TeActiverenErkenning(VCode.Create("V0001002"), 2);

        var query = new Mock<ITeActiverenErkenningenQuery>();

        query.Setup(x => x.ExecuteAsync(It.IsAny<CancellationToken>())).ReturnsAsync([first, second]);

        var messageBus = new Mock<IMessageBus>();

        var sut = new ErkenningenActivatieProcessor(
            query.Object,
            messageBus.Object,
            NullLogger<ErkenningenActivatieProcessor>.Instance
        );

        await sut.ActiveerErkenningen(CancellationToken.None);

        messageBus.VerifyCommandInvoked(
            new ActiveerErkenningCommand(first.VCode, first.ErkenningId),
            Times.Once(),
            CommandMetadata.ForDigitaalVlaanderenProcess.Initiator
        );

        messageBus.VerifyCommandInvoked(
            new ActiveerErkenningCommand(second.VCode, second.ErkenningId),
            Times.Once(),
            CommandMetadata.ForDigitaalVlaanderenProcess.Initiator
        );
    }
}
