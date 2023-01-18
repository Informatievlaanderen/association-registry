namespace AssociationRegistry.Admin.Api.Verenigingen.Registreer;

using System.Threading.Tasks;
using Infrastructure.ConfigurationBindings;
using Infrastructure.Extensions;
using Framework;
using Vereniging;
using Be.Vlaanderen.Basisregisters.Api;
using Be.Vlaanderen.Basisregisters.Api.Exceptions;
using FluentValidation;
using Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NodaTime;
using Swashbuckle.AspNetCore.Filters;
using Vereniging.RegistreerVereniging;
using Wolverine;
using ProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ProblemDetails;
using ValidationProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ValidationProblemDetails;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[ApiRoute("verenigingen")]
[ApiExplorerSettings(GroupName = "Verenigingen")]
public class RegistreerVerenigingController : ApiController
{
    private readonly IMessageBus _bus;
    private readonly AppSettings _appSettings;

    public RegistreerVerenigingController(IMessageBus bus, AppSettings appSettings)
    {
        _bus = bus;
        _appSettings = appSettings;
    }

    /// <summary>
    /// Registreer een vereniging.
    /// </summary>
    /// <remarks>
    /// Bij het registreren van de vereniging wordt een sequentie teruggegeven via de `VR-Sequence` header.
    /// Deze waarde kan gebruikt worden in andere endpoints om op te volgen of de zonet geregistreerde vereniging
    /// al is doorgestroomd naar deze endpoints.
    /// </remarks>
    [HttpPost]
    [Consumes("application/json")]
    [Produces("application/json")]
    [SwaggerRequestExample(typeof(RegistreerVerenigingRequest), typeof(RegistreerVerenigingenRequestExamples))]
    [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
    [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ValidationProblemDetailsExamples))]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<AcceptedResult> Post(
        [FromServices] IValidator<RegistreerVerenigingRequest> validator,
        [FromBody] RegistreerVerenigingRequest request)
    {
        await DefaultValidatorExtensions.ValidateAndThrowAsync(validator, request);

        var command = request.ToRegistreerVerenigingCommand();

        var metaData = new CommandMetadata(request.Initiator, SystemClock.Instance.GetCurrentInstant());
        var envelope = new CommandEnvelope<RegistreerVerenigingCommand>(command, metaData);
        var registratieResult = await _bus.InvokeAsync<CommandResult>(envelope);

        return this.AcceptedCommand(_appSettings, registratieResult);
    }
}
