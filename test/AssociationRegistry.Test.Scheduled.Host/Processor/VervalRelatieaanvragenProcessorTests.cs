namespace AssociationRegistry.Test.Scheduled.Host.Processor;

using Admin.Schema.Relatieaanvragen;
using AssociationRegistry.Framework;
using AssociationRegistry.Scheduled.Host.Queries;
using AssociationRegistry.Scheduled.Host.Relatieaanvragen.Verval;
using CommandHandling.DecentraalBeheer.Acties.Relatieaanvragen.VervalRelatieaanvraag;
using Common.Extensions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Wolverine;

public class VervalRelatieaanvragenProcessorTests
{
    [Fact]
    public async Task Given_No_Te_Vervallen_Relatieaanvragen_Then_Nothing_Is_Sent()
    {
        var messageBus = new Mock<IMessageBus>();

        await Processor([], messageBus).VervalRelatieaanvragen(CancellationToken.None);

        messageBus.Verify(
            x => x.SendAsync(It.IsAny<CommandEnvelope<VervalRelatieaanvraagCommand>>(), It.IsAny<DeliveryOptions?>()),
            Times.Never
        );
    }

    [Fact]
    public async Task Given_Te_Vervallen_Relatieaanvragen_Then_A_Command_Is_Sent_For_Each()
    {
        var aanvraagIds = new[] { "RA0000001", "RA0000007" };
        var messageBus = new Mock<IMessageBus>();

        await Processor(aanvraagIds, messageBus).VervalRelatieaanvragen(CancellationToken.None);

        foreach (var aanvraagId in aanvraagIds)
            messageBus.VerifyCommandSendAsync(
                new VervalRelatieaanvraagCommand(aanvraagId),
                Times.Once(),
                CommandMetadata.ForDigitaalVlaanderenProcess.Initiator
            );
    }

    private static VervalRelatieaanvragenProcessor Processor(string[] aanvraagIds, Mock<IMessageBus> messageBus)
    {
        var query = new Mock<ITeVervallenRelatieaanvragenQuery>();

        query
            .Setup(x => x.ExecuteAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                aanvraagIds.Select(aanvraagId => new RelatieaanvraagDocument { AanvraagId = aanvraagId }).ToArray()
            );

        return new VervalRelatieaanvragenProcessor(
            query.Object,
            messageBus.Object,
            NullLogger<VervalRelatieaanvragenProcessor>.Instance
        );
    }
}
