namespace AssociationRegistry.Admin.Api.WebApi.Administratie.AdresMatch;

using Asp.Versioning;
using AssociationRegistry.Admin.Schema.Detail;
using Be.Vlaanderen.Basisregisters.Api;
using CommandHandling.DecentraalBeheer.Acties.Locaties.ProbeerAdresTeMatchen;
using Marten;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// SuperAdmin endpoint to manually call.
/// This will try to match any location without an AdresId.
/// </summary>
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
        [FromServices] ProbeerAdresTeMatchenCommandHandler handler,
        [FromServices] ILogger<AdresMatchController> logger,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Start AdresMatchController");

        await using var session = documentStore.LightweightSession();

        var docs = await session.Query<LocatieZonderAdresMatchDocument>().ToListAsync(cancellationToken);

        var messages = docs
                      .SelectMany(s => s.LocatieIds.Select(x => (s.VCode, LocatieId: x)))
                      .Select(s => new ProbeerAdresTeMatchenCommand(s.VCode, s.LocatieId))
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

    [HttpGet("locaties/zonder-adresmatch")]
    public async Task<IActionResult> RetrieveAdressenWithoutAdresMatch(
        [FromServices] IDocumentStore documentStore,
        [FromServices] ILogger<AdresMatchController> logger,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Start retrieving locaties zonderadresmatch");

        await using var session = documentStore.LightweightSession();

        var docs = await session.Query<LocatieZonderAdresMatchDocument>()
                                .Where(x => x.LocatieIds.Length > 0)
                                .ToListAsync(cancellationToken);

        logger.LogInformation("Done retrieving locatieszonderadresmatch");
        return Ok(docs);
    }
}
