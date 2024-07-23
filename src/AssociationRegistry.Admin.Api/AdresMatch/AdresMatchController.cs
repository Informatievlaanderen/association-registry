namespace AssociationRegistry.Admin.Api.AdresMatch;

using Asp.Versioning;
using Be.Vlaanderen.Basisregisters.Api;
using Grar.AddressMatch;
using Marten;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Schema.Detail;

[ApiVersion("1.0")]

[AdvertiseApiVersions("1.0")]
[ApiRoute("admin")]
[ApiExplorerSettings(IgnoreApi = true)]
[Authorize(Policy = Program.SuperAdminPolicyName)]
public class AdresMatchController : ApiController
{
    [HttpPost("adresmatch")]
    public async Task<IActionResult> QueueAdressenForAdresMatch(
        [FromServices] IDocumentStore documentStore,
        [FromServices] ITeAdresMatchenLocatieMessageHandler handler,
        CancellationToken cancellationToken)
    {
        await using var session = documentStore.LightweightSession();

        var docs = await session.Query<LocatieZonderAdresMatchDocument>().ToListAsync(cancellationToken);
        var messages = docs.Select(s => new TeAdresMatchenLocatieMessage(s.VCode, s.LocatieId));

        foreach (var message in messages)
        {
            await handler.Handle(message, cancellationToken);
        }

        return Ok();
    }
}
