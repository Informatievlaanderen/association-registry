namespace AssociationRegistry.DecentraalBeheer.Vereniging.Bewaartermijnen.Messages;

using NodaTime;

public class StartBewaartermijnVoorOverledenVertegenwoordigerMessage : StartBewaartermijnMessage
{
    public StartBewaartermijnVoorOverledenVertegenwoordigerMessage(string streamKey, int entityId, Instant vervaldag) :
        base(streamKey, entityId, vervaldag, BewaartermijnReden.KszSyncHeeftVertegenwoordigerAangeduidAlsOverleden)
    {
    }
}
