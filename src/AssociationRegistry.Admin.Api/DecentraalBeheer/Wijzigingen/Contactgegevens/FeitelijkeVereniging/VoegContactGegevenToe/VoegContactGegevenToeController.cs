﻿namespace AssociationRegistry.Admin.Api.Verenigingen.Contactgegevens.FeitelijkeVereniging.VoegContactGegevenToe;

using Asp.Versioning;
using AssociationRegistry.Admin.Api.Infrastructure;
using AssociationRegistry.Admin.Api.Infrastructure.Extensions;
using AssociationRegistry.Admin.Api.Infrastructure.Middleware;
using AssociationRegistry.Admin.Api.Infrastructure.Swagger.Annotations;
using AssociationRegistry.Admin.Api.Infrastructure.Swagger.Examples;
using AssociationRegistry.Admin.Api.Infrastructure.Validation;
using AssociationRegistry.DecentraalBeheer.Contactgegevens.VoegContactgegevenToe;
using AssociationRegistry.Framework;
using AssociationRegistry.Hosts.Configuration.ConfigurationBindings;
using AssociationRegistry.Vereniging;
using Be.Vlaanderen.Basisregisters.Api;
using Be.Vlaanderen.Basisregisters.Api.Exceptions;
using Examples;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using RequestsModels;
using Swashbuckle.AspNetCore.Filters;
using Wolverine;
using ProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ProblemDetails;
using ValidationProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ValidationProblemDetails;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[ApiRoute("verenigingen")]
[SwaggerGroup.DecentraalBeheer]
public class VoegContactgegevenToeController : ApiController
{
    private readonly IMessageBus _messageBus;
    private readonly IValidator<VoegContactgegevenToeRequest> _validator;
    private readonly AppSettings _appSettings;

    public VoegContactgegevenToeController(IMessageBus messageBus, IValidator<VoegContactgegevenToeRequest> validator, AppSettings appSettings)
    {
        _messageBus = messageBus;
        _validator = validator;
        _appSettings = appSettings;
    }

    /// <summary>
    ///     Voeg een contactgegeven toe.
    /// </summary>
    /// <remarks>
    ///     Na het uitvoeren van deze actie wordt een sequentie teruggegeven via de `VR-Sequence` header.
    ///     Deze waarde kan gebruikt worden in andere endpoints om op te volgen of de aanpassing
    ///     al is doorgestroomd naar deze endpoints.
    /// </remarks>
    /// <param name="vCode">De VCode van de vereniging</param>
    /// <param name="request">Het toe te voegen contactgegeven</param>
    /// <param name="metadataProvider"></param>
    /// <param name="ifMatch">If-Match header met ETag van de laatst gekende versie van de vereniging.</param>
    /// <response code="202">Het contactgegeven werd goedgekeurd.</response>
    /// <response code="400">Er was een probleem met de doorgestuurde waarden.</response>
    /// <response code="412">De gevraagde vereniging heeft niet de verwachte sequentiewaarde.</response>
    /// <response code="500">Er is een interne fout opgetreden.</response>
    [HttpPost("{vCode}/contactgegevens")]
    [ConsumesJson]
    [ProducesJson]
    [SwaggerRequestExample(typeof(VoegContactgegevenToeRequest), typeof(VoegContactgegevenToeRequestExamples))]
    [SwaggerResponseHeader(StatusCodes.Status202Accepted, WellknownHeaderNames.Sequence, type: "string",
                           description: "Het sequence nummer van deze request.")]
    [SwaggerResponseHeader(StatusCodes.Status202Accepted, name: "ETag", type: "string",
                           description: "De versie van de geregistreerde vereniging.")]
    [SwaggerResponseHeader(StatusCodes.Status202Accepted, name: "Location", type: "string",
                           description: "De locatie van het toegevoegde contactgegeven.")]
    [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ProblemAndValidationProblemDetailsExamples))]
    [SwaggerResponseExample(StatusCodes.Status412PreconditionFailed, typeof(PreconditionFailedProblemDetailsExamples))]
    [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status412PreconditionFailed)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Post(
        [FromRoute] string vCode,
        [FromBody] VoegContactgegevenToeRequest request,
        [FromServices] ICommandMetadataProvider metadataProvider,
        [FromHeader(Name = "If-Match")] string? ifMatch = null)
    {
        await _validator.NullValidateAndThrowAsync(request);

        var metaData = metadataProvider.GetMetadata(IfMatchParser.ParseIfMatch(ifMatch));
        var envelope = new CommandEnvelope<VoegContactgegevenToeCommand>(request.ToCommand(vCode), metaData);
        var commandResult = await _messageBus.InvokeAsync<EntityCommandResult>(envelope);

        return this.AcceptedEntityCommand(_appSettings, WellKnownHeaderEntityNames.Contactgegevens, commandResult);
    }
}
