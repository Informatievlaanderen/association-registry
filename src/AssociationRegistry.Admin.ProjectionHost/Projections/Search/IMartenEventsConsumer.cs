namespace AssociationRegistry.Admin.ProjectionHost.Projections.Search;

using Marten.Events;

public interface IMartenEventsConsumer
{
    Task ConsumeAsync(IReadOnlyList<StreamAction> streamActions);
}
