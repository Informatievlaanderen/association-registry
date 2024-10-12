namespace AssociationRegistry.Public.Api.Verenigingen.Detail;

using Asp.Versioning;
using Be.Vlaanderen.Basisregisters.Api;
using Be.Vlaanderen.Basisregisters.Api.Exceptions;
using Be.Vlaanderen.Basisregisters.AspNetCore.Mvc.Formatters.Json;
using Constants;
using Infrastructure.ConfigurationBindings;
using Infrastructure.Extensions;
using Marten;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.NewtonsoftJson;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Queries;
using ResponseModels;
using Schema.Constants;
using Schema.Detail;
using Swashbuckle.AspNetCore.Filters;
using System.Text;
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
        [FromRoute] string vCode)
    {
        await using var session = store.LightweightSession();

        var vereniging = await GetDetail(session, vCode);

        if (vereniging is null)
            return NotFound();

        return Ok(PubliekVerenigingDetailMapper.Map(vereniging, _appsettings));
    }

    /// <summary>
    ///     Vraag het detail van alle vereniging op.
    /// </summary>
    /// <response code="200">Het detail van alle vereniging</response>
    /// <response code="500">Er is een interne fout opgetreden.</response>
    [HttpGet("detail/all")]
    [ProducesResponseType(typeof(PubliekVerenigingDetailResponse[]), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(DetailAllVerenigingResponseExamples))]
    [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
    [Produces(WellknownMediaTypes.JsonLd)]
    public async Task GetAll(
        [FromServices] IQuery<IAsyncEnumerable<PubliekVerenigingDetailDocument>> query,
        [FromServices] IResponseWriter responseWriter,
        CancellationToken cancellationToken)
    {
        try
        {
            var data = await query.ExecuteAsync(cancellationToken);
            await responseWriter.Write(Response, data, cancellationToken);
        }
        catch (TaskCanceledException)
        {
            // Nothing to do, user stopped the request
        }
    }

    /// <summary>
    ///     Vraag het detail van alle vereniging op.
    /// </summary>
    /// <response code="200">Het detail van alle vereniging</response>
    /// <response code="500">Er is een interne fout opgetreden.</response>
    [HttpGet("detail/all/redirect")]
    [ProducesResponseType(typeof(PubliekVerenigingDetailResponse[]), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(DetailAllVerenigingResponseExamples))]
    [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
    [Produces(WellknownMediaTypes.JsonLd)]
    public async Task<IActionResult> GetAllThroughS3(
        [FromServices] IQuery<IAsyncEnumerable<PubliekVerenigingDetailDocument>> query,
        [FromServices] IS3Wrapper s3Wrapper,
        [FromServices] AppSettings appSettings,
        CancellationToken cancellationToken)
    {
        var data = await query.ExecuteAsync(cancellationToken);
        var serializerSettings = JsonSerializerSettingsProvider.CreateSerializerSettings().ConfigureDefaultForApi();

        await using var inputStream = new MemoryStream();
        await using var writer = new StreamWriter(inputStream, Encoding.UTF8);

        await foreach (var vereniging in data.WithCancellation(cancellationToken))
        {
            if (IsTeVerwijderenVereniging(vereniging))
            {
                var teVerwijderenVereniging =
                    JsonConvert.SerializeObject(
                        new ResponseWriter.TeVerwijderenVereniging()
                        {
                            Vereniging = new ResponseWriter.TeVerwijderenVereniging.TeVerwijderenVerenigingData()
                            {
                                VCode = vereniging.VCode,
                                TeVerwijderen = true,
                                DeletedAt = vereniging.DatumLaatsteAanpassing,
                            }
                        },
                        serializerSettings);

                await writer.WriteLineAsync(teVerwijderenVereniging);

                continue;
            }

            var mappedVereniging = PubliekVerenigingDetailMapper.MapDetailAll(vereniging, appSettings);
            var json = JsonConvert.SerializeObject(mappedVereniging, serializerSettings);
            await writer.WriteLineAsync(json);
        }

        await writer.FlushAsync(cancellationToken);

        await s3Wrapper.PutAsync(_appsettings.Publiq.BucketName, _appsettings.Publiq.Key, inputStream, cancellationToken);
        var redirectUrl = await s3Wrapper.GetPreSignedUrlAsync(_appsettings.Publiq.BucketName, _appsettings.Publiq.Key, cancellationToken);

        Response.Headers.Location = redirectUrl;
        return StatusCode(StatusCodes.Status307TemporaryRedirect, redirectUrl);

        bool IsTeVerwijderenVereniging(PubliekVerenigingDetailDocument vereniging)
            => vereniging.Deleted ||
               vereniging.Status == VerenigingStatus.Gestopt ||
               vereniging.IsUitgeschrevenUitPubliekeDatastroom is not null &&
               vereniging.IsUitgeschrevenUitPubliekeDatastroom.Value;
    }

    private static async Task<PubliekVerenigingDetailDocument?> GetDetail(IQuerySession session, string vCode)
        => await session
                .Query<PubliekVerenigingDetailDocument>()
                .WithVCode(vCode)
                .OnlyIngeschrevenInPubliekeDatastroom()
                .OnlyActief()
                .SingleOrDefaultAsync();
}
