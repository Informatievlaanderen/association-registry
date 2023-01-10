namespace AssociationRegistry.Admin.Api.Verenigingen.Historiek;

using System.Linq;
using System.Threading.Tasks;
using Constants;
using Infrastructure.Extensions;
using Be.Vlaanderen.Basisregisters.Api;
using Be.Vlaanderen.Basisregisters.Api.Exceptions;
using Marten;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Projections.Historiek;
using Swashbuckle.AspNetCore.Filters;
using ProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ProblemDetails;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[ApiRoute("verenigingen")]
[ApiExplorerSettings(GroupName = "Verenigingen")]
public class VerenigingenHistoriekController : ApiController
{
    /// <summary>
    /// Vraag de historiek van een vereniging op.
    /// </summary>
    /// <response code="200">De historiek van een vereniging</response>
    /// <response code="404">De historiek van de gevraagde vereniging is niet gevonden</response>
    /// <response code="500">Als er een interne fout is opgetreden.</response>
    [HttpGet("{vCode}/historiek")]
    [ProducesResponseType(typeof(HistoriekResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(HistoriekResponseExamples))]
    [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
    [Produces(contentType: WellknownMediaTypes.Json)]
    public async Task<IActionResult> Historiek(
        [FromServices] IDocumentStore documentStore,
        [FromRoute] string vCode,
        [FromQuery] long? expectedSequence)
    {
        await using var session = documentStore.LightweightSession();

        if (!await session.HasReachedSequence<BeheerVerenigingHistoriekDocument>(expectedSequence))
            return StatusCode(StatusCodes.Status412PreconditionFailed);

        var maybeHistoriekVereniging = await session.Query<BeheerVerenigingHistoriekDocument>()
            .Where(document => document.VCode == vCode)
            .SingleOrDefaultAsync();

        if (maybeHistoriekVereniging is not { } historiek)
            return NotFound();

        return Ok(
            new HistoriekResponse(
                vCode,
                historiek.Gebeurtenissen.Select(
                    gebeurtenis => new HistoriekGebeurtenisResponse(
                        gebeurtenis.Gebeurtenis,
                        gebeurtenis.Initiator,
                        gebeurtenis.Tijdstip)).ToList()));
    }
}
