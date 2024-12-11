namespace AssociationRegistry.Public.Api.Verenigingen.Detail;

using Asp.Versioning;
using Be.Vlaanderen.Basisregisters.Api;
using Be.Vlaanderen.Basisregisters.Api.Exceptions;
using Constants;
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
    private readonly AppSettings _appsettings;

    public DetailVerenigingenController(AppSettings appsettings)
    {
        _appsettings = appsettings;
    }

    /// <summary>
    ///     Vraag het detail van een vereniging op.
    /// </summary>
    /// <param name="vCode">De unieke identificatie code van deze vereniging</param>
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
        [FromRoute] string vCode,
        CancellationToken cancellationToken)
    {
        await using var session = store.LightweightSession();

        var vereniging = await GetDetail(session, vCode);

        if (vereniging is null)
            return NotFound();

        var andereVerenigingen = vereniging.Lidmaatschappen.Select(x => x.AndereVereniging).ToArray();

        var namesForLidmaatschappen =
            await getNamesForVCodesQuery.ExecuteAsync(new GetNamesForVCodesFilter(andereVerenigingen), cancellationToken);

        return Ok(PubliekVerenigingDetailMapper.Map(vereniging, _appsettings,
                                                    new VerplichteNamenVoorLidmaatschapMapper(namesForLidmaatschappen)));
    }

    private static async Task<PubliekVerenigingDetailDocument?> GetDetail(IQuerySession session, string vCode)
        => await session
                .Query<PubliekVerenigingDetailDocument>()
                .WithVCode(vCode)
                .OnlyIngeschrevenInPubliekeDatastroom()
                .OnlyActiefOrDubbel()
                .SingleOrDefaultAsync();
}
