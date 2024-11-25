namespace AssociationRegistry.Test.Projections.Framework;

public interface IProjectionContext
{
    Task WaitForDataRefreshAsync();
}
