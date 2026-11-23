namespace AssociationRegistry.Admin.Api.WebApi.Relatieaanvragen.Opvragen;

using Asp.Versioning;
using AssociationRegistry.Admin.Api.Infrastructure.WebApi;
using AssociationRegistry.Admin.Api.Infrastructure.WebApi.Swagger.Annotations;
using AssociationRegistry.Admin.Api.Infrastructure.WebApi.Swagger.Examples;
using AssociationRegistry.Admin.Schema.Relatieaanvragen;
using Be.Vlaanderen.Basisregisters.Api;
using Be.Vlaanderen.Basisregisters.Api.Exceptions;
using Examples;
using Marten;
using Microsoft.AspNetCore.Mvc;
using ResponseModels;
using Swashbuckle.AspNetCore.Filters;
using Verenigingen.SequenceGuarding;
using ProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ProblemDetails;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[ApiRoute("relatieaanvragen")]
[SwaggerGroup.Opvragen]
public class DetailRelatieaanvraagController : ApiController
{
    /// <summary>
    ///     Vraag een relatieaanvraag op.
    /// </summary>
    /// <param name="sequenceGuarder"></param>
    /// <param name="documentStore"></param>
    /// <param name="problemDetailsHelper"></param>
    /// <param name="aanvraagId">De id van de relatieaanvraag.</param>
    /// <param name="expectedSequence">Sequentiewaarde verkregen bij het indienen van de relatieaanvraag.</param>
    /// <response code="200">De relatieaanvraag.</response>
    /// <response code="404">De gevraagde relatieaanvraag werd niet gevonden.</response>
    /// <response code="412">De relatieaanvragen hebben niet de verwachte sequentiewaarde.</response>
    /// <response code="500">Er is een interne fout opgetreden.</response>
    [HttpGet("{aanvraagId}")]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(RelatieaanvraagResponseExamples))]
    [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(NotFoundProblemDetailsExamples))]
    [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
    [ProducesResponseType(typeof(RelatieaanvraagResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status412PreconditionFailed)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesJson]
    public async Task<IActionResult> Detail(
        [FromServices] ISequenceGuarder<RelatieaanvraagDocument> sequenceGuarder,
        [FromServices] IDocumentStore documentStore,
        [FromServices] ProblemDetailsHelper problemDetailsHelper,
        [FromRoute] string aanvraagId,
        [FromQuery] long? expectedSequence,
        CancellationToken cancellationToken
    )
    {
        await sequenceGuarder.ThrowIfSequenceNotReached(expectedSequence);

        await using var session = documentStore.QuerySession();

        var relatieaanvraag = await session.LoadAsync<RelatieaanvraagDocument>(
            aanvraagId.ToUpperInvariant(),
            cancellationToken
        );

        if (relatieaanvraag is null)
            return await Response.WriteNotFoundProblemDetailsAsync(
                problemDetailsHelper,
                ValidationMessages.Status404Relatieaanvraag
            );

        return Ok(RelatieaanvraagResponse.From(relatieaanvraag));
    }
}
