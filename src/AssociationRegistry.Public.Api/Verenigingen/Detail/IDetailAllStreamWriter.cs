namespace AssociationRegistry.Public.Api.Verenigingen.Detail;

using Schema.Detail;

public interface IDetailAllStreamWriter
{
    Task<MemoryStream> WriteAsync(IAsyncEnumerable<PubliekVerenigingDetailDocument> data, CancellationToken cancellationToken);
}
