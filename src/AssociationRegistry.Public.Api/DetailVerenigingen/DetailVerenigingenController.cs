namespace AssociationRegistry.Public.Api.DetailVerenigingen;

using System.Linq;
using System.Threading.Tasks;
using Be.Vlaanderen.Basisregisters.Api;
using Be.Vlaanderen.Basisregisters.Api.Exceptions;
using Constants;
using ListVerenigingen;
using Marten;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Projections;
using Swashbuckle.AspNetCore.Filters;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[ApiRoute("verenigingen")]
[ApiExplorerSettings(GroupName = "Verenigingen")]
public class DetailVerenigingenController : ApiController
{
    private readonly IVerenigingenRepository _verenigingenRepository;

    public DetailVerenigingenController(IVerenigingenRepository verenigingenRepository)
    {
        _verenigingenRepository = verenigingenRepository;
    }

    /// <summary>
    /// Vraag het detail van een vereniging op.
    /// </summary>
    /// <response code="200">Het detail van een vereniging</response>
    /// <response code="500">Als er een interne fout is opgetreden.</response>
    [HttpGet("static/{vCode}")]
    [ProducesResponseType(typeof(DetailVerenigingResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(DetailVerenigingResponseExamples))]
    [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
    [Produces(contentType: WellknownMediaTypes.JsonLd)]
    public async Task<IActionResult> Detail(
        [FromServices] AppSettings appsettings,
        [FromRoute] string vCode)
    {
        var maybeVereniging = await _verenigingenRepository.Detail(vCode);
        if (maybeVereniging is not { } vereniging)
            return NotFound();

        return Ok(new DetailVerenigingResponse($"{appsettings.BaseUrl}v1/contexten/detail-vereniging-context.json", vereniging));
    }

    /// <summary>
    /// Vraag het detail van een vereniging op.
    /// </summary>
    /// <response code="200">Het detail van een vereniging</response>
    /// <response code="500">Als er een interne fout is opgetreden.</response>
    [HttpGet("{vCode}")]
    [ProducesResponseType(typeof(DetailVerenigingResponseWithActualData), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(DetailVerenigingResponseExamples))]
    [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
    [Produces(contentType: WellknownMediaTypes.JsonLd)]
    public async Task<IActionResult> DetailWithActualData(
        [FromServices] AppSettings appsettings,
        [FromServices] IDocumentStore documentStore,
        [FromRoute] string vCode)
    {
        await using var session = documentStore.LightweightSession();
        var maybeVereniging = await session.Query<VerenigingDetailDocument>()
            .Where(document => document.VCode == vCode)
            .SingleOrDefaultAsync();

        if (maybeVereniging is not { } vereniging)
            return NotFound();

        return Ok(new DetailVerenigingResponseWithActualData(
            $"{appsettings.BaseUrl}v1/contexten/detail-vereniging-context.json",
            new VerenigingDetailWithActualData(
                vereniging.VCode,
                vereniging.Naam,
                vereniging.KorteNaam,
                vereniging.KorteBeschrijving,
                vereniging.Startdatum,
                vereniging.KboNummer,
                vereniging.Status,
                vereniging.DatumLaatsteAanpassing)));
    }
}
