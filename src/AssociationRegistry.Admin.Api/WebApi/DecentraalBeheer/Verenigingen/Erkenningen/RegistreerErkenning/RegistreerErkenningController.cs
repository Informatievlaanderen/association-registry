namespace AssociationRegistry.Admin.Api.WebApi.Verenigingen.Erkenningen.RegistreerErkenning;

using Asp.Versioning;
using AssociationRegistry.Admin.Api.Infrastructure;
using AssociationRegistry.Admin.Api.Infrastructure.CommandMiddleware;
using AssociationRegistry.Admin.Api.Infrastructure.WebApi.Swagger.Annotations;
using AssociationRegistry.Admin.Api.Infrastructure.WebApi.Swagger.Examples;
using AssociationRegistry.Admin.Api.Infrastructure.WebApi.Validation;
using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Bankrekeningnummers.VoegBankrekeningnummerToe.Examples;
using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Bankrekeningnummers.VoegBankrekeningnummerToe.RequestModels;
using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Extensions;
using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Bankrekeningen.VoegBankrekeningToe;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Framework;
using AssociationRegistry.Hosts.Configuration.ConfigurationBindings;
using Be.Vlaanderen.Basisregisters.Api;
using Be.Vlaanderen.Basisregisters.Api.Exceptions;
using CommandHandling.DecentraalBeheer.Acties.Erkenningen.RegistreerErkenning;
using Examples;
using FluentValidation;
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
public class RegistreerErkenningController : ApiController
{
    private readonly IMessageBus _messageBus;
    private readonly IValidator<RegistreerErkenningRequest> _validator;
    private readonly AppSettings _appSettings;

    public RegistreerErkenningController(
        IMessageBus messageBus,
        IValidator<RegistreerErkenningRequest> validator,
        AppSettings appSettings
    )
    {
        _messageBus = messageBus;
        _validator = validator;
        _appSettings = appSettings;
    }

    /// <summary>
    ///     Registreer een erkenning.
    /// </summary>
    /// <remarks>
    ///     Na het uitvoeren van deze actie wordt een sequentie teruggegeven via de `VR-Sequence` header.
    ///     Deze waarde kan gebruikt worden in andere endpoints om op te volgen of de aanpassing
    ///     al is doorgestroomd naar deze endpoints.
    /// </remarks>
    /// <param name="vCode">De vCode van de vereniging.</param>
    /// <param name="request">De gegevens van de te registreren erkenning.</param>
    /// <param name="metadataProvider"></param>
    /// <param name="ifMatch">If-Match header met ETag van de laatst gekende versie van de vereniging.</param>
    /// <response code="202">De Erkenning werd geregistreerd.</response>
    /// <response code="400">Er was een probleem met de doorgestuurde waarden.</response>
    /// <response code="412">De gevraagde vereniging heeft niet de verwachte sequentiewaarde.</response>
    /// <response code="500">Er is een interne fout opgetreden.</response>
    [HttpPost("{vCode}/erkenningen")]
    [ConsumesJson]
    [ProducesJson]
    [SwaggerRequestExample(typeof(RegistreerErkenningRequest), typeof(RegistreerErkenningRequestExamples))]
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
    public async Task<IActionResult> Post(
        [FromRoute] string vCode,
        [FromBody] RegistreerErkenningRequest request,
        [FromServices] ICommandMetadataProvider metadataProvider,
        [FromHeader(Name = "If-Match")] string? ifMatch = null
    )
    {
        await _validator.NullValidateAndThrowAsync(request);

        var metaData = metadataProvider.GetMetadata(IfMatchParser.ParseIfMatch(ifMatch));
        var envelope = new CommandEnvelope<RegistreerErkenningCommand>(request.ToCommand(vCode), metaData);
        var commandResult = await _messageBus.InvokeAsync<EntityCommandResult>(envelope);

        return this.PostResponse(_appSettings, WellKnownHeaderEntityNames.Erkenningen, commandResult);
    }
}
