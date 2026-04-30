namespace AssociationRegistry.CommandHandling.Bewaartermijnen.MessageHandlers.Vertegenwoordigers.
    VertegenwoordigerWerdAangeduidAlsOverleden;

using AssociationRegistry.DecentraalBeheer.Vereniging.Bewaartermijnen.Messages;

public class StartBewaartermijnVoorOverledenVertegenwoordigerMessageHandler
{
    public static async Task Handle(
        StartBewaartermijnVoorOverledenVertegenwoordigerMessage message,
        IEventStore eventStore)
    {
        await message.CreateBewaartermijn(eventStore);
    }
}
