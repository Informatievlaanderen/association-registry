namespace AssociationRegistry.Public.Api.Verenigingen.Detail;

using Asp.Versioning;
using Be.Vlaanderen.Basisregisters.Api;
using Be.Vlaanderen.Basisregisters.Api.Exceptions;
using Constants;
using Infrastructure.ConfigurationBindings;
using Infrastructure.Extensions;
using Marten;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Queries;
using ResponseModels;
using Schema.Constants;
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

    private static async Task<PubliekVerenigingDetailDocument?> GetDetail(IQuerySession session, string vCode)
        => await session
                .Query<PubliekVerenigingDetailDocument>()
                .WithVCode(vCode)
                .OnlyIngeschrevenInPubliekeDatastroom()
                .OnlyActief()
                .SingleOrDefaultAsync();
}

public interface IResponseWriter
{
    Task Write(HttpResponse response, IAsyncEnumerable<PubliekVerenigingDetailDocument> data, CancellationToken cancellationToken);
}

public class ResponseWriter : IResponseWriter
{
    private readonly AppSettings _appSettings;

    public ResponseWriter(AppSettings appSettings)
    {
        _appSettings = appSettings;
    }

    public async Task Write(HttpResponse response, IAsyncEnumerable<PubliekVerenigingDetailDocument> data, CancellationToken cancellationToken)
    {
        await using var writer = new StreamWriter(response.Body);
        var serializer = Newtonsoft.Json.JsonSerializer.CreateDefault();
        var isFirst = true;
        await writer.WriteAsync('[');

        await foreach (var vereniging in data.WithCancellation(cancellationToken))
        {
            isFirst = await SeparateWithComma(writer, isFirst);

            serializer.Serialize(writer, PubliekVerenigingDetailMapper.Map(vereniging, _appSettings));

            await writer.FlushAsync(cancellationToken);
        }

        await writer.WriteAsync(']');

    }


    private static async Task<bool> SeparateWithComma(StreamWriter writer, bool isFirst)
    {
        if (!isFirst)
        {
            await writer.WriteAsync(',');

        }

        return false;
    }
}
