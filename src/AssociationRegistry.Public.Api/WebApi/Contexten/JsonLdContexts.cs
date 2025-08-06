namespace AssociationRegistry.Public.Api.WebApi.Contexten;

using AssociationRegistry.Public.Api.Infrastructure.Extensions;

public static class JsonLdContexts
{
    public static string GetContext(string folder, string name)
        => typeof(JsonLdContexts).Assembly.GetResourceString($"{typeof(JsonLdContexts).Namespace}.{folder.ToLowerInvariant()}.{name}");
}
