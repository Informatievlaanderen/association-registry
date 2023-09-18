namespace AssociationRegistry.Public.Api.Contexten;

using Infrastructure.Extensions;

public static class JsonLdContexts
{
    public static string GetContext(string folder, string name)
        => typeof(JsonLdContexts).Assembly.GetResourceString($"{typeof(JsonLdContexts).Namespace}.{folder.ToLowerInvariant()}.{name}");
}
