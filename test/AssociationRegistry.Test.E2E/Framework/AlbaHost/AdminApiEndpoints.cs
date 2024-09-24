namespace AssociationRegistry.Test.E2E.Framework.AlbaHost;

using Admin.Api.Verenigingen.Detail.ResponseModels;
using Admin.Api.Verenigingen.Historiek.ResponseModels;
using Alba;
using Newtonsoft.Json;
using Public.Api.Verenigingen.Detail.ResponseModels;

public static class AdminApiEndpoints
{
    public static HistoriekResponse GetHistoriek(this IAlbaHost source, string vCode)
        => source.GetAsJson<HistoriekResponse>(url: $"/v1/verenigingen/{vCode}/historiek")
                 .GetAwaiter().GetResult()!;

    public static string GetDetailAsText(this IAlbaHost source, string vCode)
        => source.GetAsText($"/v1/verenigingen/{vCode}").GetAwaiter().GetResult()!;

    public static DetailVerenigingResponse GetDetail(this IAlbaHost source, string vCode)
        => source.GetAsJson<DetailVerenigingResponse>($"/v1/verenigingen/{vCode}").GetAwaiter().GetResult()!;
}

public static class PublicApiEndpoints
{
    public static PubliekVerenigingDetailResponse GetPubliekDetail(this IAlbaHost source, string vCode)
        => source.GetAsJson<PubliekVerenigingDetailResponse>($"/v1/verenigingen/{vCode}").GetAwaiter().GetResult()!;

    public static PubliekVerenigingDetailResponse[] GetPubliekDetailAll(this IAlbaHost source)
    {
        // Watch out, we only receive the first vereniging due to streaming
        var response = source.GetAsText($"/v1/verenigingen/detail/all").GetAwaiter().GetResult()!;
        if (string.IsNullOrEmpty(response))
            return Array.Empty<PubliekVerenigingDetailResponse>();

        return [JsonConvert.DeserializeObject<PubliekVerenigingDetailResponse>(response)];
    }
}
