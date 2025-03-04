namespace AssociationRegistry.Admin.Api.Verenigingen.Subtype;

using Asp.Versioning;
using Be.Vlaanderen.Basisregisters.Api;
using Be.Vlaanderen.Basisregisters.Api.Exceptions;
using DecentraalBeheer.Subtype;
using Examples;
using FluentValidation;
using Framework;
using Infrastructure;
using Infrastructure.Extensions;
using Infrastructure.Middleware;
using Infrastructure.Swagger.Annotations;
using Infrastructure.Swagger.Examples;
using Infrastructure.Validation;
using Microsoft.AspNetCore.Mvc;
using Queries;
using Swashbuckle.AspNetCore.Filters;
using Vereniging;
using Wolverine;
using ProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ProblemDetails;
using ValidationProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ValidationProblemDetails;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[ApiRoute("verenigingen")]
[SwaggerGroup.DecentraalBeheer]
public class WijzigSubtypeController : ApiController
{
    private readonly IMessageBus _messageBus;

    public WijzigSubtypeController(IMessageBus messageBus)
    {
        _messageBus = messageBus;
    }

    /// <summary>
    ///     Wijzig het subtype van een vereniging.
    /// </summary>
    /// <remarks>
    ///     Na het uitvoeren van deze actie wordt een sequentie teruggegeven via de `VR-Sequence` header.
    ///     Deze waarde kan gebruikt worden in andere endpoints om op te volgen of de aanpassing
    ///     al is doorgestroomd naar deze endpoints.
    /// </remarks>
    /// <param name="vCode">De VCode van de vereniging</param>
    /// <param name="request"></param>
    /// <param name="metadataProvider"></param>
    /// <param name="ifMatch">If-Match header met ETag van de laatst gekende versie van de vereniging.</param>
    /// <response code="202">De locatie werd goedgekeurd.</response>
    /// <response code="400">Er was een probleem met de doorgestuurde waarden.</response>
    /// <response code="412">De gevraagde vereniging heeft niet de verwachte sequentiewaarde.</response>
    /// <response code="500">Er is een interne fout opgetreden.</response>
    [HttpPatch("{vCode}/subtype")]
    [ConsumesJson]
    [ProducesJson]
    [SwaggerRequestExample(typeof(WijzigSubtypeRequest), typeof(WijzigSubtypeRequestExamples))]
    [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
    [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ProblemAndValidationProblemDetailsExamples))]
    [SwaggerResponseExample(StatusCodes.Status412PreconditionFailed, typeof(PreconditionFailedProblemDetailsExamples))]
    [SwaggerResponseHeader(StatusCodes.Status202Accepted, WellknownHeaderNames.Sequence, type: "string",
                           description: "Het sequence nummer van deze request.")]
    [SwaggerResponseHeader(StatusCodes.Status202Accepted, name: "ETag", type: "string",
                           description: "De versie van de geregistreerde vereniging.")]
    [SwaggerResponseHeader(StatusCodes.Status202Accepted, name: "Location", type: "string",
                           description: "De locatie van de toegevoegde locatie.")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status412PreconditionFailed)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    public async Task<IActionResult> WijzigSubtype(
        [FromRoute] string vCode,
        [FromBody] WijzigSubtypeRequest request,
        [FromServices] IBeheerVerenigingDetailQuery detailQuery,
        [FromServices] IValidator<WijzigSubtypeRequest> validator,
        [FromServices] ICommandMetadataProvider metadataProvider,
        [FromHeader(Name = "If-Match")] string? ifMatch = null,
        CancellationToken cancellationToken = default)
    {
        await validator.NullValidateAndThrowAsync(request, cancellationToken);

        var metaData = metadataProvider.GetMetadata(IfMatchParser.ParseIfMatch(ifMatch));

        // TODO: refactor and write test
        var naam = request.AndereVereniging is not null && request.Subtype == Subtype.SubVereniging.Code
            ? (await detailQuery.ExecuteAsync(new BeheerVerenigingDetailFilter(request.AndereVereniging), cancellationToken))?.Naam
            : string.Empty;

        var envelope = new CommandEnvelope<WijzigSubtypeCommand>(request.ToCommand(vCode, naam), metaData);

        var commandResult = await _messageBus.InvokeAsync<CommandResult>(envelope, cancellationToken);

        if (!commandResult.HasChanges())
            return Ok();

        Response.AddSequenceHeader(commandResult.Sequence);
        Response.AddETagHeader(commandResult.Version);

        return Accepted();
    }
}
