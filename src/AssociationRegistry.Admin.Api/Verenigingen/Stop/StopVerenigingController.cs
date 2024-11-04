namespace AssociationRegistry.Admin.Api.Verenigingen.Stop;

using Acties.StopVereniging;
using Asp.Versioning;
using Be.Vlaanderen.Basisregisters.Api;
using Be.Vlaanderen.Basisregisters.Api.Exceptions;
using Examples;
using FluentValidation;
using Framework;
using Hosts.Configuration.ConfigurationBindings;
using Infrastructure;
using Infrastructure.Extensions;
using Infrastructure.Middleware;
using Infrastructure.Swagger.Annotations;
using Infrastructure.Swagger.Examples;
using Infrastructure.Validation;
using Microsoft.AspNetCore.Mvc;
using RequestModels;
using Swashbuckle.AspNetCore.Filters;
using Vereniging;
using Wolverine;
using ProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ProblemDetails;
using ValidationProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ValidationProblemDetails;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[ApiRoute("verenigingen")]
[SwaggerGroup.DecentraalBeheer]
public class StopVerenigingController : ApiController
{
    private readonly IMessageBus _bus;
    private readonly AppSettings _appsettings;
    private readonly IValidator<StopVerenigingRequest> _validator;

    public StopVerenigingController(IMessageBus bus, AppSettings appsettings, IValidator<StopVerenigingRequest> validator)
    {
        _bus = bus;
        _appsettings = appsettings;
        _validator = validator;
    }

    /// <summary>
    ///     Stoppen van een vereniging.
    /// </summary>
    /// <remarks>
    ///     Wanneer er wijzigingen veroorzaakt zijn door de request, bevat de response een sequence header.
    ///
    ///     Na het uitvoeren van deze actie wordt een sequentie teruggegeven via de `VR-Sequence` header.
    ///     Deze waarde kan gebruikt worden in andere endpoints om op te volgen of de aanpassing
    ///     al is doorgestroomd naar deze endpoints.
    /// </remarks>
    /// <param name="request"></param>
    /// <param name="vCode">De vCode van de vereniging</param>
    /// <param name="metadataProvider"></param>
    /// <param name="ifMatch">If-Match header met ETag van de laatst gekende versie van de vereniging.</param>
    /// <response code="200">Er waren geen wijzigingen</response>
    /// <response code="202">De vereniging werd gestopt</response>
    /// <response code="400">Er was een probleem met de doorgestuurde waarden.</response>
    /// <response code="412">De gevraagde vereniging heeft niet de verwachte sequentiewaarde.</response>
    /// <response code="500">Er is een interne fout opgetreden.</response>
    [HttpPost("{vCode}/stop")]
    [ConsumesJson]
    [ProducesJson]
    [SwaggerRequestExample(typeof(StopVerenigingRequest), typeof(StopVerenigingRequestExamples))]
    [SwaggerResponseHeader(StatusCodes.Status202Accepted, WellknownHeaderNames.Sequence, type: "string",
                           description: "Het sequence nummer van deze request.")]
    [SwaggerResponseHeader(StatusCodes.Status202Accepted, name: "ETag", type: "string",
                           description: "De versie van de aangepaste vereniging.")]
    [SwaggerResponseHeader(StatusCodes.Status202Accepted, name: "Location", type: "string",
                           description: "De locatie van de aangepaste vereniging.")]
    [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ProblemAndValidationProblemDetailsExamples))]
    [SwaggerResponseExample(StatusCodes.Status412PreconditionFailed, typeof(PreconditionFailedProblemDetailsExamples))]
    [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status412PreconditionFailed)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Post(
        [FromBody] StopVerenigingRequest? request,
        [FromRoute] string vCode,
        [FromServices] ICommandMetadataProvider metadataProvider,
        [FromHeader(Name = "If-Match")] string? ifMatch = null)
    {
        await _validator.NullValidateAndThrowAsync(request);

        var command = request.ToCommand(vCode);

        var metaData = metadataProvider.GetMetadata(IfMatchParser.ParseIfMatch(ifMatch));
        var envelope = new CommandEnvelope<StopVerenigingCommand>(command, metaData);
        var stopResult = await _bus.InvokeAsync<CommandResult>(envelope);

        if (!stopResult.HasChanges()) return Ok();

        return this.AcceptedCommand(_appsettings, stopResult);
    }
}
