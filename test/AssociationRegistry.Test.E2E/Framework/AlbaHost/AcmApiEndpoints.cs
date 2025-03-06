namespace AssociationRegistry.Test.E2E.Framework.AlbaHost;

using Acm.Api.VerenigingenPerInsz;
using Alba;
using System.Threading.Tasks;

public static class AcmApiEndpoints
{
    public static async Task<VerenigingenPerInszResponse> GetVerenigingenPerInsz(this IAlbaHost source, VerenigingenPerInszRequest request)
    {
        return await source.PostJson(request, $"/v1/verenigingen", JsonStyle.Mvc)
                     .Receive<VerenigingenPerInszResponse>();
    }
}
