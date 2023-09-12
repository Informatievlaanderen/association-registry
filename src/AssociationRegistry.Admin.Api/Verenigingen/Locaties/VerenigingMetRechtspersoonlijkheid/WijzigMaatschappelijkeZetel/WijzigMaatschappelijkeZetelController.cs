﻿namespace AssociationRegistry.Admin.Api.Verenigingen.Locaties.VerenigingMetRechtspersoonlijkheid.WijzigMaatschappelijkeZetel;

using Acties.WijzigLocatie;
using Acties.WijzigMaatschappelijkeZetel;
using Infrastructure;
using Infrastructure.Extensions;
using Infrastructure.Middleware;
using Infrastructure.Swagger;
using AssociationRegistry.Admin.Api.Verenigingen.Locaties.FeitelijkeVereniging.WijzigLocatie.RequestModels;
using Framework;
using Vereniging;
using Be.Vlaanderen.Basisregisters.Api;
using Be.Vlaanderen.Basisregisters.Api.Exceptions;
using Examples;
using FluentValidation;
using Infrastructure.ConfigurationBindings;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RequestModels;
using Swashbuckle.AspNetCore.Filters;
using System.Threading.Tasks;
using Wolverine;
using ProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ProblemDetails;
using ValidationProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ValidationProblemDetails;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[ApiRoute("verenigingen")]
[SwaggerGroup.DecentraalBeheer]
public class WijzigMaatschappelijkeZetelController : ApiController
{
    private readonly IMessageBus _messageBus;
    private readonly IValidator<WijzigMaatschappelijkeZetelRequest> _validator;
    private readonly AppSettings _appSettings;

    public WijzigMaatschappelijkeZetelController(
        IMessageBus messageBus,
        IValidator<WijzigMaatschappelijkeZetelRequest> validator,
        AppSettings appSettings)
    {
        _messageBus = messageBus;
        _validator = validator;
        _appSettings = appSettings;
    }

    /// <summary>
    ///     Verrijkt de maatschappelijke zetel volgens KBO.
    /// </summary>
    /// <remarks>
    ///     Na het uitvoeren van deze actie wordt een sequentie teruggegeven via de `VR-Sequence` header.
    ///     Deze waarde kan gebruikt worden in andere endpoints om op te volgen of de aanpassing
    ///     al is doorgestroomd naar deze endpoints.
    /// </remarks>
    /// <param name="vCode">De VCode van de vereniging</param>
    /// <param name="locatieId">De unieke identificatie code van de maatschappelijke zetel volgens KBO binnen de vereniging.</param>
    /// <param name="request">De te wijzigen gegevens</param>
    /// <param name="metadataProvider"></param>
    /// <param name="ifMatch">If-Match header met ETag van de laatst gekende versie van de vereniging.</param>
    /// <response code="200">Er waren geen wijzigingen.</response>
    /// <response code="202">De locatie werd verrijkt.</response>
    /// <response code="400">Er was een probleem met de doorgestuurde waarden.</response>
    /// <response code="412">De gevraagde vereniging heeft niet de verwachte sequentiewaarde.</response>
    /// <response code="500">Er is een interne fout opgetreden.</response>
    [HttpPatch("{vCode}/kbo/locaties/{locatieId}")]
    [Consumes("application/json")]
    [Produces("application/json")]
    [SwaggerResponseHeader(StatusCodes.Status202Accepted, WellknownHeaderNames.Sequence, "string", "Het sequence nummer van deze request.")]
    [SwaggerResponseHeader(StatusCodes.Status202Accepted, "ETag", "string", "De versie van de geregistreerde vereniging.")]
    [SwaggerRequestExample(typeof(WijzigLocatieRequest), typeof(WijzigMaatschappelijkeZetelRequestExamples))]
    [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ProblemAndValidationProblemDetailsExamples))]
    [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Patch(
        [FromRoute] string vCode,
        [FromRoute] int locatieId,
        [FromBody] WijzigMaatschappelijkeZetelRequest request,
        [FromServices] ICommandMetadataProvider metadataProvider,
        [FromHeader(Name = "If-Match")] string? ifMatch = null)
    {
        await _validator.NullValidateAndThrowAsync(request);

        var metaData = metadataProvider.GetMetadata(IfMatchParser.ParseIfMatch(ifMatch));
        var envelope = new CommandEnvelope<WijzigMaatschappelijkeZetelCommand>(request.ToCommand(vCode, locatieId), metaData);
        var commandResult = await _messageBus.InvokeAsync<CommandResult>(envelope);

        if (!commandResult.HasChanges()) return Ok();

        return this.AcceptedCommand(_appSettings, commandResult);
    }
}
