namespace AssociationRegistry.Public.Api.Verenigingen.Detail;

using Schema.Detail;

public interface IDetailAllWriter
{
    Task WriteToS3Async(IAsyncEnumerable<PubliekVerenigingDetailDocument> data, CancellationToken cancellationToken);
}
