namespace AssociationRegistry.Public.Api.Verenigingen.Detail;

using System.Linq;
using System.Threading.Tasks;
using Be.Vlaanderen.Basisregisters.Api;
using Be.Vlaanderen.Basisregisters.Api.Exceptions;
using Constants;
using Infrastructure.ConfigurationBindings;
using Infrastructure.Extensions;
using Marten;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
    [ProducesResponseType(typeof(DetailVerenigingResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(DetailVerenigingResponseExamples))]
    [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
    [Produces(WellknownMediaTypes.JsonLd)]
    public async Task<IActionResult> Detail(
        [FromRoute] string vCode)
    {
        await using var session = _documentStore.LightweightSession();
        var maybeVereniging = await session.Query<PubliekVerenigingDetailDocument>()
            .WithVCode(vCode)
            .SingleOrDefaultAsync();

        if (maybeVereniging is not { } vereniging)
            return NotFound();

        return Ok(
            new DetailVerenigingResponse(
                $"{_appsettings.BaseUrl}/v1/contexten/detail-vereniging-context.json",
                new VerenigingDetail(
                    vereniging.VCode,
                    vereniging.Naam,
                    vereniging.KorteNaam,
                    vereniging.KorteBeschrijving,
                    vereniging.Startdatum,
                    vereniging.Status,
                    vereniging.Contactgegevens.Select(
                            info => new Contactgegeven(
                                info.Type,
                                info.Waarde,
                                info.Beschrijving,
                                info.IsPrimair))
                        .ToArray(),
                    vereniging.Locaties.Select(ToLocatie).ToArray(),
                    vereniging.HoofdactiviteitenVerenigingsloket.Select(ToHoofdactiviteit).ToArray()),
                new Metadata(vereniging.DatumLaatsteAanpassing)));
    }

    private static HoofdactiviteitVerenigingsloket ToHoofdactiviteit(PubliekVerenigingDetailDocument.HoofdactiviteitVerenigingsloket ha)
        => new(ha.Code, ha.Beschrijving);

    private static Locatie ToLocatie(PubliekVerenigingDetailDocument.Locatie loc)
        => new(loc.Locatietype, loc.Hoofdlocatie, loc.Adres, loc.Naam, loc.Straatnaam, loc.Huisnummer, loc.Busnummer, loc.Postcode, loc.Gemeente, loc.Land);
}
