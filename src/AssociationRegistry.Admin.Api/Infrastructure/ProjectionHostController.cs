namespace AssociationRegistry.Admin.Api.Infrastructure;

using Be.Vlaanderen.Basisregisters.Api;
using HttpClients;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

public class ProjectionHostController : ApiController
{
    private readonly AdminProjectionHostHttpClient _adminHttpClient;
    private readonly PublicProjectionHostHttpClient _publicHttpClient;

    public ProjectionHostController(AdminProjectionHostHttpClient adminHttpClient, PublicProjectionHostHttpClient publicHttpClient)
    {
        _adminHttpClient = adminHttpClient;
        _publicHttpClient = publicHttpClient;
    }

    [HttpPost("/projections/admin/all/rebuild")]
    public async Task<IActionResult> RebuildAdminProjectionAll(CancellationToken cancellationToken)
    {
        var response = await _adminHttpClient.RebuildAllProjections(cancellationToken);

        return response.IsSuccessStatusCode
            ? Ok()
            : UnprocessableEntity();
    }

    [HttpPost("/projections/admin/detail/rebuild")]
    public async Task<IActionResult> RebuildAdminProjectionDetail(CancellationToken cancellationToken)
    {
        var response = await _adminHttpClient.RebuildDetailProjection(cancellationToken);

        return response.IsSuccessStatusCode
            ? Ok()
            : UnprocessableEntity();
    }

    [HttpPost("/projections/admin/historiek/rebuild")]
    public async Task<IActionResult> RebuildAdminProjectionHistoriek(CancellationToken cancellationToken)
    {
        var response = await _adminHttpClient.RebuildHistoriekProjection(cancellationToken);

        return response.IsSuccessStatusCode
            ? Ok()
            : UnprocessableEntity();
    }

    [HttpPost("/projections/admin/search/rebuild")]
    public async Task<IActionResult> RebuildAdminProjectionZoeken(CancellationToken cancellationToken)
    {
        var response = await _adminHttpClient.RebuildZoekenProjection(cancellationToken);

        return response.IsSuccessStatusCode
            ? Ok()
            : UnprocessableEntity();
    }

    [HttpPost("/projections/admin/status")]
    public async Task<IActionResult> GetAdminProjectionStatus(CancellationToken cancellationToken)
    {
        var response = await _adminHttpClient.GetStatus(cancellationToken);

        if (!response.IsSuccessStatusCode) return BadRequest();

        return Ok();
    }

    [HttpPost("/projections/public/detail/rebuild")]
    public async Task<IActionResult> RebuildPublicProjectionDetail(CancellationToken cancellationToken)
    {
        var response = await _publicHttpClient.RebuildDetailProjection(cancellationToken);

        return response.IsSuccessStatusCode
            ? Ok()
            : UnprocessableEntity();
    }

    [HttpPost("/projections/public/search/rebuild")]
    public async Task<IActionResult> RebuildPublicProjectionZoeken(CancellationToken cancellationToken)
    {
        var response = await _publicHttpClient.RebuildZoekenProjection(cancellationToken);

        return response.IsSuccessStatusCode
            ? Ok()
            : UnprocessableEntity();
    }

    [HttpPost("/projections/public/status")]
    public async Task<IActionResult> GetPublicProjectionStatus(CancellationToken cancellationToken)
    {
        var response = await _publicHttpClient.GetStatus(cancellationToken);

        if (!response.IsSuccessStatusCode) return BadRequest();

        return Ok();
    }
}
