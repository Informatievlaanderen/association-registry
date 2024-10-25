namespace AssociationRegistry.Public.Api.Verenigingen.Detail;

public interface IDetailAllFileWrapper
{
    public Task<string> GetPreSignedUrlAsync(CancellationToken cancellationToken);
    public Task PutAsync(Stream stream, CancellationToken cancellationToken);
}
