namespace AssociationRegistry.Admin.Api.Verenigingen.Stop;

using Acties.StopVereniging;
using Be.Vlaanderen.Basisregisters.Api;
using Be.Vlaanderen.Basisregisters.Api.Exceptions;
using Examples;
using FluentValidation;
using Framework;
using Infrastructure;
using Infrastructure.ConfigurationBindings;
using Infrastructure.Extensions;
using Infrastructure.Middleware;
using Infrastructure.Swagger;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RequestModels;
using Swashbuckle.AspNetCore.Filters;
using System.Threading.Tasks;
using Vereniging;
using Wolverine;

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

    [HttpPost("{vCode}/stoppen")]
    [Consumes("application/json")]
    [Produces("application/json")]
    [SwaggerRequestExample(typeof(StopVerenigingRequest), typeof(StopVerenigingRequestExamples))]
    [SwaggerResponseHeader(StatusCodes.Status202Accepted, WellknownHeaderNames.Sequence, type: "string", description: "Het sequence nummer van deze request.")]
    [SwaggerResponseHeader(StatusCodes.Status202Accepted, name: "ETag", type: "string", description: "De versie van de aangepaste vereniging.")]
    [SwaggerResponseHeader(StatusCodes.Status202Accepted, name: "Location", type: "string", description: "De locatie van de aangepaste vereniging.")]
    [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ProblemAndValidationProblemDetailsExamples))]
    [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Patch(
        [FromBody] StopVerenigingRequest? request,
        [FromRoute] string vCode,
        [FromServices] ICommandMetadataProvider metadataProvider,
        [FromHeader(Name = "If-Match")] string? ifMatch = null)
    {
        await _validator.NullValidateAndThrowAsync(request);

        var command = request.ToCommand(vCode);

        var metaData = metadataProvider.GetMetadata(IfMatchParser.ParseIfMatch(ifMatch));
        var envelope = new CommandEnvelope<StopVerenigingCommand>(command, metaData);
        var wijzigResult = await _bus.InvokeAsync<CommandResult>(envelope);

        if (!wijzigResult.HasChanges()) return Ok();

        return this.AcceptedCommand(_appsettings, wijzigResult);
    }
}
