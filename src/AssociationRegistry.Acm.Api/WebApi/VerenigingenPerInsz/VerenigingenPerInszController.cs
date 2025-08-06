namespace AssociationRegistry.Acm.Api.WebApi.VerenigingenPerInsz;

using Asp.Versioning;
using AssociationRegistry.Acm.Api.Queries.VerenigingenPerInsz;
using AssociationRegistry.Acm.Api.Queries.VerenigingenPerKbo;
using AssociationRegistry.Acm.Schema.VerenigingenPerInsz;
using Be.Vlaanderen.Basisregisters.Api;
using Be.Vlaanderen.Basisregisters.Api.Exceptions;
using Marten;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.Linq;
using System.Threading.Tasks;
using ProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ProblemDetails;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[ApiRoute("verenigingen")]
[ApiExplorerSettings(GroupName = "Decentraal beheer van verenigingen")]
public class VerenigingenPerInszController : ApiController
{
    /// <summary>
    ///     Vraag de lijst van verenigingen voor een INSZ op.
    /// </summary>
    /// <param name="verenigingenPerInszQuery"></param>
    /// <param name="kboNummerService"></param>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <response code="200">Als het INSZ gevonden is.</response>
    /// <response code="500">Er is een interne fout opgetreden.</response>
    [HttpPost]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ProducesResponseType(typeof(VerenigingenPerInszResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(VerenigingenPerInszResponseExamples))]
    [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
    public async Task<IActionResult> Post(
        [FromServices] IVerenigingenPerInszQuery verenigingenPerInszQuery,
        [FromServices] IVerenigingenPerKboNummerService kboNummerService,
        [FromBody] VerenigingenPerInszRequest request,
        CancellationToken cancellationToken)
    {
        var validator = new VerenigingenPerInszRequestValidator();
        await validator.ValidateAndThrowAsync(request, cancellationToken: cancellationToken);

        var verenigingenPerInsz =
            await verenigingenPerInszQuery.ExecuteAsync(new VerenigingenPerInszFilter(request.Insz), cancellationToken);

        var kboNummersMetRechtsvorm = request.KboNummers
                                             .Select(s => new KboNummerMetRechtsvorm(s.KboNummer, s.Rechtsvorm))
                                             .ToArray();

        var verenigingenPerKbo = await kboNummerService.GetVerenigingenPerKbo(kboNummersMetRechtsvorm, cancellationToken);

        return Ok(VerenigingPerInszMapper.ToResponse(verenigingenPerInsz, verenigingenPerKbo));
    }

    /// <summary>
    ///     Vraag de lijst van verenigingen voor een INSZ op.
    /// </summary>
    /// <param name="documentStore"></param>
    /// <param name="insz">Dit is de unieke identificatie van een persoon, dit kan een rijksregisternummer of bisnummer zijn</param>
    /// <response code="200">Als het INSZ gevonden is.</response>
    /// <response code="500">Er is een interne fout opgetreden.</response>
    [HttpGet]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ProducesResponseType(typeof(VerenigingenPerInszResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(VerenigingenPerInszResponseExamples))]
    [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
    public async Task<IActionResult> Get(
        [FromServices] IDocumentStore documentStore,
        [FromQuery] string insz)
    {
        await using var session = documentStore.LightweightSession();

        var verenigingenPerInsz = await GetVerenigingenPerInsz(session, insz);
        return Ok(VerenigingPerInszMapper.ToResponse(verenigingenPerInsz, []));
    }

    private static async Task<VerenigingenPerInszDocument> GetVerenigingenPerInsz(IDocumentSession session, string insz)
    {
        return await session.Query<VerenigingenPerInszDocument>()
                            .Where(x => x.Insz.Equals(insz, StringComparison.CurrentCultureIgnoreCase))
                            .SingleOrDefaultAsync()
            ?? new VerenigingenPerInszDocument { Insz = insz };
    }
}
