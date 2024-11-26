namespace AssociationRegistry.Public.Api.Verenigingen.DetailAll;

using Schema.Detail;

public interface IDetailAllStreamWriter
{
    Task<MemoryStream> WriteAsync(IAsyncEnumerable<PubliekVerenigingDetailDocument> data, CancellationToken cancellationToken);
}
