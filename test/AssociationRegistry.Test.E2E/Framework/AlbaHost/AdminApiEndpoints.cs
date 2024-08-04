namespace AssociationRegistry.Test.E2E.Framework.AlbaHost;

using Admin.Api.Verenigingen.Detail.ResponseModels;
using Alba;
using AssociationRegistry.Admin.Api.Verenigingen.Historiek.ResponseModels;
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
}

