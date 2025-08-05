namespace AssociationRegistry.Admin.Api.Verenigingen.Lidmaatschap.VoegLidmaatschapToe;

using Asp.Versioning;
using AssociationRegistry.Admin.Api.Infrastructure;
using AssociationRegistry.Admin.Api.Queries;
using AssociationRegistry.DecentraalBeheer.Lidmaatschappen.VoegLidmaatschapToe;
using AssociationRegistry.Framework;
using AssociationRegistry.Hosts.Configuration.ConfigurationBindings;
using AssociationRegistry.Vereniging;
using Be.Vlaanderen.Basisregisters.Api;
using Be.Vlaanderen.Basisregisters.Api.Exceptions;
using Examples;
using FluentValidation;
using Infrastructure.CommandMiddleware;
using Infrastructure.WebApi;
using Infrastructure.WebApi.ResponseWriter;
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
public class VoegLidmaatschapToeController : ApiController
{
    private readonly IMessageBus _messageBus;
    private readonly AppSettings _appSettings;

    public VoegLidmaatschapToeController(IMessageBus messageBus, AppSettings appSettings)
    {
        _messageBus = messageBus;
        _appSettings = appSettings;
    }

    /// <summary>
    /// Voeg een lidmaatschap toe.
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
    /// <param name="detailQuery"></param>
    /// <param name="responseWriter"></param>
    /// <param name="ifMatch">If-Match header met ETag van de laatst gekende versie van de vereniging.</param>
    /// <param name="cancellationToken"></param>
    /// <response code="202">Het lidmaatschap werd toegevoegd.</response>
    /// <response code="400">Er was een probleem met de doorgestuurde waarden.</response>
    /// <response code="412">De gevraagde vereniging heeft niet de verwachte sequentiewaarde.</response>
    /// <response code="500">Er is een interne fout opgetreden.</response>
    [HttpPost("{vCode}/lidmaatschappen")]
    [ConsumesJson]
    [ProducesJson]
    [SwaggerRequestExample(typeof(VoegLidmaatschapToeRequest), typeof(VoegLidmaatschapToeRequestExamples))]
    [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
    [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ProblemAndValidationProblemDetailsExamples))]
    [SwaggerResponseExample(StatusCodes.Status412PreconditionFailed, typeof(PreconditionFailedProblemDetailsExamples))]
    [SwaggerResponseHeader(StatusCodes.Status202Accepted, WellknownHeaderNames.Sequence, type: "string",
                           description: "Het sequence nummer van deze request.")]
    [SwaggerResponseHeader(StatusCodes.Status202Accepted, name: "ETag", type: "string",
                           description: "De versie van de geregistreerde vereniging.")]
    [SwaggerResponseHeader(StatusCodes.Status202Accepted, name: "Location", type: "string",
                           description: "De locatie van het toegevoegde lidmaatschap.")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status412PreconditionFailed)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    public async Task<IActionResult> VoegLidmaatschapToe(
        [FromRoute] string vCode,
        [FromBody] VoegLidmaatschapToeRequest request,
        [FromServices] IValidator<VoegLidmaatschapToeRequest> validator,
        [FromServices] ICommandMetadataProvider metadataProvider,
        [FromServices] IBeheerVerenigingDetailQuery detailQuery,
        [FromServices] IResponseWriter responseWriter,
        [FromHeader(Name = "If-Match")] string? ifMatch = null,
        CancellationToken cancellationToken = default)
    {
        await validator.NullValidateAndThrowAsync(request, cancellationToken: cancellationToken);

        var naam = (await detailQuery.ExecuteAsync(new BeheerVerenigingDetailFilter(request.AndereVereniging), cancellationToken))
            ?.Naam ?? string.Empty;

        var metaData = metadataProvider.GetMetadata(IfMatchParser.ParseIfMatch(ifMatch));
        var envelope = new CommandEnvelope<VoegLidmaatschapToeCommand>(request.ToCommand(vCode, naam), metaData);
        var commandResult = await _messageBus.InvokeAsync<EntityCommandResult>(envelope, cancellationToken);

        return this.AcceptedEntityCommand(responseWriter, _appSettings, WellKnownHeaderEntityNames.Lidmaatschappen, commandResult);
    }
}
