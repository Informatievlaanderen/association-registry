namespace AssociationRegistry.Admin.Api.Verenigingen.Detail;

using Be.Vlaanderen.Basisregisters.Api;
using Be.Vlaanderen.Basisregisters.Api.Exceptions;
using Examples;
using Infrastructure;
using Infrastructure.Extensions;
using Marten;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ResponseModels;
using Schema.Detail;
using Swashbuckle.AspNetCore.Filters;
using System.Threading.Tasks;
using ProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ProblemDetails;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[ApiRoute("verenigingen")]
[ApiExplorerSettings(GroupName = "Opvragen van verenigingen")]
public class DetailVerenigingenController : ApiController
{
    private readonly BeheerVerenigingDetailMapper _mapper;

    public DetailVerenigingenController(BeheerVerenigingDetailMapper mapper)
    {
        _mapper = mapper;
    }

    /// <summary>
    ///     Vraag het detail van een vereniging op.
    /// </summary>
    /// <param name="documentStore"></param>
    /// <param name="vCode">De vCode van de vereniging</param>
    /// <param name="expectedSequence">Sequentiewaarde verkregen bij creatie of aanpassing vereniging.</param>
    /// <response code="200">Het detail van een vereniging</response>
    /// <response code="400">Er was een probleem met de doorgestuurde waarden.</response>
    /// <response code="404">De gevraagde vereniging werd niet gevonden</response>
    /// <response code="412">Het detail van de gevraagde vereniging heeft niet de verwachte sequentiewaarde.</response>
    /// <response code="500">Er is een interne fout opgetreden.</response>
    [HttpGet("{vCode}")]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(DetailVerenigingResponseExamples))]
    [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
    [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ProblemDetailsExamples))]
    [SwaggerResponseHeader(StatusCodes.Status200OK, name: "ETag", type: "string", description: "De versie van de aangepaste vereniging.")]
    [ProducesResponseType(typeof(DetailVerenigingResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Detail(
        [FromServices] IDocumentStore documentStore,
        [FromRoute] string vCode,
        [FromQuery] long? expectedSequence)
    {
        await using var session = documentStore.LightweightSession();

        if (!await documentStore.HasReachedSequence<BeheerVerenigingDetailDocument>(expectedSequence))
            return StatusCode(StatusCodes.Status412PreconditionFailed);

        var maybeVereniging = await session.Query<BeheerVerenigingDetailDocument>()
                                           .WithVCode(vCode)
                                           .SingleOrDefaultAsync();

        if (maybeVereniging is not { } vereniging)
            return NotFound();

        Response.AddETagHeader(vereniging.Metadata.Version);

        return Ok(_mapper.Map(vereniging));
    }
}
