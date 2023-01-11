namespace AssociationRegistry.Acm.Api.Infrastructure.Extentions;

using Microsoft.Extensions.Configuration;

public static class ConfigurationExtensions
{

    public static string GetBaseUrl(this IConfiguration configuration)
        => configuration.GetValue<string>("BaseUrl").TrimEnd('/');
}
