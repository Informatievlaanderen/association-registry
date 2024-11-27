namespace AssociationRegistry.Test.Projections.Framework;

using Marten;

public interface IProjectionContext
{
    Task SaveAsync(EventsPerVCode[] events);
    Task RefreshDataAsync();
}
