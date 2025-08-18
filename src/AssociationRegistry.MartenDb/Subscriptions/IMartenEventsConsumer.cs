namespace AssociationRegistry.MartenDb.Subscriptions;

public interface IMartenEventsConsumer
{
    Task ConsumeAsync(SubscriptionEventList eventList);
}
