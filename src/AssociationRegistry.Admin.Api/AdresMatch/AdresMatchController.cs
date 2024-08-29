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
        [FromServices] TeAdresMatchenLocatieMessageHandler handler,
        [FromServices] ILogger<AdresMatchController> logger,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Start AdresMatchController");

        await using var session = documentStore.LightweightSession();

        var docs = await session.Query<LocatieZonderAdresMatchDocument>().ToListAsync(cancellationToken);

        var messages = docs
                      .Select(s => new TeAdresMatchenLocatieMessage(s.VCode, s.LocatieId))
                      .ToList();

        var succeededMessages = 0;
        var failedMessages = 0;

        foreach (var message in messages)
        {
            try
            {
                await handler.Handle(message, CancellationToken.None);

                logger.LogInformation($"Te adresmatchen vCode:{message.VCode} met locatie: {message.LocatieId} is succesvol verwerkt");

                succeededMessages++;
            }
            catch (Exception e)
            {
                logger.LogError(
                    e, $"Te adresmatchen vCode:{message.VCode} met locatie: {message.LocatieId} is gefaald wegens: {e.Message}");

                failedMessages++;
            }
        }

        logger.LogInformation(
            $"Aantal verwerkte locaties:{succeededMessages}. Aantal gefaalde locaties: {failedMessages}. Totaal aantal berichten: {messages.Count}");

        return Ok(
            $"Aantal verwerkte locaties:{succeededMessages}. Aantal gefaalde locaties: {failedMessages}. Totaal aantal berichten: {messages.Count}");
    }
}
