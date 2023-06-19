namespace AssociationRegistry.Public.Api.Hoofdactiviteiten;

using System.Linq;
using Be.Vlaanderen.Basisregisters.Api;
using Be.Vlaanderen.Basisregisters.Api.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;
using Vereniging;
using ProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ProblemDetails;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[ApiRoute("hoofdactiviteitenVerenigingsloket")]
[ApiExplorerSettings(GroupName = "Parameters")]
public class HoofdactiviteitenVerenigingsloketController : ApiController
{
    /// <summary>
    ///     Vraag alle mogelijke waarden op voor de hoofdactiviteiten.
    /// </summary>
    /// <response code="200">De gekende waarden voor hoofdactiviteit</response>
    /// <response code="500">Er is een interne fout opgetreden.</response>
    [HttpGet("")]
    [ProducesResponseType(typeof(HoofdactiviteitenHoofdactiviteitenVerenigingsloketResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(HoofdactiviteitenHoofdactiviteitenVerenigingsloketResponseExamples))]
    [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
    public IActionResult GetAll()
        => Ok(
            new HoofdactiviteitenHoofdactiviteitenVerenigingsloketResponse
            {
                HoofdactiviteitenVerenigingsloket = HoofdactiviteitVerenigingsloket.All().Select(ToDto).ToArray(),
            }
        );

    private static HoofdactiviteitenHoofdactiviteitenVerenigingsloketResponse.HoofdactiviteitVerenigingsloket ToDto(HoofdactiviteitVerenigingsloket arg)
        => new()
        {
            Code = arg.Code,
            Beschrijving = arg.Beschrijving,
        };
}
