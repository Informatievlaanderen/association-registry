namespace AssociationRegistry.Framework;

public interface IQuery<T>
{
    Task<T> ExecuteAsync(CancellationToken cancellationToken);
}

public interface IQuery<T, TFilter>
{
    public Task<T> ExecuteAsync(TFilter filter, CancellationToken cancellationToken);
}
