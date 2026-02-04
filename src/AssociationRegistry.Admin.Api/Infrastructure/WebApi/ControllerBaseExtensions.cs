namespace AssociationRegistry.Admin.Api.WebApi.Verenigingen.Extensions;

using Be.Vlaanderen.Basisregisters.Api;
using DecentraalBeheer.Vereniging;
using Hosts.Configuration.ConfigurationBindings;
using Infrastructure.WebApi;
using Microsoft.AspNetCore.Mvc;

public static class ControllerBaseExtensions
{
    public static IActionResult PatchResponse(this ApiController controller, CommandResult commandResult)
    {
        if (!commandResult.HasChanges())
            return controller.Ok();

        controller.Response.AddSequenceHeader(commandResult.Sequence);
        controller.Response.AddETagHeader(commandResult.Version);

        return controller.Accepted();
    }

    public static IActionResult DeleteResponse(this ApiController controller, CommandResult commandResult)
    {
        controller.Response.AddSequenceHeader(commandResult.Sequence);
        controller.Response.AddETagHeader(commandResult.Version);

        return controller.Accepted();
    }

    public static IActionResult PostResponse(this ApiController controller, CommandResult commandResult)
    {
        if (!commandResult.HasChanges())
            return controller.Ok();

        controller.Response.AddSequenceHeader(commandResult.Sequence);
        controller.Response.AddETagHeader(commandResult.Version);

        return controller.Accepted();
    }

    public static AcceptedResult PostResponse(
        this ApiController controller,
        AppSettings appSettings,
        string entityName,
        EntityCommandResult entityCommandResult
    )
    {
        controller.Response.AddSequenceHeader(entityCommandResult.Sequence);
        controller.Response.AddETagHeader(entityCommandResult.Version);

        return controller.Accepted(
            $"{appSettings.BaseUrl}/v1/verenigingen/{entityCommandResult.Vcode}/{entityName}/{entityCommandResult.EntityId}"
        );
    }
}
