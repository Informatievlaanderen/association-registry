namespace AssociationRegistry.Admin.Api.Verenigingen.Registreer.VerenigingZonderEigenRechtspersoonlijkheid;

using Asp.Versioning;
using Infrastructure;
using Infrastructure.Extensions;
using Infrastructure.Middleware;
using Infrastructure.Swagger.Annotations;
using Infrastructure.Validation;
using DuplicateVerenigingDetection;
using Framework;
using Hosts.Configuration.ConfigurationBindings;
using Vereniging;
using Be.Vlaanderen.Basisregisters.Api;
using Be.Vlaanderen.Basisregisters.Api.Exceptions;
using DecentraalBeheer.Registratie.RegistreerFeitelijkeVereniging;
using Examples;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using RequetsModels;
using ResultNet;
using Swashbuckle.AspNetCore.Filters;
using Wolverine;
using ProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ProblemDetails;
using ValidationProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ValidationProblemDetails;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[ApiRoute("verenigingen")]
[SwaggerGroup.Registratie]
public class RegistreerVerenigingZonderEigenRechtspersoonlijkheidController : ApiController
{
    private readonly AppSettings _appSettings;
    private readonly BevestigingsTokenHelper _bevestigingsTokenHelper;
    private readonly IMessageBus _bus;
    private readonly IValidator<RegistreerVerenigingZonderEigenRechtspersoonlijkheidRequest> _validator;

    public RegistreerVerenigingZonderEigenRechtspersoonlijkheidController(
        IMessageBus bus,
        IValidator<RegistreerVerenigingZonderEigenRechtspersoonlijkheidRequest> validator,
        AppSettings appSettings)
    {
        _bus = bus;
        _validator = validator;
        _appSettings = appSettings;
        _bevestigingsTokenHelper = new BevestigingsTokenHelper(_appSettings);
    }

    /// <summary>
    ///     Registreer een vereniging zonder eigen rechtspersoonlijkheid.
    /// </summary>
    /// <remarks>
    ///     Bij het registreren van de vereniging wordt een sequentie teruggegeven via de `VR-Sequence` header.
    ///     Deze waarde kan gebruikt worden in andere endpoints om op te volgen of de zonet geregistreerde vereniging
    ///     al is doorgestroomd naar deze endpoints.
    /// </remarks>
    /// <param name="request">De gegevens van de te registreren vereniging</param>
    /// <param name="metadataProvider"></param>
    /// <param name="bevestigingsToken">Dit token wordt gebruikt als bevestiging dat de vereniging uniek is,
    /// ondanks de voorgestelde duplicaten.</param>
    /// <response code="202">De vereniging zonder eigen rechtspersoonlijkheid werd geregistreerd.</response>
    /// <response code="400">Er was een probleem met de doorgestuurde waarden.</response>
    /// <response code="409">Er zijn één of meerdere mogelijke duplicaten van deze vereniging gevonden.</response>
    /// <response code="500">Er is een interne fout opgetreden.</response>
    [HttpPost("verenigingenzondereigenrechtspersoonlijkheid")]
    [ConsumesJson]
    [ProducesJson]
    [SwaggerRequestExample(typeof(RegistreerVerenigingZonderEigenRechtspersoonlijkheidRequest), typeof(RegistreerVerenigingZonderEigenRechtspersoonlijkheidRequestExamples))]
    [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
    [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ProblemAndValidationProblemDetailsExamples))]
    [SwaggerResponseExample(StatusCodes.Status409Conflict, typeof(PotentialDuplicatesResponseExamples))]
    [SwaggerResponseHeader(StatusCodes.Status202Accepted, WellknownHeaderNames.Sequence, type: "string",
                           description: "Het sequence nummer van deze request.")]
    [SwaggerResponseHeader(StatusCodes.Status202Accepted, name: "ETag", type: "string",
                           description: "De versie van de geregistreerde vereniging.")]
    [SwaggerResponseHeader(StatusCodes.Status202Accepted, name: "Location", type: "string",
                           description: "De locatie van de geregistreerde vereniging.")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(PotentialDuplicatesResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Post(
        [FromBody] RegistreerVerenigingZonderEigenRechtspersoonlijkheidRequest? request,
        [FromServices] ICommandMetadataProvider metadataProvider,
        [FromHeader(Name = WellknownHeaderNames.BevestigingsToken)]
        string? bevestigingsToken = null)
    {
        await _validator.NullValidateAndThrowAsync(request);

        var skipDuplicateDetection = _bevestigingsTokenHelper.IsValid(bevestigingsToken, request);
        Throw<InvalidBevestigingstokenProvided>.If(!string.IsNullOrWhiteSpace(bevestigingsToken) && !skipDuplicateDetection);

        var command = request.ToCommand()
            with
            {
                SkipDuplicateDetection = skipDuplicateDetection,
            };

        var metaData = metadataProvider.GetMetadata();
        var envelope = new CommandEnvelope<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand>(command, metaData);
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
