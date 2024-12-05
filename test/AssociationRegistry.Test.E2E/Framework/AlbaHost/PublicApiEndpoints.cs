namespace AssociationRegistry.Test.E2E.Framework.AlbaHost;

using Alba;
using Newtonsoft.Json.Linq;
using Public.Api.Verenigingen.Detail.ResponseModels;
using Public.Api.Verenigingen.Search.ResponseModels;
using Public.Api.Werkingsgebieden.ResponseModels;

public static class PublicApiEndpoints
{
    public static PubliekVerenigingDetailResponse GetPubliekDetail(this IAlbaHost source, string vCode)
        => source.GetAsJson<PubliekVerenigingDetailResponse>($"/v1/verenigingen/{vCode}").GetAwaiter().GetResult()!;

    public static SearchVerenigingenResponse GetPubliekZoeken(this IAlbaHost source, string query)
        => source.GetAsJson<SearchVerenigingenResponse>($"/v1/verenigingen/zoeken?q={query}").GetAwaiter().GetResult()!;

    public static WerkingsgebiedenResponse GetWerkingsgebieden(this IAlbaHost source)
        => source.GetAsJson<WerkingsgebiedenResponse>($"/v1/werkingsgebieden").GetAwaiter().GetResult()!;

    public static JObject[] GetPubliekDetailAll(this IAlbaHost source)
    {
        var locationHeader = GetDetailAllLocationHeader(source);
        using var handler = new HttpClientHandler();
        using var s3HttpClient = new HttpClient(handler);

        handler.ServerCertificateCustomValidationCallback = (_, _, _, _) => true;

        try
        {
            var s3Response = s3HttpClient.GetAsync(locationHeader).GetAwaiter().GetResult();

            var responseContent = s3Response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

            return ParseResponse(responseContent);
        }
        catch (Exception e)
        {
            throw new Exception($"{e.Message}: (locationheader: {locationHeader}, handler baseaddress {s3HttpClient.BaseAddress}.");
        }
    }

    private static JObject[] ParseResponse(string responseContent)
    {
        if (string.IsNullOrEmpty(responseContent))
            return [];

        var verenigingen = new List<JObject>();
        var reader = new StringReader(responseContent);

        while (reader.Peek() != -1)
        {
            var line = reader.ReadLine();
            verenigingen.Add(JObject.Parse(line));
        }

        return verenigingen.ToArray();
    }

    private static Uri? GetDetailAllLocationHeader(IAlbaHost source)
    {
        var client = source.Server.CreateClient();
        var response = client.GetAsync($"/v1/verenigingen/detail/all").GetAwaiter().GetResult();

        return response.Headers.Location;
    }
}
