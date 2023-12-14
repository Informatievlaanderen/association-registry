namespace AssociationRegistry.Admin.Api.Infrastructure;

using Be.Vlaanderen.Basisregisters.Api;
using HttpClients;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[ApiRoute("projections")]
[ApiExplorerSettings(IgnoreApi = true)]
public class ProjectionHostController : ApiController
{
    private readonly AdminProjectionHostHttpClient _adminHttpClient;
    private readonly PublicProjectionHostHttpClient _publicHttpClient;
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public ProjectionHostController(AdminProjectionHostHttpClient adminHttpClient, PublicProjectionHostHttpClient publicHttpClient)
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

        return response.IsSuccessStatusCode
            ? Ok()
            : UnprocessableEntity();
    }

    [HttpPost("admin/detail/rebuild")]
    public async Task<IActionResult> RebuildAdminProjectionDetail(CancellationToken cancellationToken)
    {
        var response = await _adminHttpClient.RebuildDetailProjection(cancellationToken);

        return response.IsSuccessStatusCode
            ? Ok()
            : UnprocessableEntity();
    }

    [HttpPost("admin/historiek/rebuild")]
    public async Task<IActionResult> RebuildAdminProjectionHistoriek(CancellationToken cancellationToken)
    {
        var response = await _adminHttpClient.RebuildHistoriekProjection(cancellationToken);

        return response.IsSuccessStatusCode
            ? Ok()
            : UnprocessableEntity();
    }

    [HttpPost("admin/search/rebuild")]
    public async Task<IActionResult> RebuildAdminProjectionZoeken(CancellationToken cancellationToken)
    {
        var response = await _adminHttpClient.RebuildZoekenProjection(cancellationToken);

        return response.IsSuccessStatusCode
            ? Ok()
            : UnprocessableEntity();
    }

    [HttpGet("admin/status")]
    public async Task<IActionResult> GetAdminProjectionStatus(CancellationToken cancellationToken)
    {
        var response = await _adminHttpClient.GetStatus(cancellationToken);

        if (!response.IsSuccessStatusCode) return BadRequest();

        var projectionProgress = await response.Content.ReadFromJsonAsync<ProjectionStatus[]>(_jsonSerializerOptions, cancellationToken);

        return new OkObjectResult(projectionProgress);
    }

    [HttpPost("public/detail/rebuild")]
    public async Task<IActionResult> RebuildPublicProjectionDetail(CancellationToken cancellationToken)
    {
        var response = await _publicHttpClient.RebuildDetailProjection(cancellationToken);

        return response.IsSuccessStatusCode
            ? Ok()
            : UnprocessableEntity();
    }

    [HttpPost("public/search/rebuild")]
    public async Task<IActionResult> RebuildPublicProjectionZoeken(CancellationToken cancellationToken)
    {
        var response = await _publicHttpClient.RebuildZoekenProjection(cancellationToken);

        return response.IsSuccessStatusCode
            ? Ok()
            : UnprocessableEntity();
    }

    [HttpGet("public/status")]
    public async Task<IActionResult> GetPublicProjectionStatus(CancellationToken cancellationToken)
    {
        var response = await _publicHttpClient.GetStatus(cancellationToken);

        if (!response.IsSuccessStatusCode) return BadRequest();

        var projectionProgress = await response.Content.ReadFromJsonAsync<ProjectionStatus[]>(_jsonSerializerOptions, cancellationToken);

        return new OkObjectResult(projectionProgress);
    }
}
