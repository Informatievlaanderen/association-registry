using System.Threading.Tasks;
using AssociationRegistry.Public.Api.ListVerenigingen;
using Be.Vlaanderen.Basisregisters.Api;
using Be.Vlaanderen.Basisregisters.Api.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;

namespace AssociationRegistry.Public.Api.DetailVerenigingen;

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

        return Ok(new DetailVerenigingResponse(detailVerenigingContext, vereniging));
    }
}

public class DetailVerenigingResponseExamples : IExamplesProvider<DetailVerenigingResponse>
{
    public DetailVerenigingResponse GetExamples() => null!; //TODO implement good example !
}
