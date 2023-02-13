namespace AssociationRegistry.Acm.Api.VerenigingenPerInsz;

using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AssociationRegistry.Acm.Api.Caches;
using Be.Vlaanderen.Basisregisters.Api;
using Be.Vlaanderen.Basisregisters.Api.Exceptions;
using Examples;
using Marten;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Schema.VerenigingenPerInsz;
using Swashbuckle.AspNetCore.Filters;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[ApiRoute("verenigingen")]
[ApiExplorerSettings(GroupName = "Verenigingen")]
public class VerenigingenPerInszController : ApiController
{
    /// <summary>
    /// Vraag de lijst van verenigingen voor een rijksregisternummer op.
    /// </summary>
    /// <param name="verenigingenRepository"></param>
    /// <param name="insz"></param>
    /// <response code="200">Als het rijksregisternummer gevonden is.</response>
    /// <response code="500">Als er een interne fout is opgetreden.</response>
    [HttpGet]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ProducesResponseType(typeof(VerenigingenPerInszDocument), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(VerenigingenResponseExamples))]
    [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
    public async Task<IActionResult> Get(
        [FromServices] IDocumentStore documentStore,
        [FromQuery] string insz)
    {
        await using var session = documentStore.LightweightSession();
        var document = await session.Query<VerenigingenPerInszDocument>()
            .Where(x => x.Insz.Equals(insz, StringComparison.CurrentCultureIgnoreCase))
            .SingleOrDefaultAsync();

        if (document is null)
            return Ok(new VerenigingenPerInszDocument { Insz = insz });

        return Ok(document);
    }

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
