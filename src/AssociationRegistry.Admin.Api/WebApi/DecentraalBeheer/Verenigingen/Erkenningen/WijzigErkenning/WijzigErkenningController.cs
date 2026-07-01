namespace AssociationRegistry.Admin.Api.WebApi.Verenigingen.Erkenningen.WijzigErkenning;

using Asp.Versioning;
using Be.Vlaanderen.Basisregisters.Api;
using Be.Vlaanderen.Basisregisters.Api.Exceptions;
using CommandHandling.DecentraalBeheer.Acties.Erkenningen.WijzigErkenning;
using DecentraalBeheer.Vereniging;
using Examples;
using Extensions;
using FluentValidation;
using Framework;
using Infrastructure;
using Infrastructure.CommandMiddleware;
using Infrastructure.WebApi.Swagger.Annotations;
using Infrastructure.WebApi.Swagger.Examples;
using Infrastructure.WebApi.Validation;
using Microsoft.AspNetCore.Mvc;
using RequestModels;
using Swashbuckle.AspNetCore.Filters;
using Wolverine;
using ProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ProblemDetails;
using ValidationProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ValidationProblemDetails;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[ApiRoute("verenigingen")]
[SwaggerGroup.DecentraalBeheer]
public class WijzigErkenningController : ApiController
{
    private readonly IMessageBus _messageBus;
    private readonly IValidator<WijzigErkenningRequest> _validator;

    public WijzigErkenningController(IMessageBus messageBus, IValidator<WijzigErkenningRequest> validator)
    {
        _messageBus = messageBus;
        _validator = validator;
    }

    /// <summary>
    ///    Wijzig gegevens van een erkenning.
    /// </summary>
    /// <remarks>
    ///     Na het uitvoeren van deze actie wordt een sequentie teruggegeven via de `VR-Sequence` header.
    ///     Deze waarde kan gebruikt worden in andere endpoints om op te volgen of de aanpassing
    ///     al is doorgestroomd naar deze endpoints.
    /// </remarks>
    /// <param name="vCode">De vCode van de vereniging.</param>
    /// <param name="erkenningId">De id van de erkenning.</param>
    /// <param name="request">De te wijzigen gegevens van de erkenning.</param>
    /// <param name="metadataProvider"></param>
    /// <param name="ifMatch">If-Match header met ETag van de laatst gekende versie van de vereniging.</param>
    /// <response code="200">Er waren geen wijzigingen.</response>
    /// <response code="202">De erkenning werd gewijzigd.</response>
    /// <response code="400">Er was een probleem met de doorgestuurde waarden.</response>
    /// <response code="412">De gevraagde vereniging heeft niet de verwachte sequentiewaarde.</response>
    /// <response code="500">Er is een interne fout opgetreden.</response>
    [HttpPatch("{vCode}/erkenningen/{erkenningId}")]
    [ConsumesJson]
    [ProducesJson]
    [SwaggerRequestExample(typeof(WijzigErkenningRequest), typeof(WijzigErkenningRequestExamples))]
    [SwaggerResponseHeader(
        StatusCodes.Status202Accepted,
        WellknownHeaderNames.Sequence,
        type: "string",
        description: "Het sequence nummer van deze request."
    )]
    [SwaggerResponseHeader(
        StatusCodes.Status202Accepted,
        name: "ETag",
        type: "string",
        description: "De versie van de geregistreerde vereniging."
    )]
    [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ProblemAndValidationProblemDetailsExamples))]
    [SwaggerResponseExample(StatusCodes.Status412PreconditionFailed, typeof(PreconditionFailedProblemDetailsExamples))]
    [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status412PreconditionFailed)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Patch(
        [FromRoute] string vCode,
        [FromRoute] int erkenningId,
        [FromBody] WijzigErkenningRequest request,
        [FromServices] ICommandMetadataProvider metadataProvider,
        [FromHeader(Name = "If-Match")] string? ifMatch = null
    )
    {
        await _validator.NullValidateAndThrowAsync(request);

        var metaData = metadataProvider.GetMetadata(IfMatchParser.ParseIfMatch(ifMatch));
        var envelope = new CommandEnvelope<WijzigErkenningCommand>(request.ToCommand(vCode, erkenningId), metaData);
        var commandResult = await _messageBus.InvokeAsync<CommandResult>(envelope);

        return this.PatchResponse(commandResult);
    }
}
