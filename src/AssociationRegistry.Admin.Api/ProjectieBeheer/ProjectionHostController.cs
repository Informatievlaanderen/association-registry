namespace AssociationRegistry.Admin.Api.ProjectieBeheer;

using Asp.Versioning;
using Be.Vlaanderen.Basisregisters.Api;
using Infrastructure;
using Infrastructure.HttpClients;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResponseModels;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

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

    public ProjectionController(AdminProjectionHostHttpClient adminHttpClient, PublicProjectionHostHttpClient publicHttpClient)
    {
        _adminHttpClient = adminHttpClient;
        _publicHttpClient = publicHttpClient;

        _jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };
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

    [HttpPost("admin/locaties/lookup/rebuild")]
    public async Task<IActionResult> RebuildAdminProjectionLocatieDetail(CancellationToken cancellationToken)
    {
        var response = await _adminHttpClient.RebuildLocatieLookupProjection(cancellationToken);

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

    private async Task<IActionResult> OkOrForwardedResponse(CancellationToken cancellationToken, HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode) return Ok();

        return Problem(
            title: response.ReasonPhrase,
            statusCode: (int)response.StatusCode,
            detail: await response.Content.ReadAsStringAsync(cancellationToken)
        );
    }

    private async Task<IActionResult> OkObjectOrForwardedResponse(CancellationToken cancellationToken, HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<ProjectionStatus[]>(_jsonSerializerOptions, cancellationToken);

            return result is not null
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
