namespace AssociationRegistry.Public.Api.json_ld_contexts;

using System;
using DetailVerenigingen;
using Extensions;
using ListVerenigingen;
using Newtonsoft.Json;

public static class JsonLdContexts
{
    public static DetailVerenigingContext GetDetailVerenigingContext()
    {
        var jsonLdContextsType = typeof(JsonLdContexts);
        var json = jsonLdContextsType.Assembly.GetResourceString($"{jsonLdContextsType.Namespace}.detail-vereniging-context.json");

        return JsonConvert.DeserializeObject<DetailVerenigingContext>(json) ?? throw new NullReferenceException("DetailVerenigingContext is null");
    }

    public static ListVerenigingContext GetListVerenigingenContext()
    {
        var jsonLdContextsType = typeof(JsonLdContexts);
        var json = jsonLdContextsType.Assembly.GetResourceString($"{jsonLdContextsType.Namespace}.list-verenigingen-context.json");

        return JsonConvert.DeserializeObject<ListVerenigingContext>(json) ?? throw new NullReferenceException("ListVerenigingContext is null");
    }
}
