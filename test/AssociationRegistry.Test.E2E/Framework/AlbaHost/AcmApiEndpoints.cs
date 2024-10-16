namespace AssociationRegistry.Test.E2E.Framework.AlbaHost;

using Acm.Api.VerenigingenPerInsz;
using Alba;

public static class AcmApiEndpoints
{
    public static async Task<VerenigingenPerInszResponse> GetVerenigingenPerInsz(this IAlbaHost source, string insz)
    {
        return await source.PostJson(new VerenigingenPerInszRequest(){ Insz = insz}, $"/v1/verenigingen", JsonStyle.Mvc)
                     .Receive<VerenigingenPerInszResponse>();
    }
}
