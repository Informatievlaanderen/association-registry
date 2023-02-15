namespace AssociationRegistry.Admin.Api.Verenigingen.Detail;

using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Infrastructure.Extensions;
using Be.Vlaanderen.Basisregisters.Api;
using Be.Vlaanderen.Basisregisters.Api.Exceptions;
using Marten;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Projections.Detail;
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
    /// <param name="documentStore"></param>
    /// <param name="vCode">De VCode van de vereniging</param>
    /// <param name="expectedSequence">Sequentiewaarde verkregen bij creatie of aanpassing vereniging.</param>
    /// <response code="200">Het detail van een vereniging</response>
    /// <response code="404">De gevraagde vereniging is niet gevonden</response>
    /// <response code="412">De historiek van de gevraagde vereniging heeft niet de verwachte sequentiewaarde.</response>
    /// <response code="500">Als er een interne fout is opgetreden.</response>
    [HttpGet("{vCode}")]
    [ProducesResponseType(typeof(DetailVerenigingResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(DetailVerenigingResponseExamples))]
    [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
    public async Task<IActionResult> Detail(
        [FromServices] IDocumentStore documentStore,
        [FromRoute] string vCode,
        [FromQuery] long? expectedSequence)
    {
        await using var session = documentStore.LightweightSession();
        if (!await session.HasReachedSequence<BeheerVerenigingDetailDocument>(expectedSequence))
            return StatusCode(StatusCodes.Status412PreconditionFailed);

        var maybeVereniging = await session.Query<BeheerVerenigingDetailDocument>()
            .WithVCode(vCode)
            .SingleOrDefaultAsync();

        if (maybeVereniging is not { } vereniging)
            return NotFound();

        Response.AddETagHeader(vereniging.Metadata.Version);

        return Ok(
            new DetailVerenigingResponse(
                new DetailVerenigingResponse.VerenigingDetail(
                    vereniging.VCode,
                    vereniging.Naam,
                    vereniging.KorteNaam,
                    vereniging.KorteBeschrijving,
                    vereniging.Startdatum,
                    vereniging.KboNummer,
                    vereniging.Status,
                    vereniging.ContactInfoLijst.Select(ToContactInfo).ToImmutableArray(),
                    vereniging.Locaties.Select(ToLocatie).ToImmutableArray(),
                    vereniging.Vertegenwoordigers.Select(ToVertegenwoordiger).ToImmutableArray(),
                    vereniging.HoofdActiviteiten.Select(ToHoofdactiviteit).ToImmutableArray()),
                new DetailVerenigingResponse.MetadataDetail(vereniging.DatumLaatsteAanpassing)));
    }

    private static DetailVerenigingResponse.VerenigingDetail.HoofdActiviteit ToHoofdactiviteit(BeheerVerenigingDetailDocument.HoofdActiviteit hoofdActiviteit)
        => new(hoofdActiviteit.Code, hoofdActiviteit.Beschrijving);

    private static DetailVerenigingResponse.VerenigingDetail.ContactInfo ToContactInfo(BeheerVerenigingDetailDocument.ContactInfo info)
        => new(
            info.Contactnaam,
            info.Email,
            info.Telefoon,
            info.Website,
            info.SocialMedia);

    private static DetailVerenigingResponse.VerenigingDetail.Vertegenwoordiger ToVertegenwoordiger(BeheerVerenigingDetailDocument.Vertegenwoordiger ver)
        => new(
            ver.Insz,
            ver.Voornaam,
            ver.Achternaam,
            ver.Roepnaam,
            ver.Rol,
            ver.PrimairContactpersoon,
            ver.ContactInfoLijst.Select(ToContactInfo).ToImmutableArray());

    private static DetailVerenigingResponse.VerenigingDetail.Locatie ToLocatie(BeheerVerenigingDetailDocument.Locatie loc)
        => new(
            loc.Locatietype,
            loc.Hoofdlocatie,
            loc.Adres,
            loc.Naam,
            loc.Straatnaam,
            loc.Huisnummer,
            loc.Busnummer,
            loc.Postcode,
            loc.Gemeente,
            loc.Land);
}
