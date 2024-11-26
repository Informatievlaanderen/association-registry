namespace AssociationRegistry.Test.Projections.Framework;

public interface IProjectionContext
{
    Task SaveAsync(EventsPerVCode[] events);
    Task WaitForDataRefreshAsync();
}
