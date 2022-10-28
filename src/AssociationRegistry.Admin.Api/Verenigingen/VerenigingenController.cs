namespace AssociationRegistry.Admin.Api.Verenigingen;

using System.Threading.Tasks;
using Be.Vlaanderen.Basisregisters.Api;
using Be.Vlaanderen.Basisregisters.Api.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[ApiRoute("verenigingen")]
[ApiExplorerSettings(GroupName = "Verenigingen")]
public class VerenigingenController : ApiController
{
    private readonly ISender _sender;

    public VerenigingenController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Registreer een vereniging.
    /// </summary>
    [HttpPost]
    [Consumes("application/json")]
    [Produces("application/json")]
    [SwaggerRequestExample(typeof(RegistreerVerenigingRequest), typeof(RegistreerVerenigingenRequestExamples))]
    [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
    [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ValidationProblemDetails))]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Post([FromBody] RegistreerVerenigingRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var command = new RegistreerVerenigingCommand(request.Naam);

        var envelope = new CommandEnvelope<RegistreerVerenigingCommand>(command);
        await _sender.Send(envelope);
        return Accepted();
    }
}
