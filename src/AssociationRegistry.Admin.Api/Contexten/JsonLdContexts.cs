namespace AssociationRegistry.Admin.Api.Contexten;

using Infrastructure.Extensions;

public static class JsonLdContexts
{
    public static string? GetContext(string name)
    {
        try
        {
            return typeof(JsonLdContexts).Assembly.GetResourceString($"{typeof(JsonLdContexts).Namespace}.{name}");
        }
        catch
        {
            return null;
        }
    }
}
