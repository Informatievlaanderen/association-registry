namespace AssociationRegistry.Test.Scheduled.Host.Processor;

using Admin.Schema.Erkenningen;
using AssociationRegistry.Framework;
using AssociationRegistry.Scheduled.Host.Erkenningen.Verloop;
using AssociationRegistry.Scheduled.Host.Queries;
using AutoFixture;
using CommandHandling.DecentraalBeheer.Acties.Erkenningen.VerloopErkenning;
using Common.AutoFixture;
using Common.Extensions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Wolverine;

public class VerloopErkenningenProcessorTests
{
    [Fact]
    public async Task Given_No_Te_Verlopen_Erkenningen_Then_MessageBus_Is_Never_Called()
    {
        var query = new Mock<ITeVerlopenErkenningenQuery>();

        query.Setup(x => x.ExecuteAsync(It.IsAny<CancellationToken>())).ReturnsAsync(Array.Empty<ErkenningDocument>());

        var messageBus = new Mock<IMessageBus>();

        var sut = new VerloopErkenningenProcessor(
            query.Object,
            messageBus.Object,
            NullLogger<VerloopErkenningenProcessor>.Instance
        );

        await sut.VerloopErkenningen(CancellationToken.None);

        messageBus.Verify(
            x => x.SendAsync(It.IsAny<CommandEnvelope<VerloopErkenningCommand>>(), It.IsAny<DeliveryOptions?>()),
            Times.Never
        );
    }

    [Fact]
    public async Task Given_Te_Verlopen_Erkenningen_Then_MessageBus_Is_Invoked_With_VerloopErkenningCommand()
    {
        var fixture = new Fixture().CustomizeDomain();
        var documents = fixture.CreateMany<ErkenningDocument>();

        var query = new Mock<ITeVerlopenErkenningenQuery>();

        query.Setup(x => x.ExecuteAsync(It.IsAny<CancellationToken>())).ReturnsAsync(documents.ToArray());

        var messageBus = new Mock<IMessageBus>();

        var sut = new VerloopErkenningenProcessor(
            query.Object,
            messageBus.Object,
            NullLogger<VerloopErkenningenProcessor>.Instance
        );

        await sut.VerloopErkenningen(CancellationToken.None);

        foreach (var document in documents)
        {
            messageBus.VerifyCommandSendAsync(
                new VerloopErkenningCommand(document.VCode, document.ErkenningId),
                Times.Once(),
                CommandMetadata.ForDigitaalVlaanderenProcess.Initiator
            );
        }
    }
}
