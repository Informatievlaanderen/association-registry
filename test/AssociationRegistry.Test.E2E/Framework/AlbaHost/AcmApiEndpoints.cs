namespace AssociationRegistry.Test.E2E.Framework.AlbaHost;

using Acm.Api.VerenigingenPerInsz;
using Alba;

public static class AcmApiEndpoints
{
    public static VerenigingenPerInszResponse GetVerenigingenPerInsz(this IAlbaHost source, string insz)
    {
        return source.PostJson(new VerenigingenPerInszRequest(), $"/v1/verenigingen?insz={insz}", JsonStyle.Mvc)
                     .Receive<VerenigingenPerInszResponse>()
                     .GetAwaiter().GetResult()!;
    }
}
