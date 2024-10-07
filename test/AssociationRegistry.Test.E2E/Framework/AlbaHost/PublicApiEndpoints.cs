namespace AssociationRegistry.Test.E2E.Framework.AlbaHost;

using Alba;
using Newtonsoft.Json.Linq;
using Public.Api.Verenigingen.Detail.ResponseModels;
using Public.Api.Verenigingen.Search.ResponseModels;

public static class PublicApiEndpoints
{
    public static PubliekVerenigingDetailResponse GetPubliekDetail(this IAlbaHost source, string vCode)
        => source.GetAsJson<PubliekVerenigingDetailResponse>($"/v1/verenigingen/{vCode}").GetAwaiter().GetResult()!;

    public static SearchVerenigingenResponse GetPubliekZoeken(this IAlbaHost source, string query)
        => source.GetAsJson<SearchVerenigingenResponse>($"/v1/verenigingen/zoeken?q={query}").GetAwaiter().GetResult()!;

    public static JObject[] GetPubliekDetailAll(this IAlbaHost source)
    {
        // Watch out, we only receive the first vereniging due to streaming
        var response = source.GetAsText($"/v1/verenigingen/detail/all").GetAwaiter().GetResult()!;

        if (string.IsNullOrEmpty(response))
            return [];

        var verenigingen = new List<JObject>();
        var reader = new StringReader(response);
        while (reader.Peek() != -1)
        {
            var line = reader.ReadLine();
            verenigingen.Add(JObject.Parse(line));
        }

        return verenigingen.ToArray();
    }
}
