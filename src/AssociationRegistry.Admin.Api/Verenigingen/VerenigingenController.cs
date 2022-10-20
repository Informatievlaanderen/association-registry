namespace AssociationRegistry.Admin.Api.Verenigingen;

using System.Net;
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
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [SwaggerRequestExample(typeof(CreateVerenigingCommand), typeof(CreateVerenigingenRequestExamples))]
    [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    public async Task<IActionResult> Post([FromBody] CreateVerenigingCommand command)
    {
        var envelope = new CommandEnvelope<CreateVerenigingCommand>(command);
        await _sender.Send(envelope);
        return Accepted();
    }
}
