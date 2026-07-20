namespace AssociationRegistry.Admin.Api.WebApi.Verenigingen.InStopzetting;

using Asp.Versioning;
using Be.Vlaanderen.Basisregisters.Api;
using Be.Vlaanderen.Basisregisters.Api.Exceptions;
using CommandHandling.DecentraalBeheer.Acties.InStopzetting.UpdateInStopzetting;
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
public class InStopzettingController : ApiController
{
    private readonly IMessageBus _messageBus;

    public InStopzettingController(IMessageBus messageBus)
    {
        _messageBus = messageBus;
    }

    /// <summary>
    /// Zet een vereniging in of uit stopzetting.
    /// </summary>
    /// <remarks>
    /// Na het uitvoeren van deze actie wordt een sequentie teruggegeven via de `VR-Sequence` header.
    /// Deze waarde kan gebruikt worden in andere endpoints om op te volgen of de aanpassing
    /// al is doorgestroomd naar deze endpoints.
    /// </remarks>
    /// <param name="vCode">De VCode van de vereniging.</param>
    /// <param name="request">Het toe te voegen lidmaatschap.</param>
    /// <param name="validator">De validator voor het toevoegen van een lidmaatschap.</param>
    /// <param name="metadataProvider"></param>
    /// <param name="ifMatch">If-Match header met ETag van de laatst gekende versie van de vereniging.</param>
    /// <param name="cancellationToken"></param>
    /// <response code="202">Het lidmaatschap werd toegevoegd.</response>
    /// <response code="400">Er was een probleem met de doorgestuurde waarden.</response>
    /// <response code="412">De gevraagde vereniging heeft niet de verwachte sequentiewaarde.</response>
    /// <response code="500">Er is een interne fout opgetreden.</response>
    [HttpPost("{vCode}/in-stopzetting")]
    [ConsumesJson]
    [ProducesJson]
    [SwaggerRequestExample(typeof(InStopzettingRequest), typeof(InStopzettingRequestExample))]
    [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
    [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ProblemAndValidationProblemDetailsExamples))]
    [SwaggerResponseExample(StatusCodes.Status412PreconditionFailed, typeof(PreconditionFailedProblemDetailsExamples))]
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
    [SwaggerResponseHeader(
        StatusCodes.Status202Accepted,
        name: "Location",
        type: "string",
        description: "De locatie van het toegevoegde lidmaatschap."
    )]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status412PreconditionFailed)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    public async Task<IActionResult> VoegLidmaatschapToe(
        [FromRoute] string vCode,
        [FromBody] InStopzettingRequest request,
        [FromServices] ICommandMetadataProvider metadataProvider,
        [FromHeader(Name = "If-Match")] string? ifMatch = null,
        CancellationToken cancellationToken = default
    )
    {
        var metaData = metadataProvider.GetMetadata(IfMatchParser.ParseIfMatch(ifMatch));
        var envelope = new CommandEnvelope<UpdateInStopzettingCommand>(request.ToCommand(vCode), metaData);
        var commandResult = await _messageBus.InvokeAsync<CommandResult>(envelope, cancellationToken);

        return this.PostResponse(commandResult);
    }
}
