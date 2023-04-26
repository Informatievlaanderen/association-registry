namespace AssociationRegistry.Admin.Api.Verenigingen.Vertegenwoordigers.VerwijderVertegenwoordiger;

using System.Threading.Tasks;
using Acties.VerwijderVertegenwoordiger;
using Be.Vlaanderen.Basisregisters.Api;
using Be.Vlaanderen.Basisregisters.Api.Exceptions;
using Framework;
using Infrastructure;
using Infrastructure.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NodaTime;
using Swashbuckle.AspNetCore.Filters;
using Vereniging;
using Wolverine;
using ProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ProblemDetails;
using ValidationProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ValidationProblemDetails;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[ApiRoute("verenigingen")]
[ApiExplorerSettings(GroupName = "Vereniging Vertegenwoordigers")]
public class VerwijderVertegenwoordigerController : ApiController
{
    private readonly IMessageBus _messageBus;

    public VerwijderVertegenwoordigerController(IMessageBus messageBus)
    {
        _messageBus = messageBus;
    }
    /// <summary>
    /// Verwijder een vertegenwoordiger.
    /// </summary>
    /// <remarks>
    /// Na het uitvoeren van deze actie wordt een sequentie teruggegeven via de `VR-Sequence` header.
    /// Deze waarde kan gebruikt worden in andere endpoints om op te volgen of de aanpassing
    /// al is doorgestroomd naar deze endpoints.
    /// </remarks>
    /// /// <param name="request"></param>
    /// <param name="vCode">De vCode van de vereniging</param>
    /// <param name="vertegenwoordigerId">De unieke identificatie code van deze vertegenwoordiger die verwijderd moet worden</param>
    /// <param name="ifMatch">If-Match header met ETag van de laatst gekende versie van de vereniging.</param>
    /// <response code="202">De vertegenwoordiger werd verwijderd van deze vereniging.</response>
    /// <response code="400">Er is een probleem met de doorgestuurde waarden. Zie body voor meer info.</response>
    /// <response code="500">Als er een interne fout is opgetreden.</response>
    [HttpDelete("{vCode}/vertegenwoordigers/{vertegenwoordigerId:int}")]
    [Consumes("application/json")]
    [Produces("application/json")]
    [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
    [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ValidationProblemDetailsExamples))]
    [SwaggerResponseHeader(StatusCodes.Status202Accepted, WellknownHeaderNames.Sequence, "string", "Het sequence nummer van deze request.")]
    [SwaggerResponseHeader(StatusCodes.Status202Accepted, "ETag", "string", "De versie van de geregistreerde vereniging.")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status202Accepted)]
    public async Task<IActionResult> Delete(
        [FromRoute] string vCode,
        [FromRoute] int vertegenwoordigerId,
        [FromBody] VerwijderVertegenwoordigerRequest request,
        [FromHeader(Name = "If-Match")] string? ifMatch = null)
    {
        var metaData = new CommandMetadata(request.Initiator, SystemClock.Instance.GetCurrentInstant(), IfMatchParser.ParseIfMatch(ifMatch));
        var envelope = new CommandEnvelope<VerwijderVertegenwoordigerCommand>(request.ToCommand(vCode, vertegenwoordigerId), metaData);

        var commandResult = await _messageBus.InvokeAsync<CommandResult>(envelope);

        Response.AddSequenceHeader(commandResult.Sequence);
        Response.AddETagHeader(commandResult.Version);
        return Accepted();
    }
}
