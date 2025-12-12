namespace AssociationRegistry.Admin.Api.WebApi.Administratie.VertegenwoordigersPerVCode;

using Asp.Versioning;
using AssociationRegistry.Admin.Api.Queries;
using Be.Vlaanderen.Basisregisters.Api;
using Bewaartermijnen;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NodaTime;

/// <summary>
/// SuperAdmin endpoint to manually call.
/// This will try to match any location without an AdresId.
/// </summary>
[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[ApiRoute("admin")]
[ApiExplorerSettings(IgnoreApi = true)]
[Authorize(Policy = Program.SuperAdminPolicyName)]
public class VertegenwoordigersPerVCodeController : ApiController
{
    [HttpGet("vertegenwoordigers")]
    public async Task<IActionResult> Get(
        [FromQuery] string vCode,
        [FromQuery] string status,
        [FromServices] IVertegenwoordigersPerVCodeQuery query,
        [FromServices] ILogger<VertegenwoordigersPerVCodeController> logger,
        CancellationToken cancellationToken)
    {
        var vertegenwoordigersPerVCodeDocuments = await query.ExecuteAsync(new VertegenwoordigersPerVCodeQueryFilter(vCode, status), cancellationToken);

        var responses = VertegenwoordigerResponseMapper.Map(vertegenwoordigersPerVCodeDocuments, status);

        return Ok(responses);
    }
}

public record VertegenwoordigerResponse(string VCode, int VertegenwoordigerId, string Status);
