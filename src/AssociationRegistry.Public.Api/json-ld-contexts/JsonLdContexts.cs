namespace AssociationRegistry.Public.Api.json_ld_contexts;

using System;
using DetailVerenigingen;
using Extensions;
using ListVerenigingen;
using Newtonsoft.Json;

public static class JsonLdContexts
{
    public static DetailVerenigingContext GetDetailVerenigingContext()
        => JsonConvert.DeserializeObject<DetailVerenigingContext>(GetContext("detail-vereniging-context.json"))
           ?? throw new NullReferenceException("DetailVerenigingContext is null");

    public static ListVerenigingContext GetListVerenigingenContext()
        => JsonConvert.DeserializeObject<ListVerenigingContext>(GetContext("list-verenigingen-context.json"))
           ?? throw new NullReferenceException("ListVerenigingContext is null");

    public static string GetContext(string name)
        => typeof(JsonLdContexts).Assembly.GetResourceString($"{typeof(JsonLdContexts).Namespace}.{name}");
}
