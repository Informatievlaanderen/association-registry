namespace AssociationRegistry.Test.Projections.Framework;

using Marten;

public interface IProjectionContext
{
    IDocumentSession Session { get; }
    Task SaveAsync(EventsPerVCode[] events);
    Task WaitForDataRefreshAsync();
}
