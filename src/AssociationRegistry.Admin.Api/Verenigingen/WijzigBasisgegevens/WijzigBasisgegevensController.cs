namespace AssociationRegistry.Admin.Api.Verenigingen.WijzigBasisgegevens;

using System.Threading.Tasks;
using AssociationRegistry.Admin.Api.Infrastructure;
using AssociationRegistry.Admin.Api.Infrastructure.ConfigurationBindings;
using Be.Vlaanderen.Basisregisters.Api;
using FluentValidation;
using Framework;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NodaTime;
using Swashbuckle.AspNetCore.Filters;
using Vereniging;
using Vereniging.RegistreerVereniging;
using Vereniging.WijzigBasisgegevens;
using Wolverine;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[ApiRoute("verenigingen/{vCode}")]
[ApiExplorerSettings(GroupName = "Verenigingen")]
public class WijzigBasisgegevensController : ApiController
{
    private readonly IMessageBus _bus;
    private readonly AppSettings _appSettings;

    public WijzigBasisgegevensController(IMessageBus bus, AppSettings appSettings)
    {
        _bus = bus;
        _appSettings = appSettings;
    }

    /// <summary>
    /// Wijzig de basisgegevens van een vereniging.
    /// </summary>
    [HttpPatch]
    [Consumes("application/json")]
    [Produces("application/json")]
    // [SwaggerRequestExample(typeof(RegistreerVerenigingRequest), typeof(RegistreerVerenigingenRequestExamples))]
    // [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
    [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ValidationProblemDetailsExamples))]
    // [ProducesResponseType(StatusCodes.Status202Accepted)]
    // [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    // [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Patch(
        [FromServices] WijzigBasisgegevensRequestValidator validator,
        [FromBody] WijzigBasisgegevensRequest? request,
        [FromRoute] string vCode)
    {
        if (request is null) return BadRequest();
        await validator.ValidateAndThrowAsync(request);

        var command = request.ToWijzigBasisgegevensCommand(vCode);

        var metaData = new CommandMetadata(request.Initiator, SystemClock.Instance.GetCurrentInstant());
        var envelope = new CommandEnvelope<WijzigBasisgegevensCommand>(command, metaData);
        var registratieResult = await _bus.InvokeAsync<RegistratieResult>(envelope);

        return Accepted();
    }
}
