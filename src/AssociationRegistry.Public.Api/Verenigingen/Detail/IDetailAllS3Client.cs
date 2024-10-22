namespace AssociationRegistry.Public.Api.Verenigingen.Detail;

public interface IDetailAllS3Client
{
    Task PutAsync(Stream stream, CancellationToken cancellationToken);
    Task<string> GetPreSignedUrlAsync(CancellationToken cancellationToken);
}
