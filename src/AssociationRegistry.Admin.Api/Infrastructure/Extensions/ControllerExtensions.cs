namespace AssociationRegistry.Admin.Api.Infrastructure.Extensions;

using Be.Vlaanderen.Basisregisters.Api;
using Hosts.Configuration.ConfigurationBindings;
using Microsoft.AspNetCore.Mvc;
using ResponseWriter;
using Vereniging;

public static class ControllerExtensions
{
    public static AcceptedResult AcceptedCommand(this ApiController source, AppSettings appSettings, CommandResult commandResult)
    {
        source.Response.AddSequenceHeader(commandResult.Sequence);
        source.Response.AddETagHeader(commandResult.Version);

        return source.Accepted($"{appSettings.BaseUrl}/v1/verenigingen/{commandResult.Vcode}");
    }

    public static AcceptedResult AcceptedEntityCommand(this ApiController source, AppSettings appSettings, string entityName, EntityCommandResult entityCommandResult)
    {
        source.Response.AddSequenceHeader(entityCommandResult.Sequence);
        source.Response.AddETagHeader(entityCommandResult.Version);

        return source.Accepted($"{appSettings.BaseUrl}/v1/verenigingen/{entityCommandResult.Vcode}/{entityName}/{entityCommandResult.EntityId}");
    }

    public static AcceptedResult AcceptedEntityCommand(this ApiController source, IResponseWriter responseWriter, AppSettings appSettings, string entityName, EntityCommandResult entityCommandResult)
    {
        responseWriter.AddSequenceHeader(source.Response, entityCommandResult.Sequence);
        responseWriter.AddETagHeader(source.Response, entityCommandResult.Version);

        return source.Accepted($"{appSettings.BaseUrl}/v1/verenigingen/{entityCommandResult.Vcode}/{entityName}/{entityCommandResult.EntityId}");
    }
}
