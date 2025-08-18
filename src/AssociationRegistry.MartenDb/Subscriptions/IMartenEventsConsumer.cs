namespace AssociationRegistry.MartenDb.Subscriptions;

public interface IMartenEventsConsumer
{
    Task InitializeProjectionAsync();
    Task ConsumeAsync(SubscriptionEventList eventList);
}
