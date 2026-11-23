namespace AssociationRegistry.Admin.Api.WebApi.Relatieaanvragen.Opvragen;

using Asp.Versioning;
using AssociationRegistry.Admin.Api.Infrastructure;
using AssociationRegistry.Admin.Api.Infrastructure.WebApi;
using AssociationRegistry.Admin.Api.Infrastructure.WebApi.Swagger.Annotations;
using AssociationRegistry.Admin.Api.Infrastructure.WebApi.Swagger.Examples;
using AssociationRegistry.Admin.Schema.Relatieaanvragen;
using Be.Vlaanderen.Basisregisters.Api;
using Be.Vlaanderen.Basisregisters.Api.Exceptions;
using DecentraalBeheer.Relatieaanvragen;
using DecentraalBeheer.Vereniging;
using Examples;
using Marten;
using Microsoft.AspNetCore.Mvc;
using ResponseModels;
using Swashbuckle.AspNetCore.Filters;
using Verenigingen.SequenceGuarding;
using ProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ProblemDetails;
using ValidationProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ValidationProblemDetails;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[ApiRoute("verenigingen")]
[SwaggerGroup.Opvragen]
public class RelatieaanvragenVanVerenigingController : ApiController
{
    /// <summary>
    ///     Vraag de relatieaanvragen van een vereniging op.
    /// </summary>
    /// <remarks>
    ///     Geeft de relatieaanvragen waarin de vereniging de koepel of de vereniging is, ongeacht wie de aanvraag
    ///     indiende. Optioneel gefilterd op status: Aangevraagd, Bevestigd, Geweigerd, Ingetrokken of Vervallen.
    /// </remarks>
    /// <param name="sequenceGuarder"></param>
    /// <param name="documentStore"></param>
    /// <param name="vCode">De vCode van de vereniging.</param>
    /// <param name="status">Enkel de relatieaanvragen met deze status (optioneel).</param>
    /// <param name="expectedSequence">Sequentiewaarde verkregen bij het indienen van een relatieaanvraag.</param>
    /// <param name="cancellationToken"></param>
    /// <response code="200">De relatieaanvragen van de vereniging.</response>
    /// <response code="400">Er was een probleem met de doorgestuurde waarden.</response>
    /// <response code="412">De relatieaanvragen hebben niet de verwachte sequentiewaarde.</response>
    /// <response code="500">Er is een interne fout opgetreden.</response>
    [HttpGet("{vCode}/relatieaanvragen")]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(RelatieaanvragenResponseExamples))]
    [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ProblemAndValidationProblemDetailsExamples))]
    [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
    [ProducesResponseType(typeof(RelatieaanvragenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status412PreconditionFailed)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesJson]
    public async Task<IActionResult> RelatieaanvragenVanVereniging(
        [FromServices] ISequenceGuarder<RelatieaanvraagDocument> sequenceGuarder,
        [FromServices] IDocumentStore documentStore,
        [FromRoute] string vCode,
        [FromQuery] string? status,
        [FromQuery] long? expectedSequence,
        CancellationToken cancellationToken
    )
    {
        // The value objects check the input: an unknown status or an invalid vCode gives a 400.
        var vCodeWaarde = VCode.Create(vCode).Value;
        var statusWaarde = status is null ? null : RelatieaanvraagStatus.Parse(status).Value;

        await sequenceGuarder.ThrowIfSequenceNotReached(expectedSequence);

        await using var session = documentStore.QuerySession();

        var query = session
            .Query<RelatieaanvraagDocument>()
            .Where(x => x.KoepelVCode == vCodeWaarde || x.VerenigingVCode == vCodeWaarde);

        if (statusWaarde is not null)
            query = query.Where(x => x.Status == statusWaarde);

        var relatieaanvragen = await query.OrderBy(x => x.AanvraagId).ToListAsync(cancellationToken);

        return Ok(
            new RelatieaanvragenResponse
            {
                Relatieaanvragen = relatieaanvragen.Select(RelatieaanvraagResponse.From).ToArray(),
            }
        );
    }
}
