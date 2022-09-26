namespace AssociationRegistry.Public.Api.DetailVerenigingen;

using System.Net;
using Infrastructure.Json;
using Newtonsoft.Json;
using System.Threading.Tasks;
using ListVerenigingen;
using Be.Vlaanderen.Basisregisters.Api;
using Be.Vlaanderen.Basisregisters.Api.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.Filters;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[ApiRoute("verenigingen")]
[ApiExplorerSettings(GroupName = "Verenigingen")]
public class DetailVerenigingenController : ApiController
{
    private readonly IVerenigingenRepository _verenigingenRepository;

    public DetailVerenigingenController(IVerenigingenRepository verenigingenRepository)
    {
        _verenigingenRepository = verenigingenRepository;
    }

    /// <summary>
    /// Vraag het detail van een vereniging op.
    /// </summary>
    /// <response code="200">Het detail van een vereniging</response>
    /// <response code="500">Als er een interne fout is opgetreden.</response>
    [HttpGet("{vCode}")]
    [ProducesResponseType(typeof(DetailVerenigingResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(DetailVerenigingResponseExamples))]
    [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
    [Produces(contentType: WellknownMediaTypes.JsonLd)]
    public async Task<IActionResult> Detail(
        [FromServices] DetailVerenigingContext detailVerenigingContext,
        [FromRoute] string vCode)
    {
        var maybeVereniging = await _verenigingenRepository.Detail(vCode);
        if (maybeVereniging is not { } vereniging)
            return NotFound();

        return new ContentResult
        {
            ContentType = "application/ld+json",
            StatusCode = (int)HttpStatusCode.OK,
            Content = JsonConvert.SerializeObject(
                new DetailVerenigingResponse(detailVerenigingContext, vereniging),
                Formatting.Indented,
                GetJsonSerializerSettings()),
        };
    }

    private static JsonSerializerSettings GetJsonSerializerSettings()
    {
        var getSerializerSettings = JsonConvert.DefaultSettings ?? (() => new JsonSerializerSettings());
        var jsonSerializerSettings = getSerializerSettings();

        jsonSerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
        jsonSerializerSettings.Converters.Add(new DateOnlyJsonConvertor("yyyy-MM-dd"));
        jsonSerializerSettings.Converters.Add(new NullableDateOnlyJsonConvertor("yyyy-MM-dd"));

        return jsonSerializerSettings;
    }
}

public class DetailVerenigingResponseExamples : IExamplesProvider<DetailVerenigingResponse>
{
    public DetailVerenigingResponse GetExamples()
        => null!; //TODO implement good example !
}
