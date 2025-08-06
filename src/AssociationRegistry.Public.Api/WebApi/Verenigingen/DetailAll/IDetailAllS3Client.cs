namespace AssociationRegistry.Public.Api.WebApi.Verenigingen.DetailAll;

public interface IDetailAllS3Client
{
    Task PutAsync(Stream stream, CancellationToken cancellationToken);
    Task<string> GetPreSignedUrlAsync(CancellationToken cancellationToken);
}
