namespace AssociationRegistry.Test.E2E.Framework.AlbaHost;

using Acm.Api.VerenigingenPerInsz;
using Alba;

public static class AcmApiEndpoints
{
    public static VerenigingenPerInszResponse GetVerenigingenPerInsz(this IAlbaHost source, string insz)
        => source.GetAsJson<VerenigingenPerInszResponse>(url: $"/v1/verenigingen/{insz}")
                 .GetAwaiter().GetResult()!;
}
