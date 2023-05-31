namespace AssociationRegistry.Admin.Api.Verenigingen.Detail;

using System.Linq;
using System.Threading.Tasks;
using Be.Vlaanderen.Basisregisters.Api;
using Be.Vlaanderen.Basisregisters.Api.Exceptions;
using Infrastructure.Extensions;
using Marten;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Projections.Detail;
using Swashbuckle.AspNetCore.Filters;
using Vereniging;
using ProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ProblemDetails;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[ApiRoute("verenigingen")]
[ApiExplorerSettings(GroupName = "Verenigingen")]
public class DetailVerenigingenController : ApiController
{
    /// <summary>
    ///     Vraag het detail van een vereniging op.
    /// </summary>
    /// <param name="documentStore"></param>
    /// <param name="vCode">De vCode van de vereniging</param>
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
    [SwaggerResponseHeader(StatusCodes.Status200OK, "ETag", "string", "De versie van de aangepaste vereniging.")]
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
            new DetailVerenigingResponse
            {
                Vereniging =
                    new DetailVerenigingResponse.VerenigingDetail
                    {
                        VCode = vereniging.VCode,
                        Type = vereniging.Type,
                        Naam = vereniging.Naam,
                        KorteNaam = vereniging.KorteNaam,
                        KorteBeschrijving = vereniging.KorteBeschrijving,
                        Startdatum = vereniging.Startdatum,
                        Status = vereniging.Status,
                        Contactgegevens = vereniging.Contactgegevens.Select(ToContactgegeven).ToArray(),
                        Locaties = vereniging.Locaties.Select(ToLocatie).ToArray(),
                        Vertegenwoordigers = vereniging.Vertegenwoordigers.Select(ToVertegenwoordiger).ToArray(),
                        HoofdactiviteitenVerenigingsloket = vereniging.HoofdactiviteitenVerenigingsloket.Select(ToHoofdactiviteit).ToArray(),
                        Sleutels = vereniging.Sleutels.Select(ToSleutel).ToArray(),
                    },
                Metadata = new DetailVerenigingResponse.MetadataDetail
                {
                    DatumLaatsteAanpassing = vereniging.DatumLaatsteAanpassing,
                    BeheerBasisUri = $"/verenigingen/{vereniging.VCode}"+(vereniging.Type.Code == Verenigingstype.VerenigingMetRechtspersoonlijkheid.Code?"/kbo":""),
                },
            });
    }

    private static DetailVerenigingResponse.VerenigingDetail.Sleutel ToSleutel(BeheerVerenigingDetailDocument.Sleutel s)
        => new()
        {
            Bron = s.Bron,
            Waarde = s.Waarde,
        };

    private static DetailVerenigingResponse.VerenigingDetail.Contactgegeven ToContactgegeven(BeheerVerenigingDetailDocument.Contactgegeven contactgegeven)
        => new()
        {
            ContactgegevenId = contactgegeven.ContactgegevenId,
            Type = contactgegeven.Type,
            Waarde = contactgegeven.Waarde,
            Beschrijving = contactgegeven.Beschrijving,
            IsPrimair = contactgegeven.IsPrimair,
        };

    private static DetailVerenigingResponse.VerenigingDetail.HoofdactiviteitVerenigingsloket ToHoofdactiviteit(BeheerVerenigingDetailDocument.HoofdactiviteitVerenigingsloket hoofdactiviteitVerenigingsloket)
        => new()
        {
            Code = hoofdactiviteitVerenigingsloket.Code,
            Beschrijving = hoofdactiviteitVerenigingsloket.Beschrijving,
        };

    private static DetailVerenigingResponse.VerenigingDetail.Vertegenwoordiger ToVertegenwoordiger(BeheerVerenigingDetailDocument.Vertegenwoordiger ver)
        => new()
        {
            VertegenwoordigerId = ver.VertegenwoordigerId,
            Voornaam = ver.Voornaam,
            Achternaam = ver.Achternaam,
            Roepnaam = ver.Roepnaam,
            Rol = ver.Rol,
            PrimairContactpersoon = ver.IsPrimair,
            Email = ver.Email,
            Telefoon = ver.Telefoon,
            Mobiel = ver.Mobiel,
            SocialMedia = ver.SocialMedia,
        };

    private static DetailVerenigingResponse.VerenigingDetail.Locatie ToLocatie(BeheerVerenigingDetailDocument.Locatie loc)
        => new()
        {
            Locatietype = loc.Locatietype,
            Hoofdlocatie = loc.Hoofdlocatie,
            Adres = loc.Adres,
            Naam = loc.Naam,
            Straatnaam = loc.Straatnaam,
            Huisnummer = loc.Huisnummer,
            Busnummer = loc.Busnummer,
            Postcode = loc.Postcode,
            Gemeente = loc.Gemeente,
            Land = loc.Land,
        };
}
