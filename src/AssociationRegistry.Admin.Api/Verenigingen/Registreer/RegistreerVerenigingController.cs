namespace AssociationRegistry.Admin.Api.Verenigingen.Registreer;

using System;
using System.Threading.Tasks;
using Infrastructure.ConfigurationBindings;
using Infrastructure.Extensions;
using Framework;
using Vereniging;
using Be.Vlaanderen.Basisregisters.Api;
using Be.Vlaanderen.Basisregisters.Api.Exceptions;
using FluentValidation;
using Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NodaTime;
using ResultNet;
using Swashbuckle.AspNetCore.Filters;
using Vereniging.RegistreerVereniging;
using Wolverine;
using ProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ProblemDetails;
using ValidationProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ValidationProblemDetails;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[ApiRoute("verenigingen")]
[ApiExplorerSettings(GroupName = "Verenigingen")]
public class RegistreerVerenigingController : ApiController
{
    private readonly IMessageBus _bus;
    private readonly IValidator<RegistreerVerenigingRequest> _validator;
    private readonly BevestigingsTokenHelper _bevestigingsTokenHelper;
    private readonly AppSettings _appSettings;

    public RegistreerVerenigingController(
        IMessageBus bus,
        IValidator<RegistreerVerenigingRequest> validator,
        AppSettings appSettings)
    {
        _bus = bus;
        _validator = validator;
        _appSettings = appSettings;
        _bevestigingsTokenHelper = new BevestigingsTokenHelper(_appSettings);
    }

    /// <summary>
    /// Registreer een vereniging.
    /// </summary>
    /// <remarks>
    /// Bij het registreren van de vereniging wordt een sequentie teruggegeven via de `VR-Sequence` header.
    /// Deze waarde kan gebruikt worden in andere endpoints om op te volgen of de zonet geregistreerde vereniging
    /// al is doorgestroomd naar deze endpoints.
    /// </remarks>
    /// <response code="202">De vereniging is geregistreerd.</response>
    /// <response code="400">Er is een probleem met de doorgestuurde waarden. Zie body voor meer info.</response>
    /// <response code="409">Er zijn één of meerdere mogelijke duplicaten van deze vereniging gevonden.</response>
    /// <response code="500">Als er een interne fout is opgetreden.</response>
    [HttpPost]
    [Consumes("application/json")]
    [Produces("application/json")]
    [SwaggerRequestExample(typeof(RegistreerVerenigingRequest), typeof(RegistreerVerenigingenRequestExamples))]
    [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
    [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ValidationProblemDetailsExamples))]
    [SwaggerResponseExample(StatusCodes.Status409Conflict, typeof(PotentialDuplicatesResponseExamples))]
    [SwaggerResponseHeader(StatusCodes.Status202Accepted, WellknownHeaderNames.Sequence, "string", "Het sequence nummer van deze request.")]
    [SwaggerResponseHeader(StatusCodes.Status202Accepted, "ETag", "string", "De versie van de geregistreerde vereniging.")]
    [SwaggerResponseHeader(StatusCodes.Status202Accepted, "Location", "string", "De locatie van de geregistreerde vereniging.")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(PotentialDuplicatesResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status202Accepted)]
    public async Task<IActionResult> Post(
        [FromBody] RegistreerVerenigingRequest? request,
        [FromHeader(Name = WellknownHeaderNames.BevestigingsToken)]
        string? bevestigingsToken = null)
    {
        await _validator.NullValidateAndThrowAsync(request);

        var skipDuplicateDetection = _bevestigingsTokenHelper.IsValid(bevestigingsToken, request);
        Throw<InvalidBevestigingstokenProvided>.If(!string.IsNullOrWhiteSpace(bevestigingsToken) && !skipDuplicateDetection);

        var command = request.ToRegistreerVerenigingCommand()
            with
            {
                SkipDuplicateDetection = skipDuplicateDetection,
            };

        var metaData = new CommandMetadata(request.Initiator, SystemClock.Instance.GetCurrentInstant());
        var envelope = new CommandEnvelope<RegistreerVerenigingCommand>(command, metaData);
        var registratieResult = await _bus.InvokeAsync<Result>(envelope);

        return registratieResult switch
        {
            Result<CommandResult> commandResult => this.AcceptedCommand(_appSettings, commandResult.Data),

            Result<PotentialDuplicatesFound> potentialDuplicates => Conflict(
                new PotentialDuplicatesResponse(
                    _bevestigingsTokenHelper.Calculate(request),
                    potentialDuplicates.Data,
                    _appSettings)),

            _ => throw new ArgumentOutOfRangeException(),
        };
    }
}
