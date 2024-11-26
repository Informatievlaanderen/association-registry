namespace AssociationRegistry.Test.Projections.Framework;

using AssociationRegistry.Framework;

public interface IProjectionContext
{
    Task SaveAsync(string vCode, params IEvent[] events);
    Task WaitForDataRefreshAsync();
}
