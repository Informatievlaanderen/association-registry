namespace AssociationRegistry.Admin.Api.WebApi.Administratie.ProjectieBeheer;

using System.Text.Json;
using Asp.Versioning;
using AssociationRegistry.Admin.Api.Infrastructure;
using AssociationRegistry.Admin.Api.Infrastructure.HttpClients;
using Be.Vlaanderen.Basisregisters.Api;
using CommandHandling.Bewaartermijnen.EventHandling;
using EventSubscriptions.Rebuilds;
using Marten;
using Marten.Events.Daemon.Coordination;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResponseModels;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[ApiRoute("projections")]
[ApiExplorerSettings(IgnoreApi = true)]
[Authorize(Policy = Program.SuperAdminPolicyName)]
public class ProjectionController : ApiController
{
    private readonly AdminProjectionHostHttpClient _adminHttpClient;
    private readonly PublicProjectionHostHttpClient _publicHttpClient;
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    //private readonly IProjectionCoordinator _projectionCoordinator;
    private readonly ILogger<ProjectionController> _logger;

    public ProjectionController(
        AdminProjectionHostHttpClient adminHttpClient,
        PublicProjectionHostHttpClient publicHttpClient,
        //IProjectionCoordinator projectionCoordinator,
        ILogger<ProjectionController> logger
    )
    {
        _adminHttpClient = adminHttpClient;
        _publicHttpClient = publicHttpClient;
        //_projectionCoordinator = projectionCoordinator;
        _logger = logger;

        _jsonSerializerOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
    }

    [HttpPost("admin/all/rebuild")]
    public async Task<IActionResult> RebuildAdminProjectionAll(CancellationToken cancellationToken)
    {
        var response = await _adminHttpClient.RebuildAllProjections(cancellationToken);

        return await OkOrForwardedResponse(cancellationToken, response);
    }

    [HttpPost("admin/detail/rebuild")]
    public async Task<IActionResult> RebuildAdminProjectionDetail(CancellationToken cancellationToken)
    {
        var response = await _adminHttpClient.RebuildDetailProjection(cancellationToken);

        return await OkOrForwardedResponse(cancellationToken, response);
    }

    // [HttpPost("admin/eventsubscription/bewaartermijn/vertegenwoordigers/rebuild")]
    // public async Task<IActionResult> RebuildEventSubscriptionBewaartermijnVertegenwoordigers(
    //     CancellationToken cancellationToken,
    //     [FromQuery] long sequence = 0
    // )
    // {
    //     await EventSubscriptionRebuildExtensions.StartRebuildSubscription(
    //         BewaartermijnVertegenwoordigersEventHandler.ShardName.Name,
    //         _projectionCoordinator,
    //         sequence
    //     );
    //
    //     return Ok();
    // }

    [HttpPost("admin/bewaartermijn/rebuild")]
    public async Task<IActionResult> RebuildBewaartermijnen(
        CancellationToken cancellationToken,
        [FromQuery] long sequence = 0
    )
    {
        var response = await _adminHttpClient.RebuildBewaartermijnProjection(cancellationToken);

        return await OkOrForwardedResponse(cancellationToken, response);
    }

    [HttpPost("admin/locaties/gekoppeldmetgrar/rebuild")]
    public async Task<IActionResult> RebuildAdminProjectionLocatieLookup(CancellationToken cancellationToken)
    {
        var response = await _adminHttpClient.RebuildLocatieGekoppeldMetGrarProjection(cancellationToken);

        return await OkOrForwardedResponse(cancellationToken, response);
    }

    [HttpPost("admin/locaties/zonderadresmatch/rebuild")]
    public async Task<IActionResult> RebuildAdminProjectionLocatieZonderAdresMatch(CancellationToken cancellationToken)
    {
        var response = await _adminHttpClient.RebuildLocatieZonderAdresMatchProjection(cancellationToken);

        return await OkOrForwardedResponse(cancellationToken, response);
    }

    [HttpPost("admin/powerbi/rebuild")]
    public async Task<IActionResult> RebuildAdminProjectionPowerBiExport(CancellationToken cancellationToken)
    {
        var response = await _adminHttpClient.RebuildPowerBiExportProjection(cancellationToken);

        return await OkOrForwardedResponse(cancellationToken, response);
    }

    [HttpPost("admin/powerbi-dubbeldetectie/rebuild")]
    public async Task<IActionResult> RebuildAdminProjectionPowerBiDubbelDetectieExport(
        CancellationToken cancellationToken
    )
    {
        var response = await _adminHttpClient.RebuildPowerBiDubbelDetectieExportProjection(cancellationToken);

        return await OkOrForwardedResponse(cancellationToken, response);
    }

    [HttpPost("admin/historiek/rebuild")]
    public async Task<IActionResult> RebuildAdminProjectionHistoriek(CancellationToken cancellationToken)
    {
        var response = await _adminHttpClient.RebuildHistoriekProjection(cancellationToken);

        return await OkOrForwardedResponse(cancellationToken, response);
    }

    [HttpPost("admin/search/rebuild")]
    public async Task<IActionResult> RebuildAdminProjectionZoeken(CancellationToken cancellationToken)
    {
        var response = await _adminHttpClient.RebuildZoekenProjection(cancellationToken);

        return await OkOrForwardedResponse(cancellationToken, response);
    }

    [HttpPost("admin/duplicatedetection/rebuild")]
    public async Task<IActionResult> RebuildAdminDuplicateDetectionProjectionZoeken(CancellationToken cancellationToken)
    {
        var response = await _adminHttpClient.RebuildDuplicateDetectionProjection(cancellationToken);

        return await OkOrForwardedResponse(cancellationToken, response);
    }

    [HttpPost("admin/kbo-sync-historiek/rebuild")]
    public async Task<IActionResult> RebuildAdminKboSyncProjection(CancellationToken cancellationToken)
    {
        var response = await _adminHttpClient.RebuildAdminKboSyncProjection(cancellationToken);

        return await OkOrForwardedResponse(cancellationToken, response);
    }

    [HttpPost("admin/vertegenwoordigers/rebuild")]
    public async Task<IActionResult> RebuildAdminVertegenwoordigersProjection(CancellationToken cancellationToken)
    {
        var response = await _adminHttpClient.RebuildVertegenwoordigersProjection(cancellationToken);

        return await OkOrForwardedResponse(cancellationToken, response);
    }

    [HttpPost("admin/ksz-sync-historiek/rebuild")]
    public async Task<IActionResult> RebuildAdminKszSyncHistorieProjection(CancellationToken cancellationToken)
    {
        var response = await _adminHttpClient.RebuildKszSyncHistoriekProjection(cancellationToken);

        return await OkOrForwardedResponse(cancellationToken, response);
    }

    [HttpGet("admin/status")]
    public async Task<IActionResult> GetAdminProjectionStatus(CancellationToken cancellationToken)
    {
        var response = await _adminHttpClient.GetStatus(cancellationToken);

        return await OkObjectOrForwardedResponse(cancellationToken, response);
    }

    [HttpPost("public/all/rebuild")]
    public async Task<IActionResult> RebuildPublicAllProjections(CancellationToken cancellationToken)
    {
        var response = await _publicHttpClient.RebuildAllProjections(cancellationToken);

        return await OkOrForwardedResponse(cancellationToken, response);
    }

    [HttpPost("public/detail/rebuild")]
    public async Task<IActionResult> RebuildPublicProjectionDetail(CancellationToken cancellationToken)
    {
        var response = await _publicHttpClient.RebuildDetailProjection(cancellationToken);

        return await OkOrForwardedResponse(cancellationToken, response);
    }

    [HttpPost("public/search/rebuild")]
    public async Task<IActionResult> RebuildPublicProjectionZoeken(CancellationToken cancellationToken)
    {
        var response = await _publicHttpClient.RebuildZoekenProjection(cancellationToken);

        return await OkOrForwardedResponse(cancellationToken, response);
    }

    [HttpPost("public/sequence/rebuild")]
    public async Task<IActionResult> RebuildPublicProjectionSequence(CancellationToken cancellationToken)
    {
        var response = await _publicHttpClient.RebuildSequenceProjection(cancellationToken);

        return await OkOrForwardedResponse(cancellationToken, response);
    }

    [HttpGet("public/status")]
    public async Task<IActionResult> GetPublicProjectionStatus(CancellationToken cancellationToken)
    {
        var response = await _publicHttpClient.GetStatus(cancellationToken);

        return await OkObjectOrForwardedResponse(cancellationToken, response);
    }

    private async Task<IActionResult> OkOrForwardedResponse(
        CancellationToken cancellationToken,
        HttpResponseMessage response
    )
    {
        if (response.IsSuccessStatusCode)
            return Ok();

        return Problem(
            title: response.ReasonPhrase,
            statusCode: (int)response.StatusCode,
            detail: await response.Content.ReadAsStringAsync(cancellationToken)
        );
    }

    private async Task<IActionResult> OkObjectOrForwardedResponse(
        CancellationToken cancellationToken,
        HttpResponseMessage response
    )
    {
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<ProjectionStatus[]>(
                _jsonSerializerOptions,
                cancellationToken
            );

            return result is not null && result.Length > 0
                ? new OkObjectResult(new MinimalProjectionStatusResponse(result))
                : new EmptyResult();
        }

        return Problem(
            title: response.ReasonPhrase,
            statusCode: (int)response.StatusCode,
            detail: await response.Content.ReadAsStringAsync(cancellationToken)
        );
    }
}
