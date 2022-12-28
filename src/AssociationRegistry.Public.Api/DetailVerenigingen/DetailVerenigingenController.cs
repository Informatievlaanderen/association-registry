namespace AssociationRegistry.Public.Api.DetailVerenigingen;

using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Be.Vlaanderen.Basisregisters.Api;
using Be.Vlaanderen.Basisregisters.Api.Exceptions;
using Constants;
using Infrastructure.ConfigurationBindings;
using Marten;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Schema.Detail;
using Swashbuckle.AspNetCore.Filters;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[ApiRoute("verenigingen")]
[ApiExplorerSettings(GroupName = "Verenigingen")]
public class DetailVerenigingenController : ApiController
{
    /// <summary>
    /// Vraag het detail van een vereniging op.
    /// </summary>
    /// <response code="200">Het detail van een vereniging</response>
    /// <response code="404">De gevraagde vereniging is niet gevonden</response>
    /// <response code="500">Als er een interne fout is opgetreden.</response>
    [HttpGet("{vCode}")]
    [ProducesResponseType(typeof(DetailVerenigingResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(DetailVerenigingResponseExamples))]
    [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
    [Produces(contentType: WellknownMediaTypes.JsonLd)]
    public async Task<IActionResult> Detail(
        [FromServices] AppSettings appsettings,
        [FromServices] IDocumentStore documentStore,
        [FromRoute] string vCode)
    {
        await using var session = documentStore.LightweightSession();
        var maybeVereniging = await session.Query<VerenigingDetailDocument>()
            .Where(document => document.VCode == vCode)
            .SingleOrDefaultAsync();

        if (maybeVereniging is not { } vereniging)
            return NotFound();

        return Ok(
            new DetailVerenigingResponse(
                $"{appsettings.BaseUrl}v1/contexten/detail-vereniging-context.json",
                new VerenigingDetail(
                    vereniging.VCode,
                    vereniging.Naam,
                    vereniging.KorteNaam,
                    vereniging.KorteBeschrijving,
                    vereniging.Startdatum,
                    vereniging.KboNummer,
                    vereniging.Status,
                    vereniging.Contacten.Select(
                            info => new ContactInfo(
                                info.Contactnaam,
                                info.Email,
                                info.Telefoon,
                                info.Website,
                                info.SocialMedia))
                        .ToArray(),
                    vereniging.Locaties.Select(ToLocatie).ToImmutableArray()),
                new Metadata(vereniging.DatumLaatsteAanpassing)));
    }

    private static Locatie ToLocatie(VerenigingDetailDocument.Locatie loc)
        => new(loc.Type, loc.Hoofdlocatie, loc.Adres, loc.Naam, loc.Straatnaam, loc.Huisnummer, loc.Busnummer, loc.Postcode, loc.Gemeente, loc.Land);
}
