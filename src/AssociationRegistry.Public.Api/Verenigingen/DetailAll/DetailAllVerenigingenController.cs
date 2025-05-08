namespace AssociationRegistry.Public.Api.Verenigingen.DetailAll;

using Asp.Versioning;
using Be.Vlaanderen.Basisregisters.Api;
using Microsoft.AspNetCore.Mvc;
using Queries;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[ApiRoute("verenigingen")]
public class DetailAllVerenigingenController : ApiController
{
    [HttpGet("detail/all")]
    public async Task<IActionResult> GetAll(
        [FromServices] IPubliekVerenigingenDetailAllQuery query,
        [FromServices] IDetailAllStreamWriter streamWriter,
        [FromServices] IDetailAllS3Client s3Client,
        CancellationToken cancellationToken)
    {
        var data = await query.ExecuteAsync(cancellationToken);

        var stream = await streamWriter.WriteAsync(data, cancellationToken);
        await s3Client.PutAsync(stream, cancellationToken);

        var preSignedUrl = await s3Client.GetPreSignedUrlAsync(cancellationToken);

        return Redirect(preSignedUrl);
    }
}
