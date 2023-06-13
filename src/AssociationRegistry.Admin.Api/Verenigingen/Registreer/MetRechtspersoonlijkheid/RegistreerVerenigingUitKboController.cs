namespace AssociationRegistry.Admin.Api.Verenigingen.Registreer.MetRechtspersoonlijkheid;

using System;
using System.Threading.Tasks;
using Acties.RegistreerVerenigingUitKbo;
using Infrastructure;
using Infrastructure.ConfigurationBindings;
using Infrastructure.Extensions;
using Framework;
using Vereniging;
using Be.Vlaanderen.Basisregisters.Api;
using Be.Vlaanderen.Basisregisters.Api.Exceptions;
using DuplicateVerenigingDetection;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NodaTime;
using ResultNet;
using Swashbuckle.AspNetCore.Filters;
using Wolverine;
using ProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ProblemDetails;
using ValidationProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ValidationProblemDetails;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[ApiRoute("verenigingen/kbo")]
[ApiExplorerSettings(GroupName = "Decentraal beheer van verenigingen met rechtspersoonlijkheid")]
public class RegistreerVerenigingUitKboController : ApiController
{
    private readonly AppSettings _appSettings;
    private readonly IMessageBus _bus;
    private readonly IValidator<RegistreerVerenigingUitKboRequest> _validator;

    public RegistreerVerenigingUitKboController(
        IMessageBus bus,
        IValidator<RegistreerVerenigingUitKboRequest> validator,
        AppSettings appSettings)
    {
        _bus = bus;
        _validator = validator;
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
    /// <param name="initiator">Initiator header met als waarde de instantie die de registratie uitvoert.</param>
    /// <response code="202">De vereniging met rechtspersoonlijkheid werd geregistreerd.</response>
    /// <response code="400">Er was een probleem met de doorgestuurde waarden.</response>
    /// <response code="500">Er is een interne fout opgetreden.</response>
    [HttpPost("")]
    [Consumes("application/json")]
    [Produces("application/json")]
    [SwaggerRequestExample(typeof(RegistreerVerenigingUitKboRequest), typeof(RegistreerVerenigingUitKboRequestExamples))]
    [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
    [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ValidationProblemDetailsExamples))]
    [SwaggerResponseHeader(StatusCodes.Status202Accepted, WellknownHeaderNames.Sequence, "string", "Het sequence nummer van deze request.")]
    [SwaggerResponseHeader(StatusCodes.Status202Accepted, "ETag", "string", "De versie van de geregistreerde vereniging.")]
    [SwaggerResponseHeader(StatusCodes.Status202Accepted, "Location", "string", "De locatie van de geregistreerde vereniging.")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status202Accepted)]
    public async Task<IActionResult> Post(
        [FromBody] RegistreerVerenigingUitKboRequest? request,
        [InitiatorHeader] string initiator)
    {
        await _validator.NullValidateAndThrowAsync(request);

        var command = request.ToCommand();

        var metaData = new CommandMetadata(initiator, SystemClock.Instance.GetCurrentInstant());
        var envelope = new CommandEnvelope<RegistreerVerenigingUitKboCommand>(command, metaData);
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
