namespace AssociationRegistry.Public.Api.SearchVerenigingen;

using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Caches;
using Examples;
using Be.Vlaanderen.Basisregisters.Api;
using Be.Vlaanderen.Basisregisters.Api.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[ApiRoute("verenigingen/dep")]
[ApiExplorerSettings(GroupName = "Verenigingen")]
public class SearchVerenigingenController : ApiController
{
    /// <summary>
    /// Vraag de lijst van verenigingen voor een rijksregisternummer op.
    /// </summary>
    /// <response code="200">Er kwam geen fout voor.</response>
    /// <response code="500">Als er een interne fout is opgetreden.</response>
    [HttpGet]
    [ProducesResponseType(typeof(SearchVerenigingenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(SearchVerenigingenResponseExamples))]
    [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
    public async Task<IActionResult> Get([FromServices] IVerenigingenRepository verenigingenRepository) =>
        await Task.FromResult<IActionResult>(Ok(verenigingenRepository.Verenigingen));

    [HttpPut]
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
