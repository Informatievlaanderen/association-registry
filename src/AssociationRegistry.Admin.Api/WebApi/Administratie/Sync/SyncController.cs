namespace AssociationRegistry.Admin.Api.WebApi.Administratie.Sync;

using Asp.Versioning;
using AssociationRegistry.Admin.Api.Infrastructure.WebApi.Swagger.Annotations;
using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Historiek.Examples;
using AssociationRegistry.Admin.Schema.KboSync;
using AssociationRegistry.Events;
using Be.Vlaanderen.Basisregisters.Api;
using Be.Vlaanderen.Basisregisters.Api.Exceptions;
using CommandHandling.InschrijvingenVertegenwoordigers;
using CommandHandling.MagdaSync.SyncKsz;
using CommandHandling.MagdaSync.SyncKsz.Queries;
using Contracts.Sync.Kbo;
using Contracts.Sync.Ksz;
using DecentraalBeheer.Vereniging;
using Framework;
using Infrastructure.WebApi.Swagger.Annotations;
using Marten;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Persoonsgegevens;
using Queries;
using Schema.KboSync;
using Swashbuckle.AspNetCore.Filters;
using Verenigingen.Historiek.Examples;
using Wolverine;
using Wolverine.Marten;
using ProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ProblemDetails;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[ApiRoute("admin/sync")]
[ApiExplorerSettings(IgnoreApi = true)]
[Authorize(Policy = Program.SuperAdminPolicyName)]
public class SyncController : ApiController
{
    private readonly KboSyncHistoriekResponseMapper _mapper;
    private readonly ILogger<SyncController> _logger;

    public SyncController(KboSyncHistoriekResponseMapper mapper, ILogger<SyncController> logger)
    {
        _mapper = mapper;
        _logger = logger;
    }

    /// <summary>
    ///     Vraag de historiek van de KBO sync op.
    /// </summary>
    /// <param name="documentStore"></param>
    /// <param name="problemDetailsHelper"></param>
    /// <response code="200">De historiek van de KBO sync</response>
    /// <response code="500">Er is een interne fout opgetreden.</response>
    [HttpGet("kbo/historiek")]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(HistoriekResponseExamples))]
    [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesJson]
    public async Task<IActionResult> GetKboSyncHistoriek(
        [FromServices] IDocumentStore documentStore,
        [FromServices] ProblemDetailsHelper problemDetailsHelper
    )
    {
        await using var session = documentStore.LightweightSession();

        var gebeurtenissen = await session.Query<BeheerKboSyncHistoriekGebeurtenisDocument>().ToListAsync();

        var response = _mapper.Map(gebeurtenissen);

        return Ok(response);
    }

    /// <summary>
    ///     Vraag de historiek van de KSZ sync op.
    /// </summary>
    /// <param name="vCode"></param>
    /// <param name="documentStore"></param>
    /// <param name="problemDetailsHelper"></param>
    /// <param name="kszSyncHistoriekQuery"></param>
    /// <param name="cancellationToken"></param>
    /// <response code="200">De historiek van de KBO sync</response>
    /// <response code="500">Er is een interne fout opgetreden.</response>
    [HttpGet("ksz/historiek")]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(HistoriekResponseExamples))]
    [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesJson]
    public async Task<IActionResult> GetKszSyncHistoriek(
        [FromQuery] string? vCode,
        [FromServices] IDocumentStore documentStore,
        [FromServices] ProblemDetailsHelper problemDetailsHelper,
        [FromServices] IKszSyncHistoriekQuery kszSyncHistoriekQuery,
        CancellationToken cancellationToken
    )
    {
        await using var session = documentStore.LightweightSession();

        var gebeurtenissen = await kszSyncHistoriekQuery.ExecuteAsync(
            new KszSyncHistoriekFilter(vCode),
            cancellationToken
        );

        return Ok(gebeurtenissen);
    }

    /// <summary>
    ///     Synchroniseer alle verenigingen met rechtspersoonlijkheid via de Magda GeefOnderneming dienst.
    /// Verenigingen die nog geen inschrijving hebben geregistreerd zullen door deze actie ook automatisch ingeschreven worden op wijzigingen uit de KBO.
    /// </summary>
    /// <param name="documentStore"></param>
    /// <param name="messageBus"></param>
    /// <response code="202">Indien er geen fouten zijn opgetreden.</response>
    /// <response code="500">Er is een interne fout opgetreden.</response>
    [HttpPost("kbo/all")]
    [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesJson]
    public async Task<IActionResult> SyncAllVerenigingen(
        [FromServices] IDocumentStore documentStore,
        [FromServices] IMessageBus messageBus
    )
    {
        await using var session = documentStore.LightweightSession();

        var kboNummersToSync = await session
            .Events.QueryRawEventDataOnly<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>()
            .Select(x => x.KboNummer)
            .ToListAsync();

        foreach (var kboNummer in kboNummersToSync)
        {
            await messageBus.SendAsync(new TeSynchroniserenKboNummerMessage(kboNummer));
        }

        return Accepted();
    }

    /// <summary>
    /// Synchroniseer vereniging met rechtspersoonlijkheid via de Magda GeefOnderneming dienst.
    /// Indien de vereniging nog inschrijving heeft geregistreerd zal die door deze actie ook automatisch ingeschreven worden op wijzigingen uit de KBO.
    /// </summary>
    /// <param name="documentStore"></param>
    /// <param name="messageBus"></param>
    /// <param name="vCode">De VCode van de te synchroniseren vereniging.</param>
    /// <response code="202">Indien er geen fouten zijn opgetreden.</response>
    /// <response code="500">Er is een interne fout opgetreden.</response>
    [HttpPost("kbo/{vCode}")]
    [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesJson]
    public async Task<IActionResult> SyncVereniging(
        [FromServices] IDocumentStore documentStore,
        [FromServices] IMessageBus messageBus,
        [FromRoute] string vCode
    )
    {
        await using var session = documentStore.LightweightSession();

        var verenigingMetRechtspersoonlijkheidWerdGeregistreerd = await session
            .Events.QueryRawEventDataOnly<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>()
            .SingleOrDefaultAsync(x => x.VCode == vCode);

        if (verenigingMetRechtspersoonlijkheidWerdGeregistreerd is null)
            return NotFound();

        await messageBus.SendAsync(
            new TeSynchroniserenKboNummerMessage(verenigingMetRechtspersoonlijkheidWerdGeregistreerd.KboNummer)
        );

        return Accepted();
    }

    /// <summary>
    /// Trigger een initiële ksz sync voor een verenignig.
    /// </summary>
    /// <param name="documentStore"></param>
    /// <param name="messageBus"></param>
    /// <param name="vzerOrFvExistsVCodeQuery"></param>
    /// <param name="vCode">De VCode van de te synchroniseren vereniging.</param>
    /// <response code="202">Indien er geen fouten zijn opgetreden.</response>
    /// <response code="500">Er is een interne fout opgetreden.</response>
    [HttpPost("initial/ksz/{vCode}")]
    [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesJson]
    public async Task<IActionResult> SyncInitialKszVereniging(
        [FromServices] IDocumentStore documentStore,
        [FromServices] IMessageBus messageBus,
        [FromServices] IVzerOrFvExistsQuery vzerOrFvExistsVCodeQuery,
        [FromRoute] string vCode
    )
    {
        await using var session = documentStore.LightweightSession();

        var exists = await vzerOrFvExistsVCodeQuery.ExecuteAsync(
            new VzerOrFvExistsFilter(vCode),
            CancellationToken.None
        );

        if (!exists)
            return NotFound();

        await messageBus.SendAsync(
            new CommandEnvelope<SchrijfVertegenwoordigersInMessage>(
                new SchrijfVertegenwoordigersInMessage(vCode),
                CommandMetadata.ForDigitaalVlaanderenProcess
            )
        );

        return Accepted();
    }

    /// <summary>
    /// Trigger een initiële ksz sync voor meerdere verenignigen.
    /// </summary>
    /// <param name="documentStore"></param>
    /// <param name="messageBus"></param>
    /// <param name="vzerOrFvExistsVCodeQuery"></param>
    /// <param name="vCode">De VCode van de te synchroniseren vereniging.</param>
    /// <response code="202">Indien er geen fouten zijn opgetreden.</response>
    /// <response code="500">Er is een interne fout opgetreden.</response>
    [HttpPost("initial/ksz")]
    [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesJson]
    public async Task<IActionResult> SyncInitialKszVerenigingen(
        [FromServices] IDocumentStore documentStore,
        [FromServices] IMessageBus messageBus,
        [FromServices] IVzerOrFvExistsQuery vzerOrFvExistsVCodeQuery,
        [FromBody] kszSyncRequest request
    )
    {
        await using var session = documentStore.LightweightSession();

        foreach (var vCode in request.VCodes)
        {
            _logger.LogInformation("start syncing vcode {vcode}", vCode);

            var exists = await vzerOrFvExistsVCodeQuery.ExecuteAsync(
                new VzerOrFvExistsFilter(vCode),
                CancellationToken.None
            );

            if (!exists)
                return NotFound();

            await messageBus.SendAsync(
                new CommandEnvelope<SchrijfVertegenwoordigersInMessage>(
                    new SchrijfVertegenwoordigersInMessage(vCode),
                    CommandMetadata.ForDigitaalVlaanderenProcess
                )
            );
            _logger.LogInformation("syncing vcode {vcode} done", vCode);
        }

        return Accepted();
    }

    /// <summary>
    /// Sync insz met ksz
    /// </summary>
    /// <param name="messageBus"></param>
    /// <param name="insz"></param>
    /// <param name="kszMessageHandler"></param>
    /// <param name="cancellationToken"></param>
    /// <response code="202">Indien er geen fouten zijn opgetreden.</response>
    /// <response code="500">Er is een interne fout opgetreden.</response>
    [HttpPost("ksz/{insz}")]
    [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesJson]
    public async Task<IActionResult> SyncKsz(
        [FromRoute] string insz,
        [FromServices] IMartenOutbox martenOutbox,
        [FromServices] ISyncKszMessageHandler kszMessageHandler,
        CancellationToken cancellationToken
    )
    {
        await kszMessageHandler.Handle(
            new CommandEnvelope<SyncKszMessage>(
                new SyncKszMessage(Insz.Create(insz), Guid.NewGuid()),
                CommandMetadata.ForDigitaalVlaanderenProcess
            ),
            martenOutbox,
            cancellationToken
        );

        return Accepted();
    }
}
