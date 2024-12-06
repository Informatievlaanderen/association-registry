namespace AssociationRegistry.Test.Projections.Framework;

using Marten;

public interface IProjectionContext
{
    IDocumentStore AdminStore { get; }
    Task SaveAsync(EventsPerVCode[] events, IDocumentSession session);
}
