namespace AssociationRegistry.Admin.Api.Verenigingen.Registreer;

using System.Linq;
using System.Threading.Tasks;
using Constants;
using Infrastructure.ConfigurationBindings;
using Infrastructure.Extensions;
using Framework;
using Vereniging;
using Be.Vlaanderen.Basisregisters.Api;
using Be.Vlaanderen.Basisregisters.Api.Exceptions;
using FluentValidation;
using Historiek;
using Infrastructure;
using Marten;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NodaTime;
using Projections.Historiek;
using Swashbuckle.AspNetCore.Filters;
using ProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ProblemDetails;
using ValidationProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ValidationProblemDetails;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[ApiRoute("verenigingen")]
[ApiExplorerSettings(GroupName = "Verenigingen")]
public class RegistreerVerenigingController : ApiController
{
    private readonly ISender _sender;
    private readonly AppSettings _appSettings;

    public RegistreerVerenigingController(ISender sender, AppSettings appSettings)
    {
        _sender = sender;
        _appSettings = appSettings;
    }

    /// <summary>
    /// Registreer een vereniging.
    /// </summary>
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
        var registratieResult = await _sender.Send(envelope);

        return this.AcceptedRegistratie(_appSettings, registratieResult);
    }
}
