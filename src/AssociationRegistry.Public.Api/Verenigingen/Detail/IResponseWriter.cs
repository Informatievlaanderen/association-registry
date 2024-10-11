namespace AssociationRegistry.Public.Api.Verenigingen.Detail;

using Schema.Detail;

public interface IResponseWriter
{
    Task Write(HttpResponse response, IAsyncEnumerable<PubliekVerenigingDetailDocument> data, CancellationToken cancellationToken);
    Task<string> WriteToS3(HttpResponse response, IAsyncEnumerable<PubliekVerenigingDetailDocument> data, CancellationToken cancellationToken);
}
