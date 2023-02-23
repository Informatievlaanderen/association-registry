namespace AssociationRegistry.Admin.Api.Infrastructure.Extensions;

using Vereniging;
using Be.Vlaanderen.Basisregisters.Api;
using ConfigurationBindings;
using Microsoft.AspNetCore.Mvc;

public static class ControllerExtensions
{
    public static AcceptedResult AcceptedCommand(this ApiController source, AppSettings appSettings, CommandResult commandResult)
    {
        source.Response.AddSequenceHeader(commandResult.Sequence);
        source.Response.AddETagHeader(commandResult.Version);

        return source.Accepted($"{appSettings.BaseUrl}/v1/verenigingen/{commandResult.Vcode}");
    }
}
