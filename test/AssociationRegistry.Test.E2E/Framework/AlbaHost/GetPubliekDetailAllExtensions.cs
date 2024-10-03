namespace AssociationRegistry.Test.E2E.Framework.AlbaHost;

using Newtonsoft.Json.Linq;
using Public.Api.Verenigingen.Detail;
using Public.Api.Verenigingen.Detail.ResponseModels;

public static class GetPubliekDetailAllExtensions
{
    public static IEnumerable<ResponseWriter.TeVerwijderenVereniging> OnlyTeVerwijderen(this IEnumerable<JObject> source)
    {
        return source
              .Where(IsTeVerwijderen)
              .Select(x => x.ToObject<ResponseWriter.TeVerwijderenVereniging>()!);
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
