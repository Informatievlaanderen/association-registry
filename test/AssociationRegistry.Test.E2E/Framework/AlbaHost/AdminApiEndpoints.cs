namespace AssociationRegistry.Test.E2E.Framework.AlbaHost;

using Admin.Api.Administratie.DubbelControle;
using Admin.Api.Constants;
using Admin.Api.Verenigingen.Detail.ResponseModels;
using Admin.Api.Verenigingen.Historiek.ResponseModels;
using Admin.Api.Verenigingen.Registreer.FeitelijkeVereniging.RequetsModels;
using Admin.Api.Verenigingen.Search.ResponseModels;
using Alba;
using System.Net.Http.Json;

public static class AdminApiEndpoints
{
    public static HistoriekResponse GetBeheerHistoriek(this IAlbaHost source, string vCode)
        => source.GetAsJson<HistoriekResponse>(url: $"/v1/verenigingen/{vCode}/historiek")
                 .GetAwaiter().GetResult()!;

    public static string GetDetailAsText(this IAlbaHost source, string vCode)
        => source.GetAsText($"/v1/verenigingen/{vCode}").GetAwaiter().GetResult()!;

    public static DetailVerenigingResponse GetBeheerDetail(this IAlbaHost source, string vCode)
        => source.GetAsJson<DetailVerenigingResponse>($"/v1/verenigingen/{vCode}").GetAwaiter().GetResult()!;

    public static DetailVerenigingResponse GetBeheerDetailWithHeader(this IAlbaHost source, string vCode)
    {
        var client = source.Server.CreateClient();
        client.DefaultRequestHeaders.Add(VersionHeader.HeaderName, VersionHeader.V2);
        var response = client.GetAsync($"/v1/verenigingen/{vCode}").GetAwaiter().GetResult();

        return response.Content.ReadFromJsonAsync<DetailVerenigingResponse>().GetAwaiter().GetResult();
    }

    public static DubbelControleResponse[] PostDubbelControle(this IAlbaHost source, RegistreerFeitelijkeVerenigingRequest registreerFeitelijkeVerenigingRequest)
        => source.PostJson<RegistreerFeitelijkeVerenigingRequest>(registreerFeitelijkeVerenigingRequest,
            "/v1/admin/dubbelcontrole").Receive<DubbelControleResponse[]>().GetAwaiter().GetResult();

    public static SearchVerenigingenResponse GetBeheerZoeken(this IAlbaHost source, string query)
        => source.GetAsJson<SearchVerenigingenResponse>($"/v1/verenigingen/zoeken?q={query}").GetAwaiter().GetResult()!;
}
