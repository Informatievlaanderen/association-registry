namespace AssociationRegistry.Test.E2E.Framework.AlbaHost;

using Admin.Api.Administratie.DubbelControle;
using Admin.Api.Constants;
using Admin.Api.Infrastructure;
using Admin.Api.Verenigingen.Detail.ResponseModels;
using Admin.Api.Verenigingen.Historiek.ResponseModels;
using Admin.Api.Verenigingen.Registreer.FeitelijkeVereniging.RequetsModels;
using Admin.Api.Verenigingen.Search.ResponseModels;
using Alba;
using JasperFx.Core;
using Newtonsoft.Json;
using System.Net;
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

    public static async  Task<DetailVerenigingResponse> GetBeheerDetailWithHeader(
        this IAlbaHost source,
        HttpClient authenticatedClient,
        string vCode,
        long? expectedSequence)
    {
        var client = source.Server.CreateClient();

        foreach (var defaultRequestHeader in authenticatedClient.DefaultRequestHeaders)
        {
            client.DefaultRequestHeaders.Add(defaultRequestHeader.Key, defaultRequestHeader.Value);
        }

        client.DefaultRequestHeaders.Add(WellknownHeaderNames.Version, WellknownVersions.V2);

        var response = await client.GetAsync($"/v1/verenigingen/{vCode}?expectedSequence={expectedSequence}");

        while (response.StatusCode == HttpStatusCode.PreconditionFailed)
        {
            await Task.Delay(300);
            response = await client.GetAsync($"/v1/verenigingen/{vCode}?expectedSequence={expectedSequence}");
        }

        var readAsStringAsync = await response.Content.ReadAsStringAsync();

        return JsonConvert.DeserializeObject<DetailVerenigingResponse>(readAsStringAsync);
    }

    public static DubbelControleResponse[] PostDubbelControle(this IAlbaHost source, RegistreerFeitelijkeVerenigingRequest registreerFeitelijkeVerenigingRequest)
        => source.PostJson<RegistreerFeitelijkeVerenigingRequest>(registreerFeitelijkeVerenigingRequest,
            "/v1/admin/dubbelcontrole").Receive<DubbelControleResponse[]>().GetAwaiter().GetResult();

    public static SearchVerenigingenResponse GetBeheerZoeken(this IAlbaHost source, string query)
        => source.GetAsJson<SearchVerenigingenResponse>($"/v1/verenigingen/zoeken?q={query}").GetAwaiter().GetResult()!;

    public static async  Task<SearchVerenigingenResponse> GetBeheerZoekenV2(
        this IAlbaHost source,
        HttpClient authenticatedClient,
        string query)
    {
        var client = source.Server.CreateClient();

        foreach (var defaultRequestHeader in authenticatedClient.DefaultRequestHeaders)
        {
            client.DefaultRequestHeaders.Add(defaultRequestHeader.Key, defaultRequestHeader.Value);
        }

        client.DefaultRequestHeaders.Add(WellknownHeaderNames.Version, WellknownVersions.V2);

        var response = await client.GetAsync($"/v1/verenigingen/zoeken?q={query}");

        while (response.StatusCode == HttpStatusCode.PreconditionFailed)
        {
            await Task.Delay(300);
            response = await client.GetAsync($"/v1/verenigingen/zoeken?q={query}");
        }

        var readAsStringAsync = await response.Content.ReadAsStringAsync();

        return JsonConvert.DeserializeObject<SearchVerenigingenResponse>(readAsStringAsync);
    }
}
