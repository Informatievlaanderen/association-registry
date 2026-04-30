namespace AssociationRegistry.CommandHandling.Bewaartermijnen.MessageHandlers.
    Vertegenwoordigers;

using AssociationRegistry.DecentraalBeheer.Vereniging.Bewaartermijnen.Messages;

public class StartBewaartermijnMessageListener
{
    public static async Task Handle(
        StartBewaartermijnMessage message,
        IEventStore eventStore)
    {
        await message.CreateBewaartermijn(eventStore);
    }
}
