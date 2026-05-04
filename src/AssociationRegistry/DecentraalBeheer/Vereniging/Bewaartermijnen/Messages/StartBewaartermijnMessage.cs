namespace AssociationRegistry.DecentraalBeheer.Vereniging.Bewaartermijnen.Messages;

using NodaTime;

public class StartBewaartermijnMessage
{
    public string StreamKey { get; }
    public string PersoonsgegevensType { get; }
    public int EntityId { get; }
    public Instant Vervaldag { get; }
    public string Reden { get; }

    public StartBewaartermijnMessage(
        string streamKey,
        string persoonsgegevensType,
        int entityId,
        Instant vervaldag,
        string reden
    )
    {
        StreamKey = streamKey;
        EntityId = entityId;
        Vervaldag = vervaldag;
        Reden = reden;
        PersoonsgegevensType = persoonsgegevensType;
    }
}
