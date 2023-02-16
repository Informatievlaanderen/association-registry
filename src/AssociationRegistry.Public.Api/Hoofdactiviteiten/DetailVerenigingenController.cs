namespace AssociationRegistry.Public.Api.Hoofdactiviteiten;

using System.Linq;
using AssociationRegistry.Hoofdactiviteiten;
using Be.Vlaanderen.Basisregisters.Api;
using Be.Vlaanderen.Basisregisters.Api.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[ApiRoute("hoofdactiviteiten")]
[ApiExplorerSettings(GroupName = "Hoofdactiviteiten")]
public class HoofdactiviteitenController : ApiController
{
    /// <summary>
    /// Vraag alle mogelijke waarden op voor de hoofdactiviteiten.
    /// </summary>
    /// <response code="200">De gekende waarden voor hoofdactiviteit</response>
    /// <response code="500">Als er een interne fout is opgetreden.</response>
    [HttpGet("")]
    [ProducesResponseType(typeof(HoofdactiviteitenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(HoofdactiviteitenResponseExamples))]
    [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
    public IActionResult GetAll()
        => Ok(
            new HoofdactiviteitenResponse(
                HoofdactiviteitVerenigingsloket.All().Select(ToDto).ToArray()
            )
        );

    private static HoofdactiviteitenResponse.Hoofdactiviteit ToDto(HoofdactiviteitVerenigingsloket arg)
        => new(arg.Code, arg.Beschrijving);
}
