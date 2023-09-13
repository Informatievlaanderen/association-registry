namespace AssociationRegistry.Admin.Api.Hoofdactiviteiten;

using System.Linq;
using Vereniging;
using Be.Vlaanderen.Basisregisters.Api;
using Be.Vlaanderen.Basisregisters.Api.Exceptions;
using Examples;
using Infrastructure;
using Infrastructure.Swagger;
using Infrastructure.Swagger.Annotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ResponseModels;
using Swashbuckle.AspNetCore.Filters;
using ProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ProblemDetails;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[ApiRoute("hoofdactiviteitenVerenigingsloket")]
[SwaggerGroup.Parameters]
public class HoofdactiviteitenVerenigingsloketController : ApiController
{
    /// <summary>
    ///     Vraag alle mogelijke waarden op voor de hoofdactiviteiten.
    /// </summary>
    /// <response code="200">De gekende waarden voor hoofdactiviteit</response>
    /// <response code="400">Er was een probleem met de doorgestuurde waarden.</response>
    /// <response code="500">Er is een interne fout opgetreden.</response>
    [HttpGet("")]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(HoofdactiviteitenHoofdactiviteitenVerenigingsloketResponseExamples))]
    [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ProblemDetailsExamples))]
    [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
    [ProducesResponseType(typeof(HoofdactiviteitenVerenigingsloketResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public IActionResult GetAll()
        => Ok(
            new HoofdactiviteitenVerenigingsloketResponse
            {
                HoofdactiviteitenVerenigingsloket = HoofdactiviteitVerenigingsloket.All().Select(ToDto).ToArray(),
            }
        );

    private static HoofdactiviteitVerenigingsloketResponse ToDto(HoofdactiviteitVerenigingsloket arg)
        => new()
        {
            Code = arg.Code,
            Beschrijving = arg.Beschrijving,
        };
}
