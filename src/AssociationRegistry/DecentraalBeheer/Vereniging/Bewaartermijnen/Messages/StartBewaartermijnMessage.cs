namespace AssociationRegistry.DecentraalBeheer.Vereniging.Bewaartermijnen.Messages;

using Events;
using Framework;
using NodaTime;

public class StartBewaartermijnMessage
{
    public string StreamKey { get; set; }
    public int EntityId { get; set; }
    public Instant Vervaldag { get; set; }
    public string Reden { get; set; }

    public StartBewaartermijnMessage(
        string streamKey,
        int entityId,
        Instant vervaldag,
        string reden)
    {
        StreamKey = streamKey;
        EntityId = entityId;
        Vervaldag = vervaldag;
        Reden = reden;
    }

    public async Task CreateBewaartermijn(IEventStore eventStore)
    {
        var bewaartermijnId = new BewaartermijnId(
            VCode.Create(StreamKey),
            PersoonsgegevensType.Vertegenwoordigers,
            EntityId
        );

        await eventStore.SaveNew(
            bewaartermijnId,
            CommandMetadata.ForDigitaalVlaanderenProcess,
            CancellationToken.None,
            [
                new BewaartermijnWerdGestartV2(
                    bewaartermijnId,
                    bewaartermijnId.VCode!,
                    bewaartermijnId.PersoonsgegevensType.Value,
                    bewaartermijnId.EntityId,
                    Vervaldag,
                    Reden
                ),
            ]
        );
    }
}
