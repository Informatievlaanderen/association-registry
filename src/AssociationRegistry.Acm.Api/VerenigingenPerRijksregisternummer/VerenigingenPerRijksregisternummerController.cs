namespace AssociationRegistry.Acm.Api.VerenigingenPerRijksregisternummer;

using System.Threading;
using Caches;
using System.Threading.Tasks;
using Be.Vlaanderen.Basisregisters.Api;
using Be.Vlaanderen.Basisregisters.Api.Exceptions;
using Examples;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[ApiRoute("verenigingen")]
[ApiExplorerSettings(GroupName = "Verenigingen")]
public class VerenigingenPerRijksregisternummerController : ApiController
{
    /// <summary>
    /// Vraag de lijst van verenigingen voor een rijksregisternummer op.
    /// </summary>
    /// <param name="verenigingenRepository"></param>
    /// <param name="rijksregisternummer"></param>
    /// <response code="200">Als het rijksregisternummer gevonden is.</response>
    /// <response code="500">Als er een interne fout is opgetreden.</response>
    [HttpGet]
    [ProducesResponseType(typeof(GetVerenigingenPerRijksregisternummerResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(GetVerenigingenResponseExamples))]
    [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
    public async Task<IActionResult> Get(
        [FromServices] IVerenigingenRepository verenigingenRepository,
        [FromQuery] string rijksregisternummer) =>
        await Task.FromResult<IActionResult>(Ok(new GetVerenigingenPerRijksregisternummerResponse(rijksregisternummer, verenigingenRepository.Verenigingen[rijksregisternummer])));

    [HttpPut]
    public async Task<IActionResult> Put(
        [FromServices] IVerenigingenRepository verenigingenRepository,
        [FromBody] VerenigingenAsDictionary? maybeBody,
        CancellationToken cancellationToken)
    {
        if (maybeBody is not { })
            return BadRequest();

        await verenigingenRepository.UpdateVerenigingen(maybeBody, Request.Body, cancellationToken);

        return Ok();
    }
}
