namespace AssociationRegistry.Admin.Api.Verenigingen.Registreer.Afdeling;

using Acties.RegistreerAfdeling;
using Be.Vlaanderen.Basisregisters.Api;
using Be.Vlaanderen.Basisregisters.Api.Exceptions;
using DuplicateVerenigingDetection;
using Examples;
using FluentValidation;
using Framework;
using Infrastructure;
using Infrastructure.ConfigurationBindings;
using Infrastructure.Extensions;
using Infrastructure.Middleware;
using Infrastructure.Swagger;
using Infrastructure.Swagger.Annotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RequestModels;
using ResultNet;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.Threading.Tasks;
using Vereniging;
using Wolverine;
using ProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ProblemDetails;
using ValidationProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ValidationProblemDetails;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[ApiRoute("verenigingen/afdelingen")]
[SwaggerGroup.Registratie]
public class RegistreerAfdelingController : ApiController
{
    private readonly AppSettings _appSettings;
    private readonly BevestigingsTokenHelper _bevestigingsTokenHelper;
    private readonly IMessageBus _bus;
    private readonly IValidator<RegistreerAfdelingRequest> _validator;

    public RegistreerAfdelingController(
        IMessageBus bus,
        IValidator<RegistreerAfdelingRequest> validator,
        AppSettings appSettings)
    {
        _bus = bus;
        _validator = validator;
        _appSettings = appSettings;
        _bevestigingsTokenHelper = new BevestigingsTokenHelper(_appSettings);
    }

    /// <summary>
    ///     Registreer een afdeling.
    /// </summary>
    /// <remarks>
    ///     Bij het registreren van de afdeling wordt een sequentie teruggegeven via de `VR-Sequence` header.
    ///     Deze waarde kan gebruikt worden in andere endpoints om op te volgen of de zonet geregistreerde afdeling
    ///     al is doorgestroomd naar deze endpoints.
    /// </remarks>
    /// <param name="request">De gegevens van de te registreren afdeling</param>
    /// <param name="metadataProvider"></param>
    /// <param name="bevestigingsToken">Dit token wordt gebruikt als bevestiging dat de afdeling uniek is,
    /// ondanks de voorgestelde duplicaten.</param>
    /// <response code="202">De afdeling is geregistreerd.</response>
    /// <response code="400">Er was een probleem met de doorgestuurde waarden.</response>
    /// <response code="409">Er zijn één of meerdere mogelijke duplicaten van deze vereniging gevonden.</response>
    /// <response code="500">Er is een interne fout opgetreden.</response>
    [HttpPost]
    [ConsumesJson]
    [ProducesJson]
    [SwaggerRequestExample(typeof(RegistreerAfdelingRequest), typeof(RegistreerAfdelingRequestExamples))]
    [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
    [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ProblemAndValidationProblemDetailsExamples))]
    [SwaggerResponseExample(StatusCodes.Status409Conflict, typeof(PotentialDuplicatesResponseExamples))]
    [SwaggerResponseHeader(StatusCodes.Status202Accepted, WellknownHeaderNames.Sequence, type: "string", description: "Het sequence nummer van deze request.")]
    [SwaggerResponseHeader(StatusCodes.Status202Accepted, name: "ETag", type: "string", description: "De versie van de geregistreerde vereniging.")]
    [SwaggerResponseHeader(StatusCodes.Status202Accepted, name: "Location", type: "string", description: "De locatie van de geregistreerde vereniging.")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(PotentialDuplicatesResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Post(
        [FromBody] RegistreerAfdelingRequest? request,
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
        var envelope = new CommandEnvelope<RegistreerAfdelingCommand>(command, metaData);
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
