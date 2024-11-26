namespace AssociationRegistry.Public.Api.Verenigingen.DetailAll;

using Asp.Versioning;
using Be.Vlaanderen.Basisregisters.Api;
using Be.Vlaanderen.Basisregisters.Api.Exceptions;
using Constants;
using Microsoft.AspNetCore.Mvc;
using Queries;
using Swashbuckle.AspNetCore.Filters;
using ProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ProblemDetails;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[ApiRoute("verenigingen")]
[ApiExplorerSettings(GroupName = "Opvragen van verenigingen")]
public class DetailAllVerenigingenController : ApiController
{
    /// <summary>
    ///     Vraag het detail van alle vereniging op.
    /// </summary>
    /// <response code="302">Het detail van alle vereniging</response>
    /// <response code="500">Er is een interne fout opgetreden.</response>
    [HttpGet("detail/all")]
    [ProducesResponseType(StatusCodes.Status302Found)]
    [SwaggerResponseHeader(StatusCodes.Status302Found, name: "Location", type: "string",
                           description: "De locatie van het te downloaden bestand.")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
    [Produces(WellknownMediaTypes.JsonLd)]
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
