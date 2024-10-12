namespace AssociationRegistry.Public.Api.Verenigingen.Detail;

public interface IS3Wrapper
{
    public Task GetAsync(string bucketName, string key, Stream stream, CancellationToken cancellationToken);
    public Task<string> GetPreSignedUrlAsync(string bucketName, string key, CancellationToken cancellationToken);
    public Task PutAsync(string bucketName, string key, Stream stream, CancellationToken cancellationToken);

}
