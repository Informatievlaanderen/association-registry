namespace AssociationRegistry.Public.Api.json_ld_contexts;

using System;
using DetailVerenigingen;
using Extensions;
using ListVerenigingen;
using Newtonsoft.Json;

public static class JsonLdContexts
{
    public static string GetContext(string name)
        => typeof(JsonLdContexts).Assembly.GetResourceString($"{typeof(JsonLdContexts).Namespace}.{name}");
}
