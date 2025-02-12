namespace AssociationRegistry.Public.Api.Verenigingen.Detail;

using Asp.Versioning;
using Be.Vlaanderen.Basisregisters.Api;
using Be.Vlaanderen.Basisregisters.Api.Exceptions;
using Constants;
using Infrastructure;
using Infrastructure.ConfigurationBindings;
using Infrastructure.Extensions;
using Marten;
using Microsoft.AspNetCore.Mvc;
using Queries;
using ResponseExamples;
using ResponseModels;
using Schema.Detail;
using Swashbuckle.AspNetCore.Filters;
using ProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ProblemDetails;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[ApiRoute("verenigingen")]
[ApiExplorerSettings(GroupName = "Opvragen van verenigingen")]
public class DetailVerenigingenController : ApiController
{
    /// <summary>
    ///     Vraag het detail van een vereniging op.
    /// </summary>
    /// <param name="vCode">De unieke identificatie code van deze vereniging</param>
    /// <param name="version">De versie van dit endpoint.</param>
    /// <response code="200">Het detail van een vereniging</response>
    /// <response code="404">De gevraagde vereniging is niet gevonden</response>
    /// <response code="500">Er is een interne fout opgetreden.</response>
    [HttpGet("{vCode}")]
    [ProducesResponseType(typeof(PubliekVerenigingDetailResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(DetailVerenigingResponseExamples))]
    [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
    [Produces(WellknownMediaTypes.JsonLd)]
    public async Task<IActionResult> Detail(
        [FromServices] IDocumentStore store,
        [FromServices] IGetNamesForVCodesQuery getNamesForVCodesQuery,
        [FromServices] AppSettings appSettings,
        [FromRoute] string vCode,
        [FromHeader(Name = WellknownHeaderNames.Version)] string? version,
        CancellationToken cancellationToken)
    {
        await using var session = store.LightweightSession();

        var vereniging = await GetDetail(session, vCode);

        if (vereniging is null)
            return NotFound();

        var andereVerenigingen = vereniging.Lidmaatschappen.Select(x => x.AndereVereniging).ToArray();

        var namesForLidmaatschappen =
            await getNamesForVCodesQuery.ExecuteAsync(new GetNamesForVCodesFilter(andereVerenigingen), cancellationToken);

        var responseMapper = new PubliekVerenigingDetailMapper(appSettings, version);

        return Ok(responseMapper.Map(vereniging, new VerplichteNamenVoorLidmaatschapMapper(namesForLidmaatschappen)));
    }

    private static async Task<PubliekVerenigingDetailDocument?> GetDetail(IQuerySession session, string vCode)
        => await session
                .Query<PubliekVerenigingDetailDocument>()
                .WithVCode(vCode)
                .OnlyIngeschrevenInPubliekeDatastroom()
                .OnlyActief()
                .SingleOrDefaultAsync();
}
