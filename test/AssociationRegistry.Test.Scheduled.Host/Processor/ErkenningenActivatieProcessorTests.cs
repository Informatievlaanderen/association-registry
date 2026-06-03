namespace AssociationRegistry.Test.Scheduled.Host.Processor;

using Admin.Schema.Erkenningen;
using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Erkenningen.ActiveerErkenning;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Framework;
using AssociationRegistry.Scheduled.Host.Erkenningen;
using AssociationRegistry.Scheduled.Host.Queries;
using AssociationRegistry.Test.Common.Extensions;
using AutoFixture;
using Common.AutoFixture;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Wolverine;

public class ActiveerErkenningenProcessorTests
{
    [Fact]
    public async Task Given_No_Te_Activeren_Erkenningen_Then_MessageBus_Is_Never_Called()
    {
        var query = new Mock<ITeActiverenErkenningenQuery>();

        query.Setup(x => x.ExecuteAsync(It.IsAny<CancellationToken>())).ReturnsAsync(Array.Empty<ErkenningDocument>());

        var messageBus = new Mock<IMessageBus>();

        var sut = new ActiveerErkenningenProcessor(
            query.Object,
            messageBus.Object,
            NullLogger<ActiveerErkenningenProcessor>.Instance
        );

        await sut.ActiveerErkenningen(CancellationToken.None);

        messageBus.Verify(
            x => x.SendAsync(It.IsAny<CommandEnvelope<ActiveerErkenningCommand>>(), It.IsAny<DeliveryOptions?>()),
            Times.Never
        );
    }

    [Fact]
    public async Task Given_Te_Activeren_Erkenningen_Then_MessageBus_Is_Invoked_With_ActiveerErkenningCommand()
    {
        var fixture = new Fixture().CustomizeDomain();
        var documents = fixture.CreateMany<ErkenningDocument>();

        var query = new Mock<ITeActiverenErkenningenQuery>();

        query.Setup(x => x.ExecuteAsync(It.IsAny<CancellationToken>())).ReturnsAsync(documents.ToArray());

        var messageBus = new Mock<IMessageBus>();

        var sut = new ActiveerErkenningenProcessor(
            query.Object,
            messageBus.Object,
            NullLogger<ActiveerErkenningenProcessor>.Instance
        );

        await sut.ActiveerErkenningen(CancellationToken.None);

        foreach (var document in documents)
        {
            messageBus.VerifyCommandSendAsync(
                new ActiveerErkenningCommand(document.VCode, document.ErkenningId),
                Times.Once(),
                CommandMetadata.ForDigitaalVlaanderenProcess.Initiator
            );
        }
    }
}
