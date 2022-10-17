namespace AssociationRegistry.Public.Api.json_ld_contexts;

using Extensions;

public static class JsonLdContexts
{
    public static string GetContext(string name)
        => typeof(JsonLdContexts).Assembly.GetResourceString($"{typeof(JsonLdContexts).Namespace}.{name}");
}
