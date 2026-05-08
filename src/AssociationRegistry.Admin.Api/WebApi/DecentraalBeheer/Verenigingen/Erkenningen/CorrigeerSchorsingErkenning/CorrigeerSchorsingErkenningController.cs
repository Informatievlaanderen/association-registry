namespace AssociationRegistry.Admin.Api.WebApi.Verenigingen.Erkenningen.CorrigeerSchorsingErkenning;

using Asp.Versioning;
using AssociationRegistry.Admin.Api.Infrastructure;
using AssociationRegistry.Admin.Api.Infrastructure.CommandMiddleware;
using AssociationRegistry.Admin.Api.Infrastructure.WebApi.Swagger.Annotations;
using AssociationRegistry.Admin.Api.Infrastructure.WebApi.Swagger.Examples;
using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Extensions;
using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Erkenningen.SchorsErkenning;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Framework;
using AssociationRegistry.Hosts.Configuration.ConfigurationBindings;
using Be.Vlaanderen.Basisregisters.Api;
using Be.Vlaanderen.Basisregisters.Api.Exceptions;
using CommandHandling.DecentraalBeheer.Acties.Erkenningen.CorrigeerSchorsingErkenning;
using Examples;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using RequestModels;
using SchorsErkenning.RequestModels;
using Swashbuckle.AspNetCore.Filters;
using Wolverine;
using ProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ProblemDetails;
using ValidationProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ValidationProblemDetails;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[ApiRoute("verenigingen")]
[SwaggerGroup.DecentraalBeheer]
public class SchorsErkenningController : ApiController
{
    private readonly IMessageBus _messageBus;
    private readonly IValidator<CorrigeerSchorsingErkenningRequest> _validator;
    private readonly AppSettings _appSettings;

    public SchorsErkenningController(
        IMessageBus messageBus,
        IValidator<CorrigeerSchorsingErkenningRequest> validator,
        AppSettings appSettings
    )
    {
        _messageBus = messageBus;
        _validator = validator;
        _appSettings = appSettings;
    }

    /// <summary>
    ///    Corrigeer de Schorsing van een erkenning.
    /// </summary>
    /// <remarks>
    ///     Na het uitvoeren van deze actie wordt een sequentie teruggegeven via de `VR-Sequence` header.
    ///     Deze waarde kan gebruikt worden in andere endpoints om op te volgen of de aanpassing
    ///     al is doorgestroomd naar deze endpoints.
    /// </remarks>
    /// <param name="vCode">De vCode van de vereniging.</param>
    /// <param name="erkenningId">De id van de erkenning.</param>
    /// <param name="request"></param>
    /// <param name="metadataProvider"></param>
    /// <param name="ifMatch">If-Match header met ETag van de laatst gekende versie van de vereniging.</param>
    /// <response code="202">De erkenning werd geschorst.</response>
    /// <response code="412">De gevraagde vereniging heeft niet de verwachte sequentiewaarde.</response>
    /// <response code="500">Er is een interne fout opgetreden.</response>
    [HttpPatch("{vCode}/erkenningen/{erkenningId}/schorsingen")]
    [ConsumesJson]
    [ProducesJson]
    [SwaggerRequestExample(
        typeof(CorrigeerSchorsingErkenningRequest),
        typeof(CorrigeerSchorsingErkenningRequestExamples)
    )]
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
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status412PreconditionFailed)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Patch(
        [FromRoute] string vCode,
        [FromRoute] int erkenningId,
        [FromBody] CorrigeerSchorsingErkenningRequest request,
        [FromServices] ICommandMetadataProvider metadataProvider,
        [FromHeader(Name = "If-Match")] string? ifMatch = null
    )
    {
        await _validator.ValidateAsync(request);

        var metaData = metadataProvider.GetMetadata(IfMatchParser.ParseIfMatch(ifMatch));
        var envelope = new CommandEnvelope<CorrigeerSchorsingErkenningCommand>(
            request.ToCommand(vCode, erkenningId),
            metaData
        );
        var commandResult = await _messageBus.InvokeAsync<CommandResult>(envelope);

        return this.PatchResponse(commandResult);
    }
}
