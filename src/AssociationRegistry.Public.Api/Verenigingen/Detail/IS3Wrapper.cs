namespace AssociationRegistry.Public.Api.Verenigingen.Detail;

public interface IS3Wrapper
{
    public Task<string> GetPreSignedUrlAsync(CancellationToken cancellationToken);
    public Task PutAsync(Stream stream, CancellationToken cancellationToken);
}
