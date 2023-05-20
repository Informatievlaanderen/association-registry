namespace AssociationRegistry.Public.Api.Verenigingen.Detail;

using System.Threading.Tasks;
using Be.Vlaanderen.Basisregisters.Api;
using Be.Vlaanderen.Basisregisters.Api.Exceptions;
using Constants;
using Infrastructure.ConfigurationBindings;
using Infrastructure.Extensions;
using Marten;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ResponseModels;
using Schema.Detail;
using Swashbuckle.AspNetCore.Filters;
using ProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ProblemDetails;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[ApiRoute("verenigingen")]
[ApiExplorerSettings(GroupName = "Verenigingen")]
public class DetailVerenigingenController : ApiController
{
    private readonly AppSettings _appsettings;
    private readonly IDocumentStore _documentStore;

    public DetailVerenigingenController(AppSettings appsettings, IDocumentStore documentStore)
    {
        _appsettings = appsettings;
        _documentStore = documentStore;
    }

    /// <summary>
    ///     Vraag het detail van een vereniging op.
    /// </summary>
    /// <param name="vCode">De unieke identificatie code van deze vereniging</param>
    /// <response code="200">Het detail van een vereniging</response>
    /// <response code="404">De gevraagde vereniging is niet gevonden</response>
    /// <response code="500">Als er een interne fout is opgetreden.</response>
    [HttpGet("{vCode}")]
    [ProducesResponseType(typeof(PubliekVerenigingDetailResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(DetailVerenigingResponseExamples))]
    [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
    [Produces(WellknownMediaTypes.JsonLd)]
    public async Task<IActionResult> Detail(
        [FromRoute] string vCode)
    {
        await using var session = _documentStore.LightweightSession();
        var vereniging = await session.Query<PubliekVerenigingDetailDocument>()
            .WithVCode(vCode)
            .SingleOrDefaultAsync();

        if (vereniging is null)
            return NotFound();

        return Ok(PubliekVerenigingDetailMapper.Map(vereniging, _appsettings));
    }
}
