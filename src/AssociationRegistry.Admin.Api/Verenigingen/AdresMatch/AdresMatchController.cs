namespace AssociationRegistry.Admin.Api.Verenigingen.AdresMatch;

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
        [FromServices] TeAdresMatchenLocatieMessageHandler handler,
        CancellationToken cancellationToken)
    {
        await using var session = documentStore.LightweightSession();

        var messages = session.Query<BeheerVerenigingDetailDocument>()
                              .Where(w => w.Locaties.Any(a => a.AdresId == null && a.Adres != null))
                              .ToList()
                              .SelectMany(s => s.Locaties.Where(a => a.AdresId == null && a.Adres != null)
                                                .Select(y => (s.VCode, y.LocatieId)));

        foreach (var message in messages)
        {
            await handler.Handle(new TeAdresMatchenLocatieMessage(message.VCode, message.LocatieId), cancellationToken);
        }

        return Ok();
    }
}
