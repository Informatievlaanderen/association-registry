namespace AssociationRegistry.Admin.Api.Verenigingen.KboSync;

using Asp.Versioning;
using Be.Vlaanderen.Basisregisters.Api;
using Be.Vlaanderen.Basisregisters.Api.Exceptions;
using Events;
using Historiek.Examples;
using Infrastructure.AWS;
using Infrastructure.Swagger.Annotations;
using Marten;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResponseModels;
using Schema.KboSync;
using Swashbuckle.AspNetCore.Filters;
using ProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ProblemDetails;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[ApiRoute("verenigingen/kbo")]
[ApiExplorerSettings(IgnoreApi = true)]
[Authorize(Policy = Program.SuperAdminPolicyName)]
public class KboSyncHistoriekController : ApiController
{
    private readonly KboSyncHistoriekResponseMapper _mapper;

    public KboSyncHistoriekController(KboSyncHistoriekResponseMapper mapper)
    {
        _mapper = mapper;
    }

    /// <summary>
    ///     Vraag de historiek van de KBO sync op.
    /// </summary>
    /// <param name="documentStore"></param>
    /// <param name="problemDetailsHelper"></param>
    /// <response code="200">De historiek van de KBO sync</response>
    /// <response code="500">Er is een interne fout opgetreden.</response>
    [HttpGet("historiek")]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(HistoriekResponseExamples))]
    [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
    [ProducesResponseType(typeof(KboSyncHistoriekResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesJson]
    public async Task<IActionResult> GetKboSyncHistoriek(
        [FromServices] IDocumentStore documentStore,
        [FromServices] ProblemDetailsHelper problemDetailsHelper
    )
    {
        await using var session = documentStore.LightweightSession();

        var gebeurtenissen = await session
                                  .Query<BeheerKboSyncHistoriekGebeurtenisDocument>()
                                  .ToListAsync();

        var response = _mapper.Map(gebeurtenissen);

        return Ok(response);
    }

    /// <summary>
    ///     Synchroniseer alle verenigingen met rechtspersoonlijkheid via de Magda GeefOnderneming dienst.
    /// Verenigingen die nog geen inschrijving hebben geregistreerd zullen door deze actie ook automatisch ingeschreven worden op wijzigingen uit de KBO.
    /// </summary>
    /// <param name="documentStore"></param>
    /// <param name="clientWrapper"></param>
    /// <response code="202">Indien er geen fouten zijn opgetreden.</response>
    /// <response code="500">Er is een interne fout opgetreden.</response>
    [HttpPost("sync")]
    [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesJson]
    public async Task<IActionResult> SyncAllVerenigingen(
        [FromServices] IDocumentStore documentStore,
        [FromServices] SqsClientWrapper clientWrapper
    )
    {
        await using var session = documentStore.LightweightSession();

        var kboNummersToSync = await session
                                    .Events.QueryRawEventDataOnly<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>()
                                    .Select(x => x.KboNummer)
                                    .ToListAsync();

        foreach (var kboNummer in kboNummersToSync)
        {
            await clientWrapper.QueueKboNummerToSynchronise(kboNummer);
        }

        return Accepted();
    }

    /// <summary>
    /// Synchroniseer vereniging met rechtspersoonlijkheid via de Magda GeefOnderneming dienst.
    /// Indien de vereniging nog inschrijving heeft geregistreerd zal die door deze actie ook automatisch ingeschreven worden op wijzigingen uit de KBO.
    /// </summary>
    /// <param name="documentStore"></param>
    /// <param name="clientWrapper"></param>
    /// <param name="vCode">De VCode van de te synchroniseren vereniging.</param>
    /// <response code="202">Indien er geen fouten zijn opgetreden.</response>
    /// <response code="500">Er is een interne fout opgetreden.</response>
    [HttpPost("sync/{vCode}")]
    [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesJson]
    public async Task<IActionResult> SyncVereniging(
        [FromServices] IDocumentStore documentStore,
        [FromServices] SqsClientWrapper clientWrapper,
        [FromRoute] string vCode
    )
    {
        await using var session = documentStore.LightweightSession();

        var verenigingMetRechtspersoonlijkheidWerdGeregistreerd =
            await session
                 .Events
                 .QueryRawEventDataOnly<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>()
                 .SingleOrDefaultAsync(x => x.VCode == vCode);

        if (verenigingMetRechtspersoonlijkheidWerdGeregistreerd is null)
            return NotFound();

        await clientWrapper.QueueKboNummerToSynchronise(verenigingMetRechtspersoonlijkheidWerdGeregistreerd.KboNummer);

        return Accepted();
    }
}
