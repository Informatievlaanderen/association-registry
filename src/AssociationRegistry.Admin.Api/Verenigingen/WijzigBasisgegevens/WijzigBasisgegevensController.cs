namespace AssociationRegistry.Admin.Api.Verenigingen.WijzigBasisgegevens;

using System.Threading.Tasks;
using Infrastructure;
using Be.Vlaanderen.Basisregisters.Api;
using Be.Vlaanderen.Basisregisters.Api.Exceptions;
using FluentValidation;
using Framework;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NodaTime;
using Registreer;
using Swashbuckle.AspNetCore.Filters;
using Vereniging;
using Vereniging.WijzigBasisgegevens;
using Wolverine;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[ApiRoute("verenigingen/{vCode}")]
[ApiExplorerSettings(GroupName = "Verenigingen")]
public class WijzigBasisgegevensController : ApiController
{
    private readonly IMessageBus _bus;

    public WijzigBasisgegevensController(IMessageBus bus)
    {
        _bus = bus;
    }

    /// <summary>
    /// Wijzig de basisgegevens van een vereniging.
    /// </summary>
    /// <remarks>
    /// Enkel velden die worden doorgestuurd in de request worden verwerkt. Null waarden worden niet verwerkt.
    /// </remarks>
    /// <param name="vCode">De VCode van de vereniging</param>
    [HttpPatch]
    [Consumes("application/json")]
    [Produces("application/json")]
    [SwaggerRequestExample(typeof(WijzigBasisgegevensRequest), typeof(WijzigBasisgegevensRequestExamples))]
    [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
    [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ValidationProblemDetailsExamples))]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Patch(
        [FromServices] WijzigBasisgegevensRequestValidator validator,
        [FromBody] WijzigBasisgegevensRequest? request,
        [FromRoute] string vCode)
    {
        if (request is null) return BadRequest();
        await DefaultValidatorExtensions.ValidateAndThrowAsync(validator, request);

        var command = request.ToWijzigBasisgegevensCommand(vCode);

        var metaData = new CommandMetadata(request.Initiator, SystemClock.Instance.GetCurrentInstant());
        var envelope = new CommandEnvelope<WijzigBasisgegevensCommand>(command, metaData);
        var registratieResult = await _bus.InvokeAsync<RegistratieResult>(envelope);

        return Accepted();
    }
}
