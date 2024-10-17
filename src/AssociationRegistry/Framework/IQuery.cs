namespace AssociationRegistry.Framework;

public interface IQuery<T>
{
    Task<T> ExecuteAsync(CancellationToken cancellationToken);
}

public interface IQuery<T, TFilter>
{
    Task<T> ExecuteAsync(TFilter filter, CancellationToken cancellationToken);
}
