namespace AssociationRegistry.Test.E2E.Framework.AlbaHost;

using Newtonsoft.Json.Linq;
using Public.Api.WebApi.Verenigingen.Detail.ResponseModels;
using Public.Api.WebApi.Verenigingen.DetailAll;

public static class GetPubliekDetailAllExtensions
{
    public static IEnumerable<DetailAllConverter.TeVerwijderenVereniging> OnlyTeVerwijderen(this IEnumerable<JObject> source)
    {
        return source
              .Where(IsTeVerwijderen)
              .Select(x => x.ToObject<DetailAllConverter.TeVerwijderenVereniging>()!);
    }

    private static bool IsTeVerwijderen(JObject x)
        => ((JObject)x["vereniging"]).ContainsKey("teVerwijderen");

    public static PubliekVerenigingDetailResponse FindVereniging(this IEnumerable<JObject> source, string vCode)
    {
        return source
              .Where(x => !IsTeVerwijderen(x))
              .Select(x => x.ToObject<PubliekVerenigingDetailResponse>()!)
              .Single(x => x.Vereniging.VCode == vCode);
    }
}
