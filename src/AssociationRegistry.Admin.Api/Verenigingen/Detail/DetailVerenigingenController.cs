namespace AssociationRegistry.Admin.Api.Verenigingen.Detail;

using Asp.Versioning;
using Be.Vlaanderen.Basisregisters.Api;
using Be.Vlaanderen.Basisregisters.Api.Exceptions;
using Examples;
using Hosts.Configuration.ConfigurationBindings;
using Infrastructure;
using Infrastructure.ResponseWriter;
using Infrastructure.Sequence;
using Infrastructure.Swagger.Annotations;
using Infrastructure.Swagger.Examples;
using Marten.Linq.SoftDeletes;
using Microsoft.AspNetCore.Mvc;
using Queries;
using ResponseModels;
using Swashbuckle.AspNetCore.Filters;
using ProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ProblemDetails;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[ApiRoute("verenigingen")]
[SwaggerGroup.Opvragen]
public class DetailAllVerenigingenController : ApiController
{
    private readonly AppSettings _appSettings;

    public DetailAllVerenigingenController(AppSettings appSettings)
    {
        _appSettings = appSettings;
    }

    /// <summary>
    ///     Vraag het detail van een vereniging op.
    /// </summary>
    /// <param name="sequenceGuarder"></param>
    /// <param name="problemDetailsHelper"></param>
    /// <param name="detailQuery"></param>
    /// <param name="getNamesForVCodesQuery"></param>
    /// <param name="responseWriter"></param>
    /// <param name="vCode">De vCode van de vereniging</param>
    /// <param name="expectedSequence">Sequentiewaarde verkregen bij creatie of aanpassing vereniging.</param>
    /// <param name="cancellationToken"></param>
    /// <response code="200">Het detail van een vereniging</response>
    /// <response code="400">Er was een probleem met de doorgestuurde waarden.</response>
    /// <response code="404">De gevraagde vereniging werd niet gevonden</response>
    /// <response code="412">Het detail van de gevraagde vereniging heeft niet de verwachte sequentiewaarde.</response>
    /// <response code="500">Er is een interne fout opgetreden.</response>
    [HttpGet("{vCode}")]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(DetailVerenigingResponseExamples))]
    [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
    [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(BadRequestProblemDetailsExamples))]
    [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(NotFoundProblemDetailsExamples))]
    [SwaggerResponseExample(StatusCodes.Status412PreconditionFailed, typeof(DetailPreconditionFailedProblemDetailsExamples))]
    [SwaggerResponseHeader(StatusCodes.Status200OK, name: "ETag", type: "string", description: "De versie van de aangepaste vereniging.")]
    [ProducesResponseType(typeof(DetailVerenigingResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status412PreconditionFailed)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesJson]
    public async Task<IActionResult> Detail(
        [FromServices] ISequenceGuarder sequenceGuarder,
        [FromServices] IBeheerVerenigingDetailQuery detailQuery,
        [FromServices] IGetNamesForVCodesQuery getNamesForVCodesQuery,
        [FromServices] IResponseWriter responseWriter,
        [FromRoute] string vCode,
        [FromQuery] long? expectedSequence,
        CancellationToken cancellationToken)
    {
        await sequenceGuarder.ThrowIfSequenceNotReached(expectedSequence);

        var maybeVereniging = await detailQuery.ExecuteAsync(new BeheerVerenigingDetailFilter(vCode), cancellationToken);

        if (maybeVereniging is not { } vereniging)
            return await responseWriter.WriteNotFoundProblemDetailsAsync(Response, ValidationMessages.Status404Detail);

        if (maybeVereniging.Deleted)
            return await responseWriter.WriteNotFoundProblemDetailsAsync(Response, ValidationMessages.Status404Deleted);

        responseWriter.AddETagHeader(Response, vereniging.Metadata.Version);

        var andereVerenigingen = vereniging.Lidmaatschappen.Select(x => x.AndereVereniging).ToArray();

        var namesForLidmaatschappen =
            await getNamesForVCodesQuery.ExecuteAsync(new GetNamesForVCodesFilter(andereVerenigingen), cancellationToken);

        var mapper = new BeheerVerenigingDetailMapper(_appSettings, new VerplichteNamenVoorLidmaatschapMapper(namesForLidmaatschappen));
        var mappedDetail = mapper.Map(vereniging);

        return Ok(mappedDetail);
    }
}
