namespace AssociationRegistry.Admin.Api;

using Be.Vlaanderen.Basisregisters.AspNetCore.Mvc.Middleware;
using Microsoft.Net.Http.Headers;

public static class StartupConstants
{
    public const string AllowAnyOrigin = "AllowAnyOrigin";
    public const string AllowSpecificOrigin = "AllowSpecificOrigin";
    public const string DatabaseTag = "db";
    public const string Culture = "en-GB";

    public static readonly string[] ExposedHeaders =
    {
        HeaderNames.Location, AddVersionHeaderMiddleware.HeaderName,
        AddCorrelationIdToResponseMiddleware.HeaderName,
        AddHttpSecurityHeadersMiddleware.PoweredByHeaderName,
        AddHttpSecurityHeadersMiddleware.ContentTypeOptionsHeaderName,
        AddHttpSecurityHeadersMiddleware.FrameOptionsHeaderName,
        AddHttpSecurityHeadersMiddleware.XssProtectionHeaderName,
    };

    public static readonly string[] Headers =
    {
        HeaderNames.Accept,
        HeaderNames.ContentType,
        HeaderNames.Origin,
        HeaderNames.Authorization,
        HeaderNames.IfMatch,
    };

    public static string[] HttpMethodsAsString
        => new[]
        {
            HttpMethod.Get,
            HttpMethod.Head,
            HttpMethod.Post,
            HttpMethod.Put,
            HttpMethod.Patch,
            HttpMethod.Delete,
            HttpMethod.Options,
        }.Select(m => m.Method).ToArray();
}
