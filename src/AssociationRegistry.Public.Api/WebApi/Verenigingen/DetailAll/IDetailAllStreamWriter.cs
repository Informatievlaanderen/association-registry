namespace AssociationRegistry.Public.Api.WebApi.Verenigingen.DetailAll;

using AssociationRegistry.Public.Schema.Detail;

public interface IDetailAllStreamWriter
{
    Task<MemoryStream> WriteAsync(IAsyncEnumerable<PubliekVerenigingDetailDocument> data, CancellationToken cancellationToken);
}
