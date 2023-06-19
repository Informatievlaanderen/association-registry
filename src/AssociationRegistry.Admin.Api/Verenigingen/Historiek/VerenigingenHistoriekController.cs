namespace AssociationRegistry.Admin.Api.Verenigingen.Historiek;

using System.Threading.Tasks;
using Be.Vlaanderen.Basisregisters.Api;
using Be.Vlaanderen.Basisregisters.Api.Exceptions;
using Constants;
using Examples;
using Infrastructure.Extensions;
using Marten;
using Microsoft.AspNetCore.Http;


using Microsoft.AspNetCore.Mvc;
using ResponseModels;
using Schema.Historiek;
using Swashbuckle.AspNetCore.Filters;
using ProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ProblemDetails;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[ApiRoute("verenigingen")]
[ApiExplorerSettings(GroupName = "Opvragen van verenigingen")]
public class VerenigingenHistoriekController : ApiController
{
    private readonly VerenigingHistoriekResponseMapper _mapper;

    public VerenigingenHistoriekController(VerenigingHistoriekResponseMapper mapper)
    {
        _mapper = mapper;
    }

    /// <summary>
    ///     Vraag de historiek van een vereniging op.
    /// </summary>
    /// <param name="documentStore"></param>
    /// <param name="vCode">De vCode van de vereniging</param>
    /// <param name="expectedSequence">Sequentiewaarde verkregen bij creatie of aanpassing vereniging.</param>
    /// <response code="200">De historiek van een vereniging</response>
    /// <response code="404">De historiek van de gevraagde vereniging is niet gevonden</response>
    /// <response code="412">De historiek van de gevraagde vereniging heeft niet de verwachte sequentiewaarde.</response>
    /// <response code="500">Er is een interne fout opgetreden.</response>
    [HttpGet("{vCode}/historiek")]
    [ProducesResponseType(typeof(HistoriekResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(HistoriekResponseExamples))]
    [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
    [Produces(WellknownMediaTypes.Json)]
    public async Task<IActionResult> Historiek(
        [FromServices] IDocumentStore documentStore,
        [FromRoute] string vCode,
        [FromQuery] long? expectedSequence)
    {
        await using var session = documentStore.LightweightSession();

        if (!await session.HasReachedSequence<BeheerVerenigingHistoriekDocument>(expectedSequence))
            return StatusCode(StatusCodes.Status412PreconditionFailed);

        var maybeHistoriekVereniging = await session.Query<BeheerVerenigingHistoriekDocument>()
            .WithVCode(vCode)
            .SingleOrDefaultAsync();

        if (maybeHistoriekVereniging is not { } historiek)
            return NotFound();

        return Ok(
            _mapper.Map(vCode, historiek));
    }


}
