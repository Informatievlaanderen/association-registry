namespace AssociationRegistry.Acm.Api.VerenigingenPerInsz;

using Asp.Versioning;
using Be.Vlaanderen.Basisregisters.Api;
using Be.Vlaanderen.Basisregisters.Api.Exceptions;
using Marten;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Schema.VerenigingenPerInsz;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.Linq;
using System.Threading.Tasks;
using ProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ProblemDetails;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[ApiRoute("verenigingen")]
[ApiExplorerSettings(GroupName = "Decentraal beheer van feitelijke verenigingen")]
public class VerenigingenPerInszController : ApiController
{
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

        var document = await GetDocument(session, insz);
        return Ok(document.ToResponse());
    }

    private static async Task<VerenigingenPerInszDocument> GetDocument(IDocumentSession session, string insz)
    {
        return await session.Query<VerenigingenPerInszDocument>()
                            .Where(x => x.Insz.Equals(insz, StringComparison.CurrentCultureIgnoreCase))
                            .SingleOrDefaultAsync()
            ?? new VerenigingenPerInszDocument { Insz = insz };
    }
}
