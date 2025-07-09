namespace AssociationRegistry.Admin.Api.Verenigingen.Registreer.MetRechtspersoonlijkheid;

using Asp.Versioning;
using AssociationRegistry.Admin.Api.Infrastructure;
using AssociationRegistry.Admin.Api.Infrastructure.Extensions;
using AssociationRegistry.Admin.Api.Infrastructure.Middleware;
using AssociationRegistry.Admin.Api.Infrastructure.Swagger.Annotations;
using AssociationRegistry.Admin.Api.Infrastructure.Validation;
using AssociationRegistry.DecentraalBeheer.Registratie.RegistreerVerenigingUitKbo;
using AssociationRegistry.DuplicateVerenigingDetection;
using AssociationRegistry.Framework;
using AssociationRegistry.Hosts.Configuration.ConfigurationBindings;
using AssociationRegistry.Vereniging;
using Be.Vlaanderen.Basisregisters.Api;
using Be.Vlaanderen.Basisregisters.Api.Exceptions;
using Examples;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using RequestModels;
using ResultNet;
using Swashbuckle.AspNetCore.Filters;
using Wolverine;
using ProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ProblemDetails;
using ValidationProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ValidationProblemDetails;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[ApiRoute("verenigingen/kbo")]
[SwaggerGroup.Registratie]
public class RegistreerVerenigingUitKboController : ApiController
{
    private readonly AppSettings _appSettings;
    private readonly IValidator<RegistreerVerenigingUitKboRequest> _validator;
    private readonly IMessageBus _bus;

    public RegistreerVerenigingUitKboController(
        IValidator<RegistreerVerenigingUitKboRequest> validator,
        IMessageBus bus,
        AppSettings appSettings)
    {
        _validator = validator;
        _bus = bus;
        _appSettings = appSettings;
    }

    /// <summary>
    ///     Registreer een vereniging met rechtspersoonlijkheid vanuit de KBO.
    /// </summary>
    /// <remarks>
    ///     Bij het registreren van de vereniging wordt een sequentie teruggegeven via de `VR-Sequence` header.
    ///     Deze waarde kan gebruikt worden in andere endpoints om op te volgen of de zonet geregistreerde vereniging
    ///     al is doorgestroomd naar deze endpoints.
    /// </remarks>
    /// <param name="request">De gegevens van de te registreren vereniging</param>
    /// <param name="commandMetadataProvider"></param>
    /// <param name="initiator">Initiator header met als waarde de instantie die de registratie uitvoert.</param>
    /// <response code="200">De vereniging was reeds geregistreerd in het register.</response>
    /// <response code="202">De vereniging met rechtspersoonlijkheid werd geregistreerd.</response>
    /// <response code="400">Er was een probleem met de doorgestuurde waarden.</response>
    /// <response code="500">Er is een interne fout opgetreden.</response>
    [HttpPost("")]
    [ConsumesJson]
    [ProducesJson]
    [SwaggerRequestExample(typeof(RegistreerVerenigingUitKboRequest), typeof(RegistreerVerenigingUitKboRequestExamples))]
    [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
    [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ProblemAndValidationProblemDetailsExamples))]
    [SwaggerResponseHeader(StatusCodes.Status200OK, name: "Location", type: "string",
                           description: "De locatie van de geregistreerde vereniging.")]
    [SwaggerResponseHeader(StatusCodes.Status202Accepted, WellknownHeaderNames.Sequence, type: "string",
                           description: "Het sequence nummer van deze request.")]
    [SwaggerResponseHeader(StatusCodes.Status202Accepted, name: "ETag", type: "string",
                           description: "De versie van de geregistreerde vereniging.")]
    [SwaggerResponseHeader(StatusCodes.Status202Accepted, name: "Location", type: "string",
                           description: "De locatie van de geregistreerde vereniging.")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    public async Task<IActionResult> Post(
        [FromBody] RegistreerVerenigingUitKboRequest? request,
        [FromServices] ICommandMetadataProvider commandMetadataProvider,
        [FromServices] InitiatorProvider initiator)
    {
        await _validator.NullValidateAndThrowAsync(request);

        var command = request.ToCommand();

        var metadata = commandMetadataProvider.GetMetadata();
        var envelope = new CommandEnvelope<RegistreerVerenigingUitKboCommand>(command, metadata);

        var registratieResult = await _bus.InvokeAsync<Result>(envelope);

        return registratieResult switch
        {
            Result<CommandResult> commandResult => this.AcceptedCommand(_appSettings, commandResult.Data),
            Result<DuplicateKboFound> duplicateKboFound => DuplicateKboFoundResponse(_appSettings, duplicateKboFound.Data),
            _ => throw new ArgumentOutOfRangeException(),
        };
    }

    private OkResult DuplicateKboFoundResponse(AppSettings appSettings, DuplicateKboFound data)
    {
        Response.Headers.Location = $"{appSettings.BaseUrl}/v1/verenigingen/{data.VCode}";

        return Ok();
    }
}
