namespace AssociationRegistry.Public.Api.Verenigingen.Detail;

using Asp.Versioning;
using Be.Vlaanderen.Basisregisters.Api;
using Be.Vlaanderen.Basisregisters.Api.Exceptions;
using Constants;
using Framework;
using Infrastructure.ConfigurationBindings;
using Infrastructure.Extensions;
using Marten;
using Microsoft.AspNetCore.Mvc;
using ResponseModels;
using Schema.Detail;
using Swashbuckle.AspNetCore.Filters;
using ProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ProblemDetails;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[ApiRoute("verenigingen")]
[ApiExplorerSettings(GroupName = "Opvragen van verenigingen")]
public class DetailVerenigingenController : ApiController
{
    private readonly AppSettings _appsettings;

    public DetailVerenigingenController(AppSettings appsettings)
    {
        _appsettings = appsettings;
    }

    /// <summary>
    ///     Vraag het detail van een vereniging op.
    /// </summary>
    /// <param name="vCode">De unieke identificatie code van deze vereniging</param>
    /// <response code="200">Het detail van een vereniging</response>
    /// <response code="404">De gevraagde vereniging is niet gevonden</response>
    /// <response code="500">Er is een interne fout opgetreden.</response>
    [HttpGet("{vCode}")]
    [ProducesResponseType(typeof(PubliekVerenigingDetailResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(DetailVerenigingResponseExamples))]
    [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
    [Produces(WellknownMediaTypes.JsonLd)]
    public async Task<IActionResult> Detail(
        [FromServices] IDocumentStore store,
        [FromRoute] string vCode)
    {
        await using var session = store.LightweightSession();

        var vereniging = await GetDetail(session, vCode);

        if (vereniging is null)
            return NotFound();

        return Ok(PubliekVerenigingDetailMapper.Map(vereniging, _appsettings));
    }

    // /// <summary>
    // ///     Vraag het detail van alle vereniging op.
    // /// </summary>
    // /// <response code="200">Het detail van alle vereniging</response>
    // /// <response code="500">Er is een interne fout opgetreden.</response>
    // [HttpGet("detail/all")]
    // [ProducesResponseType(typeof(PubliekVerenigingDetailResponse[]), StatusCodes.Status200OK)]
    // [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    // [SwaggerResponseExample(StatusCodes.Status200OK, typeof(DetailAllVerenigingResponseExamples))]
    // [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
    // [Produces(WellknownMediaTypes.JsonLd)]
    // public async Task GetAll(
    //     [FromServices] IQuery<IAsyncEnumerable<PubliekVerenigingDetailDocument>> query,
    //     [FromServices] IResponseWriter responseWriter,
    //     CancellationToken cancellationToken)
    // {
    //     try
    //     {
    //         var data = await query.ExecuteAsync(cancellationToken);
    //         await responseWriter.Write(Response, data, cancellationToken);
    //     }
    //     catch (TaskCanceledException)
    //     {
    //         // Nothing to do, user stopped the request
    //     }
    // }

    /// <summary>
    ///     Vraag het detail van alle vereniging op.
    /// </summary>
    /// <response code="302">Het detail van alle vereniging</response>
    /// <response code="500">Er is een interne fout opgetreden.</response>
    [HttpGet("detail/all")]
    [ProducesResponseType(typeof(PubliekVerenigingDetailResponse[]), StatusCodes.Status302Found)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [SwaggerResponseExample(StatusCodes.Status302Found, typeof(DetailAllVerenigingResponseExamples))]
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

    private static async Task<PubliekVerenigingDetailDocument?> GetDetail(IQuerySession session, string vCode)
        => await session
                .Query<PubliekVerenigingDetailDocument>()
                .WithVCode(vCode)
                .OnlyIngeschrevenInPubliekeDatastroom()
                .OnlyActief()
                .SingleOrDefaultAsync();
}
