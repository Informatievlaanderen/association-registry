namespace AssociationRegistry.Public.Api.Queries;

public interface IQuery<T>
{
    Task<T> ExecuteAsync(CancellationToken cancellationToken);
}
