namespace AssociationRegistry.Admin.Api.Verenigingen.Registreer;

using System;
using System.Linq;
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
using Microsoft.AspNetCore.Mvc.ModelBinding;
using NodaTime;
using ResultNet;
using Swashbuckle.AspNetCore.Filters;
using Vereniging.DuplicateDetection;
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
    private readonly AppSettings _appSettings;

    public RegistreerVerenigingController(IMessageBus bus, AppSettings appSettings)
    {
        _bus = bus;
        _appSettings = appSettings;
    }

    /// <summary>
    /// Registreer een vereniging.
    /// </summary>
    /// <remarks>
    /// Bij het registreren van de vereniging wordt een sequentie teruggegeven via de `VR-Sequence` header.
    /// Deze waarde kan gebruikt worden in andere endpoints om op te volgen of de zonet geregistreerde vereniging
    /// al is doorgestroomd naar deze endpoints.
    /// </remarks>
    /// <response code="202">De vereniging is aangemaakt.</response>
    /// <response code="400">Er is een probleem met de doorgestuurde waarden. Zie body voor meet info.</response>
    /// <response code="500">Als er een interne fout is opgetreden.</response>
    [HttpPost]
    [Consumes("application/json")]
    [Produces("application/json")]
    [SwaggerRequestExample(typeof(RegistreerVerenigingRequest), typeof(RegistreerVerenigingenRequestExamples))]
    [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
    [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ValidationProblemDetailsExamples))]
    [SwaggerResponseHeader(StatusCodes.Status202Accepted, WellknownHeaderNames.Sequence, "string", "Het sequence nummer van deze request.")]
    [SwaggerResponseHeader(StatusCodes.Status202Accepted, "ETag", "string", "De versie van de aangemaakte vereniging.")]
    [SwaggerResponseHeader(StatusCodes.Status202Accepted, "Location", "string", "De locatie van de aangemaakte vereniging.")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Post(
        [FromServices] IValidator<RegistreerVerenigingRequest> validator,
        [FromBody] RegistreerVerenigingRequest? request)
    {
        await validator.NullValidateAndThrowAsync(request);

        var command = request.ToRegistreerVerenigingCommand();

        var metaData = new CommandMetadata(request.Initiator, SystemClock.Instance.GetCurrentInstant());
        var envelope = new CommandEnvelope<RegistreerVerenigingCommand>(command, metaData);
        var registratieResult = await _bus.InvokeAsync<Result>(envelope);

        return registratieResult switch
        {
            Result<CommandResult> commandResult => this.AcceptedCommand(_appSettings, commandResult.Data),
            Result<PotentialDuplicatesFound> potentialDuplicates => Conflict(new PotentialDuplicatesResponse(potentialDuplicates.Data)),
            _ => throw new ArgumentOutOfRangeException(),
        };
    }
}

public class PotentialDuplicatesResponse
{
    public DuplicateCandidateResponse[] Duplicaten { get; set; }

    public PotentialDuplicatesResponse(PotentialDuplicatesFound potentialDuplicates)
    {
        Duplicaten = potentialDuplicates.Candidates.Select(DuplicateCandidateResponse.FromDuplicateCandidate).ToArray();
    }

    public class DuplicateCandidateResponse
    {
        public string VCode { get; set; } = null!;
        public string Naam { get; set; } = null!;

        public static DuplicateCandidateResponse FromDuplicateCandidate(DuplicateCandidate candidate)
            => new()
            {
                VCode = candidate.VCode,
                Naam = candidate.Naam,
            };
    }
}
