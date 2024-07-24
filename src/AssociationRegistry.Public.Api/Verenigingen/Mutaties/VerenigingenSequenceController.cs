namespace AssociationRegistry.Public.Api.Verenigingen.Mutaties;

using Asp.Versioning;
using Be.Vlaanderen.Basisregisters.Api;
using Be.Vlaanderen.Basisregisters.Api.Exceptions;
using Constants;
using Marten;
using Microsoft.AspNetCore.Mvc;
using Schema.Sequence;
using Swashbuckle.AspNetCore.Filters;
using ProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ProblemDetails;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[ApiRoute("verenigingen")]
[ApiExplorerSettings(GroupName = "Mutatiedienst")]
public class VerenigingenSequenceController : ApiController
{
    private readonly IDocumentStore _documentStore;

    public VerenigingenSequenceController(IDocumentStore documentStore)
    {
        _documentStore = documentStore;
    }

    /// <summary>
    ///     Opvragen gewijzigde vCodes.
    /// </summary>
    /// <param name="vCode">De unieke identificatie code van deze vereniging</param>
    /// <response code="200">Het detail van een vereniging</response>
    /// <response code="500">Er is een interne fout opgetreden.</response>
    [HttpGet("mutaties")]
    [ProducesResponseType(typeof(PubliekVerenigingSequenceResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(VerenigingSequenceResponseExamples))]
    [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
    [Produces(WellknownMediaTypes.Json)]
    public async Task<IActionResult> Detail(
        [FromQuery] long sinds,
        CancellationToken cancellationToken)
    {
        await using var session = _documentStore.LightweightSession();

        var docs = await session.Query<PubliekVerenigingSequenceDocument>()
                                .Where(w => w.Sequence >= sinds)
                                .ToListAsync(cancellationToken);

        var response = docs
           .Select(s => new PubliekVerenigingSequenceResponse()
            {
                VCode = s.VCode,
                Sequence = s.Sequence,
            })
           .OrderBy(o => o.Sequence);

        return Ok(response);
    }
}
