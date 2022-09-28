namespace AssociationRegistry.Public.Api.SearchVerenigingen;

using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Caches;
using Examples;
using Be.Vlaanderen.Basisregisters.Api;
using Be.Vlaanderen.Basisregisters.Api.Exceptions;
using Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[ApiRoute("verenigingen/search")]
[ApiExplorerSettings(GroupName = "Verenigingen")]
public class SearchVerenigingenController : ApiController
{
    /// <summary>
    /// Zoek verenigingen op. (statische dataset)
    /// </summary>
    /// <response code="200">Indien de zoekopdracht succesvol was.</response>
    /// <response code="500">Als er een interne fout is opgetreden.</response>
    [HttpGet]
    [ProducesResponseType(typeof(SearchVerenigingenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(SearchVerenigingenResponseExamples))]
    [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
    [Produces(contentType: WellknownMediaTypes.Json)]
    public async Task<IActionResult> Get([FromServices] IVerenigingenRepository verenigingenRepository) =>
        await Task.FromResult<IActionResult>(Ok(verenigingenRepository.Verenigingen));

    [HttpPut]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> Put(
        [FromServices] IVerenigingenRepository verenigingenRepository,
        [FromBody] ImmutableArray<Vereniging>? maybeBody,
        CancellationToken cancellationToken)
    {
        if (maybeBody is not { }body)
            return BadRequest();

        await verenigingenRepository.UpdateVerenigingen(body, Request.Body, cancellationToken);
        return Ok();
    }
}
