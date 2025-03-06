namespace AssociationRegistry.Test.Projections.Framework;

using Marten;
using System.Threading.Tasks;

public interface IProjectionContext
{
    IDocumentStore AdminStore { get; }
    Task SaveAsync(EventsPerVCode[] events, IDocumentSession session);
}
