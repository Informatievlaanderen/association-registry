namespace AssociationRegistry.Admin.Api.Verenigingen;

using System.Threading.Tasks;
using Be.Vlaanderen.Basisregisters.Api;
using MediatR;
using Microsoft.AspNetCore.Mvc;

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

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CreateVerenigingCommand command)
    {
        var envelope = new CommandEnvelope<CreateVerenigingCommand>(command);
        await _sender.Send(envelope);
        return Accepted();
    }
}
