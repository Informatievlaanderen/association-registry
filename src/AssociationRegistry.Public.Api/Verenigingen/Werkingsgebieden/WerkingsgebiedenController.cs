namespace AssociationRegistry.Public.Api.Verenigingen.Werkingsgebieden;

using Asp.Versioning;
using Be.Vlaanderen.Basisregisters.Api;
using Be.Vlaanderen.Basisregisters.Api.Exceptions;
using Constants;
using Detail;
using Detail.ResponseModels;
using Infrastructure.ConfigurationBindings;
using Marten;
using Microsoft.AspNetCore.Mvc;
using Queries;
using ResponseExamples;
using ResponseModels;
using Swashbuckle.AspNetCore.Filters;
using Vereniging;
using ProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ProblemDetails;
using Werkingsgebied = ResponseModels.Werkingsgebied;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[ApiRoute("werkingsgebieden")]
[ApiExplorerSettings(GroupName = "Opvragen van de werkingsgebieden van de verenigingen")]
public class WerkingsgebiedenController : ApiController
{
    private readonly AppSettings _appsettings;

    public WerkingsgebiedenController(AppSettings appsettings)
    {
        _appsettings = appsettings;
    }

    /// <summary>
    ///     Vraag alle werkingsgebieden op.
    /// </summary>
    /// <response code="200">Een lijst met alle gekende werkingsgebieden.</response>
    /// <response code="500">Er is een interne fout opgetreden.</response>
    [HttpGet]
    [ProducesResponseType(typeof(WerkingsgebiedenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(WerkingsgebiedenResponseExamples))]
    [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
    [Produces(WellknownMediaTypes.JsonLd)]
    public IActionResult GetWerkingsgebieden()
    {
        var werkingsgebieden = AssociationRegistry.Vereniging.Werkingsgebied
                                                  .All
                                                  .Select(wg => new Werkingsgebied
                                                   {
                                                       Code = wg.Code,
                                                       Naam = wg.Naam,
                                                   })
                                                  .ToArray();

        return Ok(new WerkingsgebiedenResponse { Werkingsgebieden = werkingsgebieden });
    }
}
