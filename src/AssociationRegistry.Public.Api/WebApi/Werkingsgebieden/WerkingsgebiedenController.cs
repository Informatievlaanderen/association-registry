namespace AssociationRegistry.Public.Api.WebApi.Werkingsgebieden;

using Asp.Versioning;
using AssociationRegistry.Public.Api.Constants;
using AssociationRegistry.Vereniging;
using Be.Vlaanderen.Basisregisters.Api;
using Be.Vlaanderen.Basisregisters.Api.Exceptions;
using Microsoft.AspNetCore.Mvc;
using ResponseExamples;
using ResponseModels;
using Swashbuckle.AspNetCore.Filters;
using ProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ProblemDetails;
using Werkingsgebied = ResponseModels.Werkingsgebied;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[ApiRoute("werkingsgebieden")]
[ApiExplorerSettings(GroupName = "Parameters")]
public class WerkingsgebiedenController : ApiController
{
    /// <summary>
    ///     Vraag alle mogelijke waarden op voor de werkingsgebieden.
    /// </summary>
    /// <response code="200">Een lijst met alle gekende werkingsgebieden.</response>
    /// <response code="500">Er is een interne fout opgetreden.</response>
    [HttpGet]
    [ProducesResponseType(typeof(WerkingsgebiedenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(WerkingsgebiedenResponseExamples))]
    [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
    [Produces(WellknownMediaTypes.JsonLd)]
    public IActionResult GetWerkingsgebieden([FromServices] WerkingsgebiedenService werkingsgebiedenService)
    {
        var werkingsgebieden = werkingsgebiedenService
                              .AllWithNVT()
                              .Select(wg => new Werkingsgebied
                               {
                                   Code = wg.Code,
                                   Naam = wg.Naam,
                               })
                              .ToArray();

        return Ok(new WerkingsgebiedenResponse { Werkingsgebieden = werkingsgebieden });
    }
}
